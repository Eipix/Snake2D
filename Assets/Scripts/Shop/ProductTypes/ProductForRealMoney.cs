using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductForRealMoney : Product
{
    [SerializeField] private TextMeshProUGUI _price;
    [SerializeField] private RawImage _currency;

    public TextMeshProUGUI Price => _price;
    public RawImage Currency => _currency;
    public DonateProductData DonatData => Data as DonateProductData;

    protected override void Start()
    {
        base.Start();
#if UNITY_WEBGL && !UNITY_EDITOR
        StartCoroutine(SetCurrency());
        StartCoroutine(GetPrice());
#endif
    }

    public override void BuyProduct()
    {
        Debug.Log($"Buy init: {DonatData.Type}");
        string id = ((int)DonatData.Type).ToString();

#if UNITY_EDITOR
        OnSucceed();
#else
        YandexMarkups.BuyProduct(id, transform.GetPathInHierarchy(), nameof(OnSucceed));
#endif
    }

    public virtual void OnSucceed() => Data.Add();

    //invoke extern
    public void SetPrice(string price) => _price.text = price;

    private IEnumerator GetPrice()
    {
        yield return new WaitUntil(() => YandexMarkups.Instance.IsInit);
        string id = ((int)DonatData.Type).ToString();
        YandexMarkups.GetPurchasePrice(id, transform.GetPathInHierarchy(), nameof(SetPrice));
    }

    private IEnumerator SetCurrency()
    {
        yield return new WaitWhile(() => YandexMarkups.Instance.Currency == null);
        _currency.texture = YandexMarkups.Instance.Currency;
    }
}
