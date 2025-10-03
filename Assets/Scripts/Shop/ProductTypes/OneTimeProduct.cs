using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class OneTimeProduct : ProductForRealMoney
{
    [SerializeField] private string _key;

    public event UnityAction Bought;
    private bool _isBought;

    protected override void Awake()
    {
        base.Awake();
        _isBought = SaveSerial.Instance.Load(_key, SaveSerial.JsonPaths.ConsumedProducts, false);
        if (_isBought)
        {
            BuyButton.interactable = false;
            float rgb = 115 / 255;
            var color = new Color(rgb, rgb, rgb, 0.5f);
            Price.color = color;
            Currency.color = color;
        }
    }

    public override void OnSucceed()
    {
        base.OnSucceed();
        BuyButton.interactable = false;
        _isBought = true;
        SaveSerial.Instance.Save((_key, _isBought), SaveSerial.JsonPaths.ConsumedProducts);
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
        Bought?.Invoke();
    }
}
