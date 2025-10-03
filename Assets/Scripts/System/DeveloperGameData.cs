using UnityEngine;
using Sirenix.OdinInspector;

public class DeveloperGameData : SerializedMonoBehaviour
{
    private int _redAppleToAdd = 0;
    private int _goldAppleToAdd = 0;
    private int _levelToUnlock = 0;
    private int _bonusToAdd = 0;

    private Bonus[] _bonuses;
    private Skin[] _skins;

    private void Start()
    {
        _bonuses = SaveSerial.Instance.BonusPrefabs;
        _skins = SaveSerial.Instance.SkinPrefabs;
    }

    public void ResetLeaderBoard() => LeaderBoard.Instance.ResetLB();

    public void ParseRedApple(string text) => int.TryParse(text, out _redAppleToAdd);
    public void ParseGoldApple(string text) => int.TryParse(text, out _goldAppleToAdd);
    public void ParseBonusCount(string text) => int.TryParse(text, out _bonusToAdd);
    public void ParseLevelToUnlock(string text) => int.TryParse(text, out _levelToUnlock);

    public void AddRedApple() => IncreaseApple(_redAppleToAdd, 0);
    public void AddGoldApple() => IncreaseApple(0, _goldAppleToAdd);
    public void AddBonus() => AddBonuses(_bonusToAdd);
    public void UnlockLevels() => CompleteLevels(_levelToUnlock);

    public void ResetUpgrade()
    {
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SavedSkinSkill);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SavedApple2XParam);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SavedHealthParam);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SavedLuckParam);
    }

    public void ResetAchievements()
    {
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.Achievements);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectedRedApples);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectedGoldApples);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CompletedLevels);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectedStars);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.PassDistance);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SpentRedApples);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SpentGoldApples);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.SpentBonuses);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectRareSkins);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectEpicSkins);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.CollectLegendarySkins);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.DestroyedMouses);
        SaveSerial.Instance.Reset(SaveSerial.JsonPaths.DestroyedHedgehogs);
}

    [Button, GUIColor(0, 1, 0)]
    public void UnlockAllSkins()
    {
        foreach (var skin in _skins)
        {
            if (skin.UnlockState == false)
                skin.Unlock();
        }
    }

    [Button, GUIColor(0,1,0)]
    public void IncreaseApple(int red, int gold)
    {
        int clampedRed = Mathf.Clamp(red, 1000, 999999);
        int clampedGold = Mathf.Clamp(gold, 100, 9999);

        Wallet.Instance.TryGetRedApple(clampedRed);
        Wallet.Instance.TryGetGoldApple(clampedGold);
    }

    [Button, GUIColor(0, 1, 0)]
    public void AddBonuses(int count)
    {
        int clampedValue = Mathf.Clamp(count, 0, 999);

        foreach (var bonus in _bonuses)
        {
            bonus.Add(clampedValue);
        }
    }

    [Button, GUIColor(0, 1, 0)]
    public void CompleteLevels(int count)
    {
        if (count < 1 || count >= SaveSerial.Instance.Levels.Length)
            return;

        for (int i = 0; i < count; i++)
        {
            SaveSerial.Instance.Save((i, new bool[3] {true, false, false }), SaveSerial.JsonPaths.LevelStars);
        }
    }


    [ButtonGroup, Button(ButtonSizes.Medium), GUIColor(1,0,0)]
    public void ResetSkins()
    {
        foreach (var skin in _skins)
        {
            if (skin.UnlockState)
            {
                skin.Block();
            }
        }
    }

    [ButtonGroup, Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void ResetLevels()
    {
        int count = SaveSerial.Instance.Levels.Length;
        for (int i = 0; i < count; i++)
        {
            Debug.Log(i);
            SaveSerial.Instance.Save((i, new bool[3]), SaveSerial.JsonPaths.LevelStars);
        }
    }

    [ButtonGroup, Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void ResetBonuses()
    {
        foreach (var bonus in _bonuses)
        {
            bonus.Decrese(bonus.Amount);
        }
    }

    [ButtonGroup("Currency"), Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void ResetRedApples()
    {
        Wallet.Instance.TrySpentRedApple(SaveSerial.Instance.LoadApple().Item1);
        Wallet.Instance.UpdateBalance();
    }

    [ButtonGroup("Currency"), Button(ButtonSizes.Medium), GUIColor(1, 0, 0)]
    public void ResetGoldApples()
    {
        Wallet.Instance.TrySpentGoldApple(SaveSerial.Instance.LoadApple().Item2);
        Wallet.Instance.UpdateBalance();
    }
}
