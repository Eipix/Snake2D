using System;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public class BannerTab : SerializedMonoBehaviour
{
    [Header("References")]
    [SerializeField] private BannerSwiper _swiper;
    [SerializeField] private Summoner _summoner;
    //[SerializeField] private Notification _notification;
    [SerializeField] private GameObject _summons;

    [Header("Sprites")]
    [SerializeField] private Translatable<Sprite>[] _langBanner;
    [SerializeField] private Sprite[] _spinButtons;

    [Header("Components")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _shadow;

    [field: Space]
    [ShowInInspector, OdinSerialize] private BannerData _data;
    [field: SerializeField] public SpinData SpinData { get; private set; }

    public GameObject Summons => _summons;
    public Summoner Summoner => _summoner;
    public Sprite[] SpinButtons => _spinButtons;
    public Translatable<Sprite>[] LangBanners => _langBanner;
    public BannerData Data => _data;

    protected virtual void Awake() => FadeZero();

    public virtual void OnSummonClick(int count)
    {
        var price = SpinData.Price * count;
        var apple = SpinData.Apple as Apple;

        if (Wallet.Instance.TrySpentApples(apple, price))
        {
            _summoner.Summon(count);
        }
        else
        {
            Notification.Instance.Notify(Notification.Instance.LangNotEnough.Translate);
        }
    }

    public virtual void Set()
    {
        _summons.SetActive(true);
        _swiper.ChangeBannerData(this);
        _shadow.color = _shadow.color.Fade(1f);
    }

    public void FadeZero() => _shadow.color = _shadow.color.Fade(0f);
}

[Serializable]
public class SpinData
{
    [field: SerializeField] public Item Apple { get; set; }
    [field: SerializeField] public int Price { get; set; }
}
