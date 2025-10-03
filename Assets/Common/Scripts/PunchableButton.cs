using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PunchableButton : UIMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Range(0, 1)]
    [SerializeField] private float _punchStrength;
    [Range(0, 1)]
    [SerializeField] private float _time;

    private Tween _tween;
    private Vector2 _defaultScale;

    protected override void Awake()
    {
        base.Awake();
        _defaultScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData) => ScaleDown();

    public void OnPointerUp(PointerEventData eventData) => ScaleUp();

    private void ScaleDown()
    {
        _tween.CompleteIfActive(true);
        _tween = rectTransform.DOScale(_defaultScale * (1 - _punchStrength), _time).SetUpdate(true)
            .OnPlay(() => SoundsPlayer.Instance.PlayOneShotSound(SoundsPlayer.Instance.Button));
    }

    private void ScaleUp()
    {
        _tween.CompleteIfActive(true);
        _tween = rectTransform.DOScale(_defaultScale, _time).SetUpdate(true);
    }
}
