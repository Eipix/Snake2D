using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SkinLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinReferences _references;
    [SerializeField] private PoolObjects _pool;
    [SerializeField] private Skin[] _skinPrefabs;
    [SerializeField] private SaveSerial _saveSerial;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer[] currentSkinParts;
    [SerializeField] private SpriteRenderer _bodyDefaultInScene;
    [SerializeField] private Image _avatar;

    public Skin[] Prefabs => _skinPrefabs;
    public Skin CurrentSkin { get; private set; }

    public bool IsSkinEpic { get; private set; }

    private void Start()
    {
        SetSkin();
        CurrentSkin = GetCurrentSkin();
        CurrentSkin.Init(_references);
        IsSkinEpic = CurrentSkin.Rareness == Rarity.Epic;
    }

    private Skin GetCurrentSkin()
    {
        Skin currentSkin = _skinPrefabs
            .Where(prefab => prefab.GetType().ToString() == _saveSerial.LoadCurrentSkinType())
            .First();

        var skinInScene = Instantiate(currentSkin.gameObject, transform);
        skinInScene.GetComponent<ClickBehaviour>().enabled = false;
        var skin = skinInScene.GetComponent<Skin>();

        return skin;
    }

    private void SetSkin()
    {
        foreach (var skin in _skinPrefabs)
        {
            if (_saveSerial.LoadCurrentSkinType() == skin.GetType().ToString())
            {
                skin.LoadSkin(ref currentSkinParts, ref _avatar, ref _bodyDefaultInScene);
            }
        }
        List<Body> bodysInPool = _pool.GetBodysByType(currentSkinParts[1].GetComponent<Body>());
        List<Body> leftBodysInPool = _pool.GetBodysByType(currentSkinParts[2].GetComponent<Body>());
        List<Body> RightbodysInPool = _pool.GetBodysByType(currentSkinParts[3].GetComponent<Body>());

        SetSpriteByType(bodysInPool, currentSkinParts[1].sprite);
        SetSpriteByType(leftBodysInPool, currentSkinParts[2].sprite);
        SetSpriteByType(RightbodysInPool, currentSkinParts[3].sprite);
    }

    private void SetSpriteByType(List<Body> list, Sprite sprite)
    {
        foreach (var body in list)
        {
            body.GetComponent<SpriteRenderer>().sprite = sprite;
        }
    }
}
