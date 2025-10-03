using UnityEngine;
using UnityEngine.UI;

public abstract class Product : MonoBehaviour
{
    [SerializeField] private ProductData _productData;

    private Button _buyButton;

    protected Button BuyButton => _buyButton;
    protected ProductData Data => _productData;

    protected virtual void Awake() => _buyButton = GetComponentInChildren<Button>();

    protected virtual void Start() => _buyButton.onClick.AddListener(() => BuyProduct());

    public abstract void BuyProduct();
}
