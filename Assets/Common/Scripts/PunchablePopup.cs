using UnityEngine;
using DG.Tweening;

public class PunchablePopup : MonoBehaviour
{
    [SerializeField] private GameObject _blackout;
    [SerializeField] private RectTransform rectTransform;

    private Sequence _popupScaling;

    public void Open()
    {
        _popupScaling.CompleteIfActive(true);
        _popupScaling = DOTween.Sequence().SetUpdate(true)
            .AppendCallback(() =>
            {
                _blackout.SetActive(true);
                gameObject.SetActive(true);
                rectTransform.localScale = Vector2.one;
            })
            .Append(rectTransform.DOPunchScale(Vector2.one * 0.2f, 0.3f));
    }

    public void Close()
    {
        _popupScaling.CompleteIfActive(true);
        _popupScaling = DOTween.Sequence().SetUpdate(true)
        .Append(rectTransform.DOScale(Vector3.one * 1.2f, 0.1f))
        .Append(rectTransform.DOScale(Vector2.zero, 0.2f))
        .AppendCallback(() =>
        {
            _blackout.SetActive(false);
            gameObject.SetActive(false);
        });
    }

    public void CloseImmediately()
    {
        _popupScaling.CompleteIfActive(true);
        rectTransform.localScale = Vector2.zero;
        _blackout.SetActive(false);
        gameObject.SetActive(false);
    }
}
