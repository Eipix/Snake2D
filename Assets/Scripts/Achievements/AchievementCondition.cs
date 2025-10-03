using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AchievementCondition
{
    [field: SerializeField] public Item Reward { get; private set; }
    [field: SerializeField] public int RewardCount { get; private set; }
    [field: SerializeField] public int TargetValue { get; private set; }

    public UnityAction Collected;
    public UnityAction Completed;
    public AchievementData Data { get; set; }

    public void Complete((string key, AchievementData[] datas) saveData)
    {
        Data.IsCompleted = true;
        SaveSerial.Instance.Save(saveData, SaveSerial.JsonPaths.Achievements);
        Completed?.Invoke();
    }

    public void Collect((string, AchievementData[]) data)
    {
        Reward.Add(RewardCount);
        Data.IsCollected = true;
        SaveSerial.Instance.Save(data, SaveSerial.JsonPaths.Achievements);

        if (Reward is Apple)
            LeaderBoard.Instance.Add(RewardCount);

        SaveSerial.Instance.SaveGame();
        Collected?.Invoke();
    }
}
