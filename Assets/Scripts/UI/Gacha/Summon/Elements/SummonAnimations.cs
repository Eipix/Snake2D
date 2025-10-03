using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class SummonAnimations : SerializedMonoBehaviour
{
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [OdinSerialize] private Dictionary<Rarity, RewardScene> _rewardScenes;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _background;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _skinArt;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _skinMirage;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private TextMeshProUGUI _headline;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private TextMeshProUGUI _rarityHeadline;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _frameHeadline;
    [FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    [SerializeField] private RectTransform _rarityHeadlineTransform;

    [OdinSerialize] private Dictionary<Rarity, RaritySpriteSet> _spriteSets;

    private RewardScene _current;
    private Sequence _sequence;
    
    private readonly Vector2 _headlineTargetPosition = new Vector2(-540, -220);
    private readonly Vector2 _headlineStartPosition = new Vector2(-1400, -40);

    private void Awake() => ElementsDisable();

    public void Init(Sprite reward, AssetText headline, Rarity rarity)
    {
        _skinArt.sprite = reward;
        _skinMirage.sprite = reward;
        _rarityHeadline.text = _spriteSets[rarity].RarityText.Translate;
        _frameHeadline.sprite = _spriteSets[rarity].FrameHeadlines;

        _headline.SetAssetText(headline);
        _skinArt.SetNativeSize();
        _skinMirage.SetNativeSize();
        _rarityHeadline.SetNativeSize();
        _frameHeadline.SetNativeSize();
        SetScene(rarity);
    }

    public YieldInstruction PlayOneShot(IGachaReward reward)
    {
        Init(reward.FullScreenIcon, reward.LangHeadline, reward.Rarity);

        _sequence.CompleteIfActive();

        _sequence = DOTween.Sequence()
            .OnStart(() => Prepare())
            .Append(_skinMirage.rectTransform.DOScale(Vector2.one, 0.3f))
            .Join(_skinMirage.DOFade(0f, 0.3f))
            .Join(_rarityHeadlineTransform.DOAnchorPos(_headlineTargetPosition, 0.6f).SetEase(Ease.OutExpo));

        return _sequence.WaitForCompletion();
    }

    public void ElementsDisable()
    {
        _rarityHeadlineTransform.anchoredPosition = _headlineStartPosition;

        MaskableGraphic[] graphics = { _frameHeadline, _rarityHeadline, _headline, _skinMirage, _skinArt };
        graphics.FadeAll(0f);
        OffScene();
    }

    private void SetScene(Rarity reward)
    {
        _current = _rewardScenes[reward];
        _background.sprite = _current.Background;
        _background.gameObject.SetActive(true);
        _current.Particle.gameObject.SetActive(true);
    }

    private void OffScene()
    {
        if (_current == null)
            return;

        _background.gameObject.SetActive(false);
        _current.Particle.gameObject.SetActive(false);
    }

    private void Prepare()
    {
        _rarityHeadlineTransform.anchoredPosition = _headlineStartPosition;
        _skinMirage.rectTransform.localScale = Vector2.one * 3;

        MaskableGraphic[] graphics = { _frameHeadline, _rarityHeadline, _headline, _skinMirage, _skinArt };
        graphics.FadeAll(1f);
    }

    public class RewardScene
    {
        [OdinSerialize] public Sprite Background { get; private set; }
        [OdinSerialize] public ParticleSystem Particle { get; private set; }
    }

    public class RaritySpriteSet
    {
        [ShowInInspector, OdinSerialize] public Sprite FrameHeadlines { get; private set; }
        [OdinSerialize] public TranslatableString RarityText { get; private set; }
    }
}
