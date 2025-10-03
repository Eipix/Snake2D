using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductForAd : Product
{
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private Image _currency;
    [SerializeField] private Image _circle;
    [SerializeField] private TextMeshProUGUI _adsCount;
    [SerializeField] private int _maxAds;
    [SerializeField] private string _key;

    private (int adsViewed, DateTime waitingTime) _adsData;

    public string Key => _key;

    private readonly int _hourToUpdate = 19;

    protected override void Awake()
    {
        base.Awake();
        _adsData = SaveSerial.Instance.Load<string, (int, DateTime)>(_key, SaveSerial.JsonPaths.ShopAds, (0, DateTime.MinValue));
    }

    private void OnEnable()
    {
        if(AdsAvailable())
        {
            Unlock();
        }
        else
        {
            BuyButton.interactable = false;
            _circle.gameObject.SetActive(false);
            float rgb = 115 / 255;
            var color = new Color(rgb, rgb, rgb, 0.5f);
            _price.color = color;
            _currency.color = color;
        }
    }

    public override void BuyProduct()
    {
        YandexMarkups.Instance.ShowRewardedAd(transform.GetPathInHierarchy(), nameof(OnRewarded), nameof(Nothing), () =>
        {
            OnRewarded();
        });
    }

    public void Nothing() { }

    public void OnRewarded()
    {
        Data.Add();
        _adsData.adsViewed += 1;
        SaveSerial.Instance.Save((_key, _adsData), SaveSerial.JsonPaths.ShopAds);
        _adsCount.text = (_maxAds - _adsData.adsViewed).ToString();

        UpdateAds();

#if !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
    }

    private bool AdsAvailable()
    {
        DateTime timeSource = DateTime.MinValue;
#if UNITY_WEBGL && !UNITY_EDITOR
        timeSource = YandexMarkups.Instance.ServerTime;
#endif
        return _adsData.adsViewed < _maxAds && timeSource >= _adsData.waitingTime;
    }

    private void Block()
    {
        BuyButton.interactable = false;
        _circle.gameObject.SetActive(false);

        var mainTime = DateTime.MinValue.AddYears(5000);
#if UNITY_WEBGL && !UNITY_EDITOR
            mainTime = YandexMarkups.Instance.ServerTime;
#endif
        var addTime = Extensions.GetHourToAdd(mainTime, _hourToUpdate);
        _adsData.waitingTime = mainTime.Add(addTime);
        _adsData.adsViewed = 0;
        SaveSerial.Instance.Save((_key, _adsData), SaveSerial.JsonPaths.ShopAds);
    }

    private void Unlock()
    {
        BuyButton.interactable = true;
        _circle.gameObject.SetActive(true);
        _adsCount.text = (_maxAds - _adsData.adsViewed).ToString();
    }

    private void UpdateAds()
    {
        if (AdsAvailable())
        {
            Unlock();
        }
        else
        {
            Debug.Log("Block");
            Block();
        }
    }
}
