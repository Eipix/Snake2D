using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnSkins : MonoBehaviour
{
    [field:SerializeField] public InventoryArt InventoryArt { get; private set; }

    [SerializeField] private RectTransform _content;

    private List<Skin> _skins = new List<Skin>();
    private Skin[] _prefabs;

    private void Awake() => _prefabs = SaveSerial.Instance.SkinPrefabs;

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
        SpawnFilter(_prefabs.Where(skin => skin.UnlockState).OrderBy(skin => skin.Rareness), true);
        SpawnFilter(_prefabs.Where(skin => !skin.UnlockState), false);
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
            foreach (var skin in _prefabs)
            {
                if (skin.GetType().ToString() == SaveSerial.Instance.LoadCurrentSkinType())
                    InventoryArt.ChangeArt(skin);
            }
        }
    }

    public void SetUnclickableAll()
    {
        foreach (var skin in _skins)
        {
            skin.ClickBehaviour.ShadowDisable();
        }
    }

    public void SetUnselectedAll()
    {
        foreach (var skin in _skins)
        {
            skin.ClickBehaviour.SelectedDisable();
        }
    }
}
