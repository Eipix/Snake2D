using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class ArrowAnimation : MonoBehaviour
{
    [SerializeField] private float _offset;

    private RectTransform _rectTransform;

    private float _startPosX;
    private float _targetPosX;

    private void Awake()
    {
        _rectTransform = transform as RectTransform;

        _startPosX = _rectTransform.anchoredPosition.x;
        _targetPosX = _startPosX + _offset;
    }

    private void Start()
    {
        DOTween.Sequence()
            .Append(_rectTransform.DOAnchorPosX(_targetPosX, 1f))
            .Append(_rectTransform.DOAnchorPosX(_startPosX, 1f))
            .SetLoops(-1);
    }
}
