using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlackoutAnimation : MonoBehaviour
{
    private Image _blackout;
    private Sequence _sequence;

    private void Awake() => _blackout = GetComponent<Image>();

    public YieldInstruction PlayOneShot(TweenCallback onFade = null, TweenCallback onComplete = null, bool withAppear = false)
    {
        gameObject.SetActive(true);
        _blackout.color = _blackout.color.Fade(0f);
        _sequence = DOTween.Sequence();

        _sequence.Append(_blackout.DOFade(1f, 1f));
        _sequence.AppendCallback(onFade);
        if (withAppear) _sequence.Append(_blackout.DOFade(0f, 1f));
        _sequence.OnComplete(onComplete);

        return _sequence.WaitForCompletion();
    }
}
