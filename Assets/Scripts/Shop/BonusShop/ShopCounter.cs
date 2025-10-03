using UnityEngine;
using TMPro;

public class ShopCounter : MonoBehaviour
{
    [SerializeField] private ShopCell _default;
    [SerializeField] private TextMeshProUGUI _redApplesTotalPrice;
    [SerializeField] private TextMeshProUGUI _goldApplesTotalPrice;
    [SerializeField] private TextMeshProUGUI _count;
    [SerializeField] private ShopArt _shopArt;

    private ShopCell _selected;
    private TextMeshProUGUI _currentBalance;

    private int _amount;

    private int TotalPrice => _amount * _selected.Price;

    private readonly int _maxValue = 99;
    private readonly int _minValue = 1;

    private void Awake() => OnSelected(_default);

    private void OnDisable() => ResetValues();

    public void OnSelected(ShopCell cell)
    {
        ResetValues();
        _shopArt.Set(cell.Bonus);
        _selected = cell;
        _currentBalance = GetBalanceByCurrency(_selected.Currency);
        _currentBalance.text = TotalPrice.ToString();
    }

    public void Increment()
    {
        if (_amount >= _maxValue)
            return;

        _amount++;
        UpdateValues();
    }

    public void Decrement()
    {
        if (_amount <= _minValue)
            return;

        _amount--;
        UpdateValues();
    }

    public void Buy()
    {
        if(Wallet.Instance.TrySpentApples(_selected.Currency, TotalPrice))
        {
            _selected.Bonus.Add(_amount);
#if UNITY_WEBGL && !UNITY_EDITOR
            SaveSerial.Instance.SaveGame();
#endif
        }
        else
        {
            Notification.Instance.Notify(Notification.Instance.LangNotEnough.Translate);
        }
    }

    private TextMeshProUGUI GetBalanceByCurrency(Apple currency)
    {
        return currency is RedApple
            ? _redApplesTotalPrice
            : _goldApplesTotalPrice;
    }

    private void UpdateValues()
    {
        _count.text = $"x{_amount}";
        _currentBalance.text = TotalPrice.ToString();
    }

    private void ResetValues()
    {
        _amount = _minValue;
        _count.text = $"x{_minValue}";
        _redApplesTotalPrice.text = "0";
        _goldApplesTotalPrice.text = "0";
    }
}
