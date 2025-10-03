using System;

public interface IAd
{
    public DateTime WaitingTime { get; set; }
    public int AdsViewed { get; set; }
    public int MaxAds { get; set; }
    public int HourToUpdate { get; }
    public string Key { get; }
    public string Path { get; set; }

    public void Block();

    public void Unlock();

    public void WatchAd();

    public bool AdsAvailable()
    {
        DateTime timeSource = DateTime.Now;
#if UNITY_WEBGL && !UNITY_EDITOR
        timeSource = YandexMarkups.Instance.ServerTime;
#endif
        return AdsViewed < MaxAds && timeSource > WaitingTime;
    }

    public void UpdateAds()
    {
        if (AdsAvailable())
        {
            Unlock();
        }
        else
        {
            Block();
        }
    }

    public void IncreaseTimer()
    {
        var mainTime = DateTime.Now;
#if UNITY_WEBGL && !UNITY_EDITOR
            mainTime = YandexMarkups.Instance.ServerTime;
#endif
        var addTime = Extensions.GetHourToAdd(mainTime, HourToUpdate);
        WaitingTime = mainTime.Add(addTime);
        AdsViewed = 0;
        SaveSerial.Instance.Save((Key, (AdsViewed, WaitingTime)), SaveSerial.JsonPaths.Ads);
    }
}
