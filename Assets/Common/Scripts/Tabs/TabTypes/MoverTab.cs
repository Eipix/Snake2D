using UnityEngine;
using DG.Tweening;

public class MoverTab : Tab
{
    [SerializeField] private Vector2 _switchPosition;

    private RectTransform _scrollContent;
    private Vector2 _boundary;
    public readonly float Offset = 900f;

    protected override void Awake()
    {
        base.Awake();
        _scrollContent = Content.parent as RectTransform;
    }

    public override void On()
    {
        base.On();
        var targetPosX = ConvertToParent(Content.anchoredPosition.x);
        _scrollContent.DOAnchorPosX(targetPosX, 0.3f).SetEase(Ease.Linear).SetUpdate(true);
    }

    public void CheckSwitch()
    {
        var leftEdge = Content.anchoredPosition.x - Content.sizeDelta.x / 2;
        var rightEdge = Content.anchoredPosition.x + Content.sizeDelta.x / 2;
        _boundary.Set(ConvertToParent(leftEdge), ConvertToParent(rightEdge));

        if (_scrollContent.anchoredPosition.x < _boundary.x && _scrollContent.anchoredPosition.x > _boundary.y)
            base.On();
    }

    private float ConvertToParent(float positionX) => -(positionX - Offset);
}
