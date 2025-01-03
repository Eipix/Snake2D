using UnityEngine;

public class UpgradeTab : MonoBehaviour
{
    [SerializeField] private SpawnSkins _skinTab;
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private Upgrade _skinSkill;

    private void OnEnable()
    {
        SetSnakeArt();
        _skinSkill.gameObject.SetActive(true);
    }

    private void SetSnakeArt()
    {
        foreach (var skin in _skinTab.PrefabSkins)
        {
            if(skin.GetType().ToString() == _saveSerial.LoadCurrentSkinType())
            {
                _skinTab.InventoryArt.ChangeArt(skin);
            }
        }
    }
}
