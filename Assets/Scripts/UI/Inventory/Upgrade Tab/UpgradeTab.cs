using UnityEngine;

public class UpgradeTab : MonoBehaviour
{
    [SerializeField] private SpawnSkins _skinTab;
    [SerializeField] private Upgrade _skinSkill;

    private void OnEnable()
    {
        SetSnakeArt();
        _skinSkill.gameObject.SetActive(true);
    }

    private void SetSnakeArt()
    {
        var prefabs = SaveSerial.Instance.SkinPrefabs;
        foreach (var skin in prefabs)
        {
            if(skin.GetType().ToString() == SaveSerial.Instance.LoadCurrentSkinType())
            {
                _skinTab.InventoryArt.ChangeArt(skin);
            }
        }
    }
}
