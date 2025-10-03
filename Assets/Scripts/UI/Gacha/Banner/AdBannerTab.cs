using System;
using System.Collections.Generic;
using UnityEngine;

public class AdBannerTab : BannerTab, IAd
{
    [SerializeField] private GameObject _adBanner;
    [SerializeField] private BannerTab _defaultTab;
    [SerializeField] private string _key;
    [field:SerializeField] public int MaxAds { get; set; }

    private (List<IGachaReward> rewards, List<GachaResults> statuses) _summonData;
    private IAd _this;

    private int _count;

    public DateTime WaitingTime { get; set; }
    public int AdsViewed { get; set; }
    public string Path { get; set; }
    public int HourToUpdate => 19;
    public string Key => _key;

    protected override void Awake()
    {
        base.Awake();
        _this = this;
        Path = transform.GetPathInHierarchy();
        (AdsViewed, WaitingTime) = SaveSerial.Instance.Load<string, (int, DateTime)>(Key, SaveSerial.JsonPaths.Ads, (0, DateTime.MinValue));
    }

    private void OnEnable()
    {
        if(_this.AdsAvailable() == false)
            _adBanner.SetActive(false);
    }

    public override void OnSummonClick(int count)
    {
        _count = count;
        WatchAd();
    }

    public void WatchAd()
    {
        YandexMarkups.Instance.ShowRewardedAd(Path, nameof(GetReward), nameof(AdClose), () =>
        {
            SendMessage(nameof(GetReward));
            SendMessage(nameof(AdClose));
        });
    }

    public void GetReward()
    {
        _summonData.rewards = Summoner.GetRewards(_count, out _summonData.statuses);
        AdsViewed += 1;
        SaveSerial.Instance.Save((_key, (AdsViewed, WaitingTime)), SaveSerial.JsonPaths.Ads);
        _this.UpdateAds();
#if !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
    }

    public void AdClose() => Summoner.Summon(_count, _summonData);

    public void Block()
    {
        _defaultTab.Set();
        _this.IncreaseTimer();
        _adBanner.SetActive(false);
    }

    public void Unlock() { }
}
