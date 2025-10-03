using System.Collections;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class BannerSwiper : SerializedMonoBehaviour
{
    [SerializeField] private BannerTab _default;
    [SerializeField] private Summoner _summoner;
    [SerializeField] private Translatable<Sprite>[] _langBanner;

    [SerializeField] private GameObject _summons;
    [SerializeField] private SwapCircle[] _circles;
    [SerializeField] private Banner _current;
    [SerializeField] private Banner _next;
    [SerializeField] private RectTransform _tabParent;

    private BannerTab _activeTab;
    private SwapCircle _activeCircle;

    private Sequence _swap;
    private Sequence _autoSwap;

    private int _swapIndex = 0;

    private void Awake()
    {
        _activeTab = _tabParent.GetComponentInChildren<BannerTab>();
        _activeCircle = _circles[0].Init().On();
    }

    private void Start() => _default.Set();

    private void OnEnable()
    {
        _current.Image.sprite = _langBanner[_swapIndex].Translate;
        _autoSwap = DOTween.Sequence()
            .OnStart(() => StartOver())
            .AppendInterval(5f)
            .AppendCallback(() => StartCoroutine(SwipeLeft()))
            .SetLoops(-1);
    }

    private void OnDisable()
    {
        _autoSwap.Kill();
        StopAllCoroutines();
    }

    public void ChangeBannerData(BannerTab nextTab)
    {
        if (nextTab == _activeTab)
            return;

        if (_swap.IsActive())
            _swap.Complete(true);

        _summons.SetActive(false);
        _summons = nextTab.Summons;

        _langBanner = nextTab.LangBanners;
        _current.Image.sprite = _langBanner[0].Translate;

        _activeTab?.FadeZero();
        _activeTab = nextTab;

        _summoner.Data = nextTab.Data;

        StartOver();
    }

    public void OnRightArrowClick()
    {
        StartCoroutine(SwipeLeft());
    }

    public void OnLeftArrowClick()
    {
        StartCoroutine(SwipeRight());
    }

    public IEnumerator SwipeLeft(float time = 0.3f)
    {
        if (!_swap.IsActive())
        {
            _swapIndex++;
            if (_swapIndex >= _langBanner.Length)
                _swapIndex = 0;

            yield return Move(_current.SizeX, time);
        }
    }

    public IEnumerator SwipeRight(float time = 0.3f)
    {
        if (!_swap.IsActive())
        {
            _swapIndex--;
            if (_swapIndex < 0)
                _swapIndex = _langBanner.Length - 1;

            yield return Move(-_current.SizeX, time);
        }
    }

    private YieldInstruction Move(float startPosX, float time = 0.3f)
    {
        NextCircle();

        _next.Image.sprite = _langBanner[_swapIndex].Translate;
        _next.RectTransform.anchoredPosition = new Vector2(startPosX, _current.RectTransform.anchoredPosition.y);

        _swap = DOTween.Sequence()
                       .Append(_current.RectTransform.DOAnchorPosX(-startPosX, time))
                       .Join(_next.RectTransform.DOAnchorPosX(_current.PosX, time))
                       .AppendCallback(() => (_current, _next) = (_next, _current));

        return _swap.WaitForCompletion();
    }

    private void StartOver()
    {
        if (_swapIndex <= 0)
            return;

        _swapIndex = -1;
        StartCoroutine(SwipeLeft(0f));
    }

    private void NextCircle()
    {
        _activeCircle.Off();
        _activeCircle = _circles[_swapIndex];
        _activeCircle.On();
    }
}
