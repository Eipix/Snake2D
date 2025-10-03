using UnityEngine;
using DG.Tweening;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform _fill;
    [SerializeField] private Vector2 _range;

    private Tween _filling;

    public void Fill(Vector2 range, float currentValue)
    {
        float value = GetValue(range, currentValue);
        _fill.anchoredPosition = new Vector2(0, value);
    }

    public Tween DOFill(Vector2 range, float currentValue, float duration)
    {
        if (_filling != null && _filling.IsActive())
            _filling.Complete();

        float value = GetValue(range, currentValue);
        _filling = _fill.DOAnchorPosY(value, duration);
        return _filling;
    }

    public void SetMax() => _fill.anchoredPosition = new Vector2(0, _range.y);

    public void SetMix() => _fill.anchoredPosition = new Vector2(0, _range.x);

    private float GetValue(Vector2 range, float currentValue)
    {
        return _range.x + (_range.y - _range.x) * ((currentValue - range.x) /
                                                    (range.y - range.x));
    }
}
