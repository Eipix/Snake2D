using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkinFilter : MonoBehaviour
{
    [SerializeField] private SpawnSkins skinsTab;
    [SerializeField] private Image _content;
    [SerializeField] private RectTransform _arrow;
    [SerializeField] private Image[] outlines;

    private Dictionary<int, Action> _filters = new Dictionary<int, Action>();
    
    private void OnEnable()
    {
        if (IsContentEnable())
            ChangeContentState(0);
    }

    private void Start()
    {
        _filters.Add(0, () => skinsTab.SpawnFilter(skinsTab.PrefabSkins.Where(skin => skin.UnlockState && skin.Rareness == Rarity.Common), true));
        _filters.Add(1, () => skinsTab.SpawnFilter(skinsTab.PrefabSkins.Where(skin => skin.UnlockState && skin.Rareness == Rarity.Epic), true));
        _filters.Add(2, () => skinsTab.SpawnFilter(skinsTab.PrefabSkins.Where(skin => skin.UnlockState && skin.Rareness == Rarity.Legendary), true));
        _filters.Add(3, () => skinsTab.SpawnFilter(skinsTab.PrefabSkins.Where(skin => skin.UnlockState).OrderBy(skin => skin.Rareness), true));
    }

    public void OnFilterClick()
    {
        ChangeContentState();
    }

    private void ChangeContentState(float speed = 0.2f)
    {
        float fillAmount = IsContentEnable() ? 0f : 1f;
        int arrowAngle = IsContentEnable() ? 0 : -90;

        Vector3 newRotation = new Vector3(_arrow.rotation.x, _arrow.rotation.y, _arrow.rotation.z + arrowAngle);
        
        DOTween.Sequence()
               .Append(_arrow.DORotate(newRotation, speed))
               .Append(_content.DOFillAmount(fillAmount, speed));
    }

    public void OnOptionClick(int option)
    {
        DisableOutlines();
        outlines[option].enabled = true;

        if (_filters.ContainsKey(option))
            _filters[option].Invoke();

        ChangeContentState();
    }

    private bool IsContentEnable() => _content.fillAmount == 1 ? true : false;

    private void DisableOutlines()
    {
        foreach (var outline in outlines)
            outline.enabled = false;
    }
}
