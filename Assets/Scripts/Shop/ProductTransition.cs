using System.Collections;
using UnityEngine;

public class ProductTransition : MonoBehaviour
{
    [SerializeField] private string _key;

    private void OnEnable()
    {
        SaveSerial.Instance.DataLoaded += DisableIfOneTimeBought;
        DonatShop.Inited += () => DonatShop.Instance.SnakePack.Bought += DisableIfOneTimeBought;

    }

    private void OnDisable()
    {
        SaveSerial.Instance.DataLoaded -= DisableIfOneTimeBought;
        DonatShop.Inited -= () => DonatShop.Instance.SnakePack.Bought -= DisableIfOneTimeBought;
    }

    public void Goto() => StartCoroutine(GotoRoutine());

    private IEnumerator GotoRoutine()
    {
        DonatShop.Instance.Window.Open();
        yield return new WaitWhile(() => DonatShop.Instance.PacksTab.Image == null);
        DonatShop.Instance.PacksTab.On();
    }

    private void DisableIfOneTimeBought()
    {
        var isBought = SaveSerial.Instance.Load(_key, SaveSerial.JsonPaths.ConsumedProducts, false);
        gameObject.SetActive(isBought == false);
    }
}
