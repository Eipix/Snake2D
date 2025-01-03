using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Notification : MonoBehaviour
{
    [SerializeField] private Image _notify;
    private TMP_Text _text;

    private void Start()
    {
        _text = _notify.GetComponentInChildren<TMP_Text>();
        _notify.gameObject.SetActive(false);
    }

    public void Notify(string text)
    {
        if (!_notify.gameObject.activeSelf)
        {
            _text.text = text;
            _notify.gameObject.SetActive(true);
            (_notify.color, _text.color) = (_notify.color.Invisible(), _notify.color.Invisible());

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_notify.DOFade(1f, 0.5f));
            sequence.Join(_text.DOFade(1f, 0.5f));
            sequence.AppendInterval(0.5f);
            sequence.Append(_notify.DOFade(0f, 1f));
            sequence.Join(_text.DOFade(0f, 1f));

            sequence.AppendCallback(() => _notify.gameObject.SetActive(false));
        }
    }
}
