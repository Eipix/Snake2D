using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class SaveSerial : Singleton<SaveSerial>
{
    [field:SerializeField] public Level[] Levels { get; set; }
    [field: SerializeField] public Skin[] SkinPrefabs { get; private set; }
    [field: SerializeField] public Bonus[] BonusPrefabs { get; private set; }

    [field: SerializeField] public LevelData[] Waves { get; set; }
    [field: SerializeField] public LevelData Data { get; set; }
    [field: SerializeField] public LevelData[] Datas { get; set; }
    [field: SerializeField] public bool ArenaMode { get; set; }
    [field: SerializeField] public LevelMode Mode { get; set; }

    public event UnityAction DataLoaded;

    private string[] _saveFiles;

    [DllImport("__Internal")]
    private static extern void SaveExtern(string json);

    [DllImport("__Internal")]
    public static extern void LoadExtern();

    public static IEnumerator WaitInitCoroutine()
    {
        yield return new WaitWhile(() => Instance == null);
        LoadExtern();
    }

    public override void OnInit()
    {
        base.OnInit();
        _saveFiles = GetJsons();
    }

    private string[] GetJsons()
    {
        var fields = typeof(JsonPaths).GetFields(BindingFlags.Public | BindingFlags.Static);
        var fieldInfo = fields.Where(field => field.IsLiteral && field.FieldType == typeof(string)).ToArray();
        string[] jsons = new string[fieldInfo.Length];
        for (int i = 0; i < jsons.Length; i++)
        {
            jsons[i] = (string)fieldInfo[i].GetRawConstantValue();
        }
        return jsons;
    }

    public void SaveGame()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Dictionary<string, string> valueOfPaths = new Dictionary<string, string>();
        var files = _saveFiles;
        foreach (var json in files)
        {
            var jsonPath = Application.persistentDataPath + json;
            if (File.Exists(jsonPath))
            {
                valueOfPaths.Add(json, File.ReadAllText(jsonPath));
            }
        }
        string gameDataJson = JsonConvert.SerializeObject(valueOfPaths);
        SaveExtern(gameDataJson);
#endif
    }

    //invoke extern
    public void LoadGame(string serverJson)
    {
        Dictionary<string, string> gameData;
        gameData = JsonConvert.DeserializeObject<Dictionary<string, string>>(serverJson);
        foreach (var data in gameData)
        {
            string path = Application.persistentDataPath + data.Key;
            File.WriteAllText(path, data.Value);
        }
        Wallet.Instance.UpdateBalance();
        DataLoaded?.Invoke();
    }

    public void Increment(int number, string file)
    {
        string path = Application.persistentDataPath + file;
        if(File.Exists(path))
            number += JsonConvert.DeserializeObject<int>(File.ReadAllText(path));

        var json = JsonConvert.SerializeObject(number);
        File.WriteAllText(path, json);
    }
    public void Save<T>(T t, string file)
    {
        string path = Application.persistentDataPath + file;
        var json = JsonConvert.SerializeObject(t);
        File.WriteAllText(path, json);
    }
    public T Load<T>(string file, T defaultValue = default)
    {
        string path = Application.persistentDataPath + file;
        T t = defaultValue;

        if (File.Exists(path))
        {
            t = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
        return t;
    }

    public void Save<T0,T1>((T0 key,T1 value) newData, string file)
    {
        string path = Application.persistentDataPath + file;
        Dictionary<T0, T1> data = new Dictionary<T0, T1>();

        if (File.Exists(path))
            data = JsonConvert.DeserializeObject<Dictionary<T0,T1>>(File.ReadAllText(path));

        if (data.ContainsKey(newData.key))
            data[newData.key] = newData.value;
        else
            data.Add(newData.key, newData.value);

        var json = JsonConvert.SerializeObject(data);
        File.WriteAllText(path, json);
    }

    public TValue Load<TKey,TValue>(TKey key, string file, TValue defaultValue = default)
    {
        string path = Application.persistentDataPath + file;
        Dictionary<TKey, TValue> datas = new Dictionary<TKey, TValue>();

        if (File.Exists(path))
            datas = JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(File.ReadAllText(path));

        return datas.ContainsKey(key)
                   ? datas[key]
                   : defaultValue;
    }

    public void SaveApple(int apple, int goldApple)
    {
        string path = Application.persistentDataPath + JsonPaths.SavedApple;
        File.WriteAllText(path, JsonConvert.SerializeObject(new int[] { apple, goldApple }));
    }
    public (int, int) LoadApple()
    {
        int apple = 0;
        int goldApple = 0;
        string path = Application.persistentDataPath + JsonPaths.SavedApple;

        if (File.Exists(path))
        {
            int[] apples = JsonConvert.DeserializeObject<int[]>(File.ReadAllText(path));
            apple = apples[0];
            goldApple = apples[1];
        }
        return (apple, goldApple);
    }

    public void SaveBonusAmount(Bonus bonus, int amount)
    {
        int index = 0;
        CheckIndexOfBonuse(bonus, ref index);

        string path = Application.persistentDataPath + JsonPaths.SavedBonuses;
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonus = JsonConvert.DeserializeObject<Dictionary<Bonuses, int>>(File.ReadAllText(path));

        if (data.SavedBonus.ContainsKey((Bonuses)index))
            data.SavedBonus[(Bonuses)index] = amount;
        else
            data.SavedBonus.Add((Bonuses)index, amount);

        File.WriteAllText(path, JsonConvert.SerializeObject(data.SavedBonus));
    }
    public int LoadBonusAmount(Bonus bonus)
    {
        int index = 0;
        CheckIndexOfBonuse(bonus, ref index);

        string path = Application.persistentDataPath + JsonPaths.SavedBonuses;
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonus = JsonConvert.DeserializeObject<Dictionary<Bonuses, int>>(File.ReadAllText(path));

        int amount = 0;
        if (data.SavedBonus.ContainsKey((Bonuses)index))
            amount = data.SavedBonus[(Bonuses)index];

        return amount;
    }
    public void CheckIndexOfBonuse(Bonus bonus, ref int index)
    {
        Dictionary<Type, int> indexOfType = new Dictionary<Type, int>()
        {
            { typeof(Bomb), 0}, { typeof(Cheese), 1}, { typeof(Coin), 2}, 
            { typeof(GoldPeach), 3}, { typeof(IceCube), 4}, { typeof(Lightning), 5}, 
            { typeof(Peach), 6}, { typeof(Timer), 7}
        };
        if (indexOfType.TryGetValue(bonus.GetType(), out int value))
            index = value;
        else
            Debug.LogError("В сохранение не указан тип Бонуса");
    }

    public void SaveBonusInSlots(Bonus bonus, int increase = 1)
    {
        string path = Application.persistentDataPath + JsonPaths.SavedSlots;
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonusInSlots = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(path));

        string type = bonus.GetType().ToString();
        if (!data.SavedBonusInSlots.ContainsKey(type) && increase > 0)
        {
            data.SavedBonusInSlots.Add(type, 1);
        }
        else if (data.SavedBonusInSlots.ContainsKey(type))
        {
            if (data.SavedBonusInSlots[type] <= Math.Abs(increase) && increase < 0)
                data.SavedBonusInSlots.Remove(type);
            else
                data.SavedBonusInSlots[type] += increase;
        }

        File.WriteAllText(path, JsonConvert.SerializeObject(data.SavedBonusInSlots));
    }
    public Dictionary<string, int> LoadBonusInSlots()
    {
        string path = Application.persistentDataPath + JsonPaths.SavedSlots;
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonusInSlots = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(path));
        return data.SavedBonusInSlots;
    }

    public void SaveSkinSkill(int value, Skin equippedSkin)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + JsonPaths.SavedSkinSkill;

        if (File.Exists(path))
            data.SavedSkinSkills = JsonConvert.DeserializeObject<Dictionary<Type, int>>(File.ReadAllText(path));

        if (data.SavedSkinSkills.ContainsKey(equippedSkin.GetType()))
            data.SavedSkinSkills[equippedSkin.GetType()] = value;
        else
            data.SavedSkinSkills.Add(equippedSkin.GetType(), value);

        File.WriteAllText(path, JsonConvert.SerializeObject(data.SavedSkinSkills));
    }

    public int LoadSkinSkill(Skin skin)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + JsonPaths.SavedSkinSkill;
        if (File.Exists(path))
            data.SavedSkinSkills = JsonConvert.DeserializeObject<Dictionary<Type, int>>(File.ReadAllText(path));

        return data.SavedSkinSkills.ContainsKey(skin.GetType())
                                   ? data.SavedSkinSkills[skin.GetType()]
                                   : 0;
    }

    public void SaveParametr(int value, Parametr parametr = null, Skin equippedSkin = null)
    {
        if (equippedSkin != null)
            SaveSkinSkill(value, equippedSkin);

        if (parametr == null) return;

        if (parametr is HealthParametr)
            Save(value, JsonPaths.SavedHealthParam);
        else if (parametr is AppleDouble)
            Save(value, JsonPaths.SavedApple2XParam);
        else if (parametr is LuckParametr)
            Save(value, JsonPaths.SavedLuckParam);

    }
    public int LoadParametr(Parametr parametr, Skin equippedSkin = null)
    {
        int value = 0;
        if (parametr is HealthParametr)
            value = Load<int>(JsonPaths.SavedHealthParam, 3);
        else if (parametr is AppleDouble)
            value = Load<int>(JsonPaths.SavedApple2XParam);
        else if (parametr is LuckParametr)
            value = Load<int>(JsonPaths.SavedLuckParam);

        if (equippedSkin != null)
            value = LoadSkinSkill(equippedSkin);

        return value;
    }

    public void SaveDataSkin(Skin skin, bool lockState)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + JsonPaths.SavedSkins;

        if (File.Exists(path))
            data.SavedDataSkins = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(path));

        bool notFound = true;
        for (int i = 0; i < data.SavedDataSkins.Count; i++)
        {
            string[] skinData = data.SavedDataSkins[i].Split(':');
            if (skinData[0] == skin.GetType().ToString())
            {
                string skinDataToString = $"{skin.GetType()}:{lockState}";
                data.SavedDataSkins[i] = skinDataToString;
                notFound = false;
            }
        }
        if (notFound)
        {
            string skinData = $"{skin.GetType()}:{lockState}";
            data.SavedDataSkins.Add(skinData);
        }
        File.WriteAllText(Application.persistentDataPath + JsonPaths.SavedSkins, JsonConvert.SerializeObject(data.SavedDataSkins));
    }
    public bool LoadLockState(Type skinType)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + JsonPaths.SavedSkins;
        bool lockState = false;

        if (File.Exists(path))
        {
            data.SavedDataSkins = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(path));
        }

        for (int i = 0; i < data.SavedDataSkins.Count; i++)
        {
            string[] skinData = data.SavedDataSkins[i].Split(':');
            if (skinData[0] == skinType.ToString())
            {
                bool.TryParse(skinData[1], out lockState);
            }
        }
        return lockState;
    }

    public void SaveCurrentSkin(Skin skin)
    {
        string path = Application.persistentDataPath + JsonPaths.SavedCurrentSkin;
        string savedSkin = skin != null
            ? skin.GetType().ToString()
            : typeof(Pagko).ToString();
        File.WriteAllText(path, JsonConvert.SerializeObject(savedSkin));
    }
    public string LoadCurrentSkinType()
    {
        string savedSkinTypeName = "";
        string path = Application.persistentDataPath + JsonPaths.SavedCurrentSkin;
        if (File.Exists(path))
        {
            savedSkinTypeName = JsonConvert.DeserializeObject<string>(File.ReadAllText(path));
        }
        return savedSkinTypeName;
    }

    public void Reset(string file)
    {
        string path = Application.persistentDataPath + file;
        if (File.Exists(path))
        {
            File.Delete(path);
#if UNITY_WEBGL && !UNITY_EDITOR
            SaveSerial.Instance.SaveGame();
#endif
        }
    }

    public static class JsonPaths
    {
        public const string IndicatorPosition = "/SavedIndicatorPosition.json";
        public const string SavedApple = "/SavedApple.json";
        public const string LevelStars = "/LevelStars.json";

        public const string SavedBonuses = "/SavedBonuses.json";
        public const string SavedSlots = "/SavedBonusInSlot.json";

        public const string SavedCurrentSkin = "/SavedCurrentSkin.json";
        public const string SavedSkins = "/SavedSkins.json";
        public const string SavedSkinSkill = "/SavedSkinSkill.json";

        public const string SavedApple2XParam = "/SavedApple2X.json";
        public const string SavedLuckParam = "/SavedLuck.json";
        public const string SavedHealthParam = "/SavedHealth.json";
        public const string SavedTotalCollectedApples = "/SavedTotalCollectedApples.json";

        public const string ControllerType = "/SavedControllerType.json";
        public const string SpinAdTime = "/SpinAdTime.json";
        public const string SpinAdAttempts = "/SpinAdAttempts.json";
        public const string ShopAds = "/ShopAds.json";
        public const string Ads = "/Ads.json";
        public const string ConsumedProducts = "/ShopConsumedProducts.json";

        public const string Achievements = "/Achievements.json";
        public const string CollectedRedApples = "/CollectedRedAppples.json";
        public const string CollectedGoldApples = "/CollectedGoldAppples.json";
        public const string CompletedLevels = "/CompleteLevel.json";
        public const string CollectedStars = "/CollectedStars.json";
        public const string PassDistance = "/PassDistance.json";
        public const string SpentRedApples = "/SpentRedApples.json";
        public const string SpentGoldApples = "/SpentGoldApples.json";
        public const string SpentBonuses = "/SpentBonuses.json";
        public const string CollectRareSkins = "/CollectRareSkins.json";
        public const string CollectEpicSkins = "/CollectEpicSkins.json";
        public const string CollectLegendarySkins = "/CollectLegendarySkins.json";
        public const string DestroyedMouses = "/DestroyedMouses.json";
        public const string DestroyedHedgehogs = "/DestroyedHedgehogs.json";

        public const string Volumes = "/Volumes.json";
        public const string ShowTutorialWindow = "/ShowTutorial.json";
    }
}

[Serializable]
public class SaveData
{
    [HideInInspector] public List<string> SavedDataSkins = new List<string>();
    [HideInInspector] public Dictionary<Bonuses, int> SavedBonus = new Dictionary<Bonuses, int>();
    [HideInInspector] public Dictionary<string, int> SavedBonusInSlots = new Dictionary<string, int>();
    [HideInInspector] public Dictionary<Type, int> SavedSkinSkills = new Dictionary<Type, int>();

    public SaveData()
    {
        foreach (Bonuses bonus in Enum.GetValues(typeof(Bonuses)))
            SavedBonus.Add(bonus, 0);
    }
}
public enum Bonuses
{
    Bomb,
    Cheese,
    Coin,
    GoldPeach,
    IceCube,
    Lightning,
    Peach,
    Timer
}