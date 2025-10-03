using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SkinLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SkinReferences _references;
    [SerializeField] private PoolObjects _pool;

    [Header("Sprites")]
    [SerializeField] private SpriteRenderer[] currentSkinParts;
    [SerializeField] private SpriteRenderer _bodyDefaultInScene;
    [SerializeField] private Image _avatar;

    private Skin[] _prefabs;
    public Skin CurrentSkin { get; private set; }

    private void Awake() => _prefabs = SaveSerial.Instance.SkinPrefabs;

    private void Start()
    {
        SetSkin();
        CurrentSkin = GetCurrentSkin();
        CurrentSkin.Init(_references);
        Debug.Log($"Skin type: {SaveSerial.Instance.LoadCurrentSkinType()}");
    }

    private Skin GetCurrentSkin()
    {
        Skin currentSkin = _prefabs
            .Where(prefab => prefab.GetType().ToString() == SaveSerial.Instance.LoadCurrentSkinType())
            .FirstOrDefault();

        if (currentSkin == null)
            currentSkin = _prefabs
                .Where(skin => skin.GetType() == typeof(Pagko))
                .FirstOrDefault();

        var skinInScene = Instantiate(currentSkin.gameObject, transform);
        skinInScene.GetComponent<ClickBehaviour>().enabled = false;
        var skin = skinInScene.GetComponent<Skin>();

        return skin;
    }

    private void SetSkin()
    {
        foreach (var skin in _prefabs)
        {
            if (SaveSerial.Instance.LoadCurrentSkinType() == skin.GetType().ToString())
            {
                skin.LoadSkin(ref currentSkinParts, ref _avatar, ref _bodyDefaultInScene);
                break;
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
