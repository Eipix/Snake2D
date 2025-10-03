using System.Runtime.InteropServices;

public class LeaderBoard : Singleton<LeaderBoard>
{
    [DllImport("__Internal")]
    public static extern void SetToLeaderboard(int value);

    public void Add(int value)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var totalCollected = SaveSerial.Instance.Load<int>(SaveSerial.JsonPaths.SavedTotalCollectedApples) + value;
        SetToLeaderboard(totalCollected);
        SaveSerial.Instance.Save(totalCollected, SaveSerial.JsonPaths.SavedTotalCollectedApples);
#endif
    }

    public void ResetLB()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetToLeaderboard(0);
        SaveSerial.Instance.Save(0, SaveSerial.JsonPaths.SavedTotalCollectedApples);
#endif
    }
}
