using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;

public class SideNotification : Singleton<SideNotification>
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private RectTransform _notification;

    [SerializeField] private Vector2 _showPosition = new Vector2(0, 300);
    [SerializeField] private Vector2 _hidePosition = new Vector2(-700, 300);

    private Sequence _notify;

    public override void OnInit()
    {
        _notification.gameObject.SetActive(false);
    }

    [Button]
    public void Show(string text, Sprite icon, float showTime = 1.5f)
    {
        StartCoroutine(ShowByQueue(text, icon, showTime));
    }

    private IEnumerator ShowByQueue(string text, Sprite icon, float showTime)
    {
        if (_notify != null && _notify.IsActive())
            yield return new WaitUntil(() => _notify == null || !_notify.IsActive());

        _icon.sprite = icon;
        _icon.SetNativeSize();
        _description.text = text;

        _notify = PlayOneShot(showTime);
    }

    private Sequence PlayOneShot(float showTime)
    {
        return DOTween.Sequence().SetUpdate(true)
            .AppendCallback(() => _notification.gameObject.SetActive(true))
            .Append(_notification.DOAnchorPos(_showPosition, 0.5f))
            .AppendInterval(showTime)
            .Append(_notification.DOAnchorPos(_hidePosition, 0.5f))
            .AppendCallback(() => _notification.gameObject.SetActive(false));
    }
}
