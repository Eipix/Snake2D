using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnSkins : MonoBehaviour
{
    [field:SerializeField] public InventoryArt InventoryArt { get; private set; }
    [field:SerializeField] public Skin[] PrefabSkins { get; private set; }

    [SerializeField] private RectTransform _content;
    [SerializeField] private SaveSerial _saveSerial;

    private List<Skin> _skins = new List<Skin>();

    private void OnEnable()
    {
        SetSelectedInArt();
        UpdateTab();
    }

    private void ClearTab()
    {
        if (_skins.Count > 0)
        {
            foreach (var skin in _skins)
                Destroy(skin.gameObject);
        }
        _skins.Clear();
    }

    private void UpdateTab()
    {
        SpawnFilter(PrefabSkins.Where(skin => skin.UnlockState).OrderBy(skin => skin.Rareness), true);
        SpawnFilter(PrefabSkins.Where(skin => !skin.UnlockState), false);
    }

    public void SpawnFilter(IEnumerable<Skin> filteredSkins, bool clearTab)
    {
        if (clearTab) ClearTab();

        foreach (var skin in filteredSkins)
        {
            var newSkin = Instantiate(skin.gameObject, _content.transform);
            _skins.Add(newSkin.GetComponent<Skin>());
        }
    }

    private void SetSelectedInArt()
    {
        if (gameObject.activeSelf)
        {
            foreach (var skin in PrefabSkins)
            {
                if (skin.GetType().ToString() == _saveSerial.LoadCurrentSkinType())
                    InventoryArt.ChangeArt(skin);
            }
        }
    }

    public void SetUnclickableState()
    {
        foreach (var skin in _skins)
        {
            skin.GetComponent<ClickBehaviour>().ShadowDisable();
        }
    }

    public void SetUnselectedState()
    {
        foreach (var skin in _skins)
        {
            skin.GetComponent<ClickBehaviour>().SelectedDisable();
        }
    }
}
