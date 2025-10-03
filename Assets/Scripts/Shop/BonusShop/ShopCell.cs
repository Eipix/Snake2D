using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class ShopCell : MonoBehaviour
{
    [FoldoutGroup("Childrens"), ChildGameObjectsOnly]
    [SerializeField] private Image _icon;
    [FoldoutGroup("Childrens"), ChildGameObjectsOnly]
    [SerializeField] private TextMeshProUGUI _label;
    [FoldoutGroup("Childrens"), ChildGameObjectsOnly]
    [SerializeField] private Image _currencyImage;

    [SerializeField] private ShopCounter _counter;
    [SerializeField] private MovableObject _frame;
    [SerializeField] private MovableObject _shadow;
    [SerializeField] private ShopArt _shopArt;
    [SerializeField] private Bonus _bonus;
    [SerializeField] private Apple _currency;
    [SerializeField] private int _price;

    public UnityAction<ShopCell> _selected;

    private RectTransform _rectTransform;

    public Apple Currency => _currency;
    public Bonus Bonus => _bonus;
    public int Price => _price;

    private void OnEnable() => _selected += _counter.OnSelected;
    private void OnDisable() => _selected -= _counter.OnSelected;

    private void Awake()
    {
        _currencyImage.sprite = _currency.Sprite;
        _label.text = _price.ToString();
        _rectTransform = transform as RectTransform;
        _icon.sprite = _bonus.SmallIcon;
        _icon.SetNativeSize();
        _icon.rectTransform.sizeDelta *= 0.8f;
    }

    public void OnButtonClick()
    {
        _shadow.Move(_rectTransform);
        _frame.Move(_rectTransform);
        _shopArt.Set(_bonus);
        _selected?.Invoke(this);
    }
}
