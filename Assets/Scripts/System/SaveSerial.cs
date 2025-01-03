using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSerial : MonoBehaviour
{
    [field: SerializeField] public LevelData[] Waves { get; set; }
    [field: SerializeField] public LevelData Data { get; set; }
    [field: SerializeField] public LevelData[] Datas { get; set; }
    [field: SerializeField] public bool ArenaMode { get; set; }

    public int Level => _levelNumber;

    private int _levelNumber = 0;

    public void SaveApple(int apple, int goldApple)
    {
        SaveData data = new SaveData();
        data.SavedAppleCount = apple;
        data.SavedGoldAppleCount = goldApple;
        File.WriteAllText(Application.persistentDataPath + "/SavedApple.json", JsonUtility.ToJson(data));
    }
    public (int, int) LoadApple()
    {
        int apple = 0;
        int goldApple = 0;
        string path = Application.persistentDataPath + "/SavedApple.json";

        if (File.Exists(path))
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
            apple = data.SavedAppleCount;
            goldApple = data.SavedGoldAppleCount;
        }
        return (apple, goldApple);
    }

    public void CacheAllLevelsData(LevelData[] datas) => Datas = datas;

    public void SaveStars(bool[] calculatedStars)
    {
        string path = Application.persistentDataPath + "/SavedLevelStars.json";
        SaveData data = new SaveData();

        if (File.Exists(path))
            data.SavedStars = JsonConvert.DeserializeObject<List<bool[]>>(File.ReadAllText(path));

        for (int i = 0; i < 3; i++)
        {
            if (calculatedStars[i] == true)
                data.SavedStars[Data.LevelIndex][i] = calculatedStars[i];
        }
        File.WriteAllText(Application.persistentDataPath + "/SavedLevelStars.json", JsonConvert.SerializeObject(data.SavedStars));
    }
    public bool[] LoadStars(int indexOfLevel)
    {
        string path = Application.persistentDataPath + "/SavedLevelStars.json";
        if (File.Exists(path))
        {
            SaveData data = new SaveData();
            data.SavedStars = JsonConvert.DeserializeObject<List<bool[]>>(File.ReadAllText(path));

            return indexOfLevel >= data.SavedStars.Count || indexOfLevel < 0
                ? new bool[] { false, false, false }
                : data.SavedStars[indexOfLevel];
        }
        else
        {
            return new bool[] { false, false, false };
        }
    }

    public void SaveIndicatorPositions(Vector2 currentPosition)
    {
        SaveData data = new SaveData();
        data.SavedIndicatorPosition[0] = currentPosition.x;
        data.SavedIndicatorPosition[1] = currentPosition.y;
        File.WriteAllText(Application.persistentDataPath + "/SavedIndicatorPosition.json", JsonConvert.SerializeObject(data.SavedIndicatorPosition));
    }
    public Vector2 LoadIndicatorPosition()
    {
        string path = Application.persistentDataPath + "/SavedIndicatorPosition.json";
        Vector2 position = Vector2.zero;

        if (File.Exists(path))
        {
            SaveData data = new SaveData();
            data.SavedIndicatorPosition = JsonConvert.DeserializeObject<List<float>>(File.ReadAllText(path));
            position.x = data.SavedIndicatorPosition[0];
            position.y = data.SavedIndicatorPosition[1];
        }
        return position;
    }

    public void SaveBonusAmount(Bonus bonus, int amount)
    {
        int index = 0;
        CheckIndexOfBonuse(bonus, ref index);

        string path = Application.persistentDataPath + "/SavedBonuses.json";
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonus = JsonConvert.DeserializeObject<Dictionary<Bonuses, int>>(File.ReadAllText(path));

        if (data.SavedBonus.ContainsKey((Bonuses)index))
            data.SavedBonus[(Bonuses)index] = amount;
        else
            data.SavedBonus.Add((Bonuses)index, amount);

        File.WriteAllText(Application.persistentDataPath + "/SavedBonuses.json", JsonConvert.SerializeObject(data.SavedBonus));
    }
    public int LoadBonusAmount(Bonus bonus)
    {
        int index = 0;
        CheckIndexOfBonuse(bonus, ref index);

        string path = Application.persistentDataPath + "/SavedBonuses.json";
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
        if (bonus is Bomb)
            index = 0;
        else if (bonus is Cheese)
            index = 1;
        else if (bonus is Coin)
            index = 2;
        else if (bonus is GoldPeach)
            index = 3;
        else if (bonus is IceCube)
            index = 4;
        else if (bonus is Lightning)
            index = 5;
        else if (bonus is Peach)
            index = 6;
        else if (bonus is Timer)
            index = 7;
        else
            Debug.LogError("В сохранение не указан тип Бонуса");
    }

    public void SaveBonusInSlots(Bonus bonus, int increase = 1)
    {
        string path = Application.persistentDataPath + "/SavedBonusInSlot.json";
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

        File.WriteAllText(Application.persistentDataPath + "/SavedBonusInSlot.json", JsonConvert.SerializeObject(data.SavedBonusInSlots));
    }
    public Dictionary<string, int> LoadBonusInSlots()
    {
        string path = Application.persistentDataPath + "/SavedBonusInSlot.json";
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedBonusInSlots = JsonConvert.DeserializeObject<Dictionary<string, int>>(File.ReadAllText(path));
        return data.SavedBonusInSlots;
    }

    public void SaveHealth(int health)
    {
        SaveData data = new SaveData();
        data.SavedHealth = health;
        File.WriteAllText(Application.persistentDataPath + "/SavedHealth.json", JsonConvert.SerializeObject(data.SavedHealth));
    }
    public int LoadHealth()
    {
        string path = Application.persistentDataPath + "/SavedHealth.json";
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedHealth = JsonConvert.DeserializeObject<int>(File.ReadAllText(path));
        return data.SavedHealth;
    }

    public void SaveControl(int control)
    {
        SaveData data = new SaveData();
        data.SavedControl = control;
        File.WriteAllText(Application.persistentDataPath + "/SavedControl.json", JsonConvert.SerializeObject(data.SavedControl));
    }
    public int LoadControl()
    {
        string path = Application.persistentDataPath + "/SavedControl.json";
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedControl = JsonConvert.DeserializeObject<int>(File.ReadAllText(path));
        return data.SavedControl;
    }

    public void SaveLuck(int luck)
    {
        SaveData data = new SaveData();
        data.SavedLuck = luck;
        File.WriteAllText(Application.persistentDataPath + "/SavedLuck.json", JsonConvert.SerializeObject(data.SavedLuck));
    }
    public int LoadLuck()
    {
        string path = Application.persistentDataPath + "/SavedLuck.json";
        SaveData data = new SaveData();
        if (File.Exists(path))
            data.SavedLuck = JsonConvert.DeserializeObject<int>(File.ReadAllText(path));
        return data.SavedLuck;
    }

    public void SaveSkinSkill(int value, Skin equippedSkin)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + "/SavedSkinSkill.json";

        if (File.Exists(path))
            data.SavedSkinSkills = JsonConvert.DeserializeObject<Dictionary<Type, int>>(File.ReadAllText(path));

        if (data.SavedSkinSkills.ContainsKey(equippedSkin.GetType()))
            data.SavedSkinSkills[equippedSkin.GetType()] = value;
        else
            data.SavedSkinSkills.Add(equippedSkin.GetType(), value);

        File.WriteAllText(Application.persistentDataPath + "/SavedSkinSkill.json", JsonConvert.SerializeObject(data.SavedSkinSkills));
    }

    public int LoadSkinSkill(Skin skin)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + "/SavedSkinSkill.json";
        if (File.Exists(path))
            data.SavedSkinSkills = JsonConvert.DeserializeObject<Dictionary<Type, int>>(File.ReadAllText(path));

        return data.SavedSkinSkills.ContainsKey(skin.GetType()) ? data.SavedSkinSkills[skin.GetType()] : 0;
    }

    public void SaveParametr(int value, Parametr parametr, Skin equippedSkin = null)
    {
        if (parametr is HealthParametr)
            SaveHealth(value);
        else if (parametr is AppleDouble)
            SaveControl(value);
        else if (parametr is LuckParametr)
            SaveLuck(value);

        if (equippedSkin != null)
            SaveSkinSkill(value, equippedSkin);
    }
    public int LoadParametr(Parametr parametr, Skin equippedSkin = null)
    {
        int value = 0;
        if (parametr is HealthParametr)
            value = LoadHealth();
        else if (parametr is AppleDouble)
            value = LoadControl();
        else if (parametr is LuckParametr)
            value = LoadLuck();

        if (equippedSkin != null)
            value = LoadSkinSkill(equippedSkin);

        return value;
    }

    public void SaveDataSkin(Skin skin, bool lockState)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + "/SavedSkins.json";

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
        File.WriteAllText(Application.persistentDataPath + "/SavedSkins.json", JsonConvert.SerializeObject(data.SavedDataSkins));
    }
    public bool LoadLockState(Type skinType)
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + "/SavedSkins.json";
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
        SaveData data = new SaveData();
        data.SavedCurrentSkin = skin.GetType().ToString();
        File.WriteAllText(Application.persistentDataPath + "/SavedCurrentSkin.json", JsonConvert.SerializeObject(data.SavedCurrentSkin));
    }
    public string LoadCurrentSkinType()
    {
        SaveData data = new SaveData();
        string path = Application.persistentDataPath + "/SavedCurrentSkin.json";
        if (File.Exists(path))
        {
            data.SavedCurrentSkin = JsonConvert.DeserializeObject<string>(File.ReadAllText(path));
        }
        return data.SavedCurrentSkin;
    }

    public void ResetData()
    {
        try
        {
            File.Delete(Application.persistentDataPath + "/SavedApple.json");
            File.Delete(Application.persistentDataPath + "/SavedLevelNumber.json");
            File.Delete(Application.persistentDataPath + "/SavedLevelStars.json");
            File.Delete(Application.persistentDataPath + "/SavedIndicatorPosition.json");
            File.Delete(Application.persistentDataPath + "/SavedChances.json");
            File.Delete(Application.persistentDataPath + "/SavedBonuses.json");
            File.Delete(Application.persistentDataPath + "/SavedBonusInSlots.json");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}

[Serializable]
public class SaveData
{
    [HideInInspector] public int SavedLevelNumber;

    [HideInInspector] public int SavedAppleCount;
    [HideInInspector] public int SavedGoldAppleCount;

    [HideInInspector] public int SavedHealth;
    [HideInInspector] public int SavedControl;
    [HideInInspector] public int SavedLuck;

    [HideInInspector] public string SavedCurrentSkin;

    [HideInInspector] public List<bool[]> SavedStars = new List<bool[]>();
    [HideInInspector] public List<float> SavedIndicatorPosition = new List<float>();
    [HideInInspector] public List<string> SavedDataSkins = new List<string>();
    [HideInInspector] public Dictionary<Bonuses, int> SavedBonus = new Dictionary<Bonuses, int>();
    [HideInInspector] public Dictionary<string, int> SavedBonusInSlots = new Dictionary<string, int>();
    [HideInInspector] public Dictionary<Type, int> SavedSkinSkills = new Dictionary<Type, int>();

    public SaveData()
    {
        SavedHealth = 3;
        SavedControl = 0;
        SavedLuck = 0;
        SavedCurrentSkin = "";

        for (int i = 0; i < 10; i++)
        {
            bool[] falses = new bool[3];
            SavedStars.Add(falses);
        }

        SavedIndicatorPosition.Add(0f);
        SavedIndicatorPosition.Add(0f);

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