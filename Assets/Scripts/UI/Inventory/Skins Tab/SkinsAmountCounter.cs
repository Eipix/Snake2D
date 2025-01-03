using System.Linq;
using UnityEngine;
using TMPro;

public class SkinsAmountCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text _availableSkins;
    [SerializeField] private TMP_Text _totalSkins;
    [SerializeField] private SpawnSkins _skinTab;

    private void OnEnable()
    {
        SetCounterValue();    
    }

    private void SetCounterValue()
    {
        var availableSkins = _skinTab.PrefabSkins.Where(skin => skin.UnlockState);
        _availableSkins.text = availableSkins.Count().ToString();
        _totalSkins.text = _skinTab.PrefabSkins.Length.ToString();
    }
}
