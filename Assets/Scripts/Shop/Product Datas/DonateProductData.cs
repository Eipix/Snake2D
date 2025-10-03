using UnityEngine;

[CreateAssetMenu(fileName = "Donate Product", menuName = "Purchases/Donate Product")]
public class DonateProductData : ProductData
{
    [SerializeField] private Goods _type;

    public Goods Type => _type;
}
