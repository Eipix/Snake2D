using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementComplete : MonoBehaviour
{
    [SerializeField] private Image _blackout;
    [SerializeField] private Image _checkMark;

    private Sequence _sequence;

    private void OnDisable()
    {
        if (_sequence != null && _sequence.IsActive())
            _sequence.Complete(true);
    }

    public Sequence PlayOneShot()
    {
        _sequence = DOTween.Sequence().SetUpdate(true)
            .OnStart(() =>
            {
                _blackout.gameObject.SetActive(true);
                _checkMark.rectTransform.localScale = Vector3.one * 3;
            })
            .Append(_blackout.DOFade(1f, 1f))
            .Join(_checkMark.rectTransform.DOScale(Vector3.one, .3f))
            .Insert(.3f, _checkMark.rectTransform.DOPunchScale(Vector3.one * 0.5f, .3f));

            return _sequence;
    }

    public void SetCompleted(RectTransform view)
    {
        _blackout.gameObject.SetActive(true);
        view.SetAsLastSibling();
    }
}
