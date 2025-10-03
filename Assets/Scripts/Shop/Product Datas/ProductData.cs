using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Product", menuName = "Purchases/Product")]
public class ProductData : SerializedScriptableObject
{
    [OdinSerialize] private (IProduct product,int count)[] _items;

    public (IProduct product, int count)[] Items => _items;

    public void Add()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].product.Receive(Items[i].count);
        }
        SaveSerial.Instance.SaveGame();
    }
}
