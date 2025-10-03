using System.Linq;
using UnityEngine;
using TMPro;

public class SkinsAmountCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _availableSkins;
    [SerializeField] private TMP_Text _totalSkins;

    private void OnEnable()
    {
        SetCounterValue();    
    }

    private void SetCounterValue()
    {
        var skins = SaveSerial.Instance.SkinPrefabs;
        var availableSkins = skins.Where(skin => skin.UnlockState);
        _availableSkins.text = availableSkins.Count().ToString();
        _totalSkins.text = skins.Length.ToString();
    }
}
