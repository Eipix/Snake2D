using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeTransition : MonoBehaviour
{
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    public void Open(GameObject panel) => Play(panel, true);

    public void Close(GameObject panel) => Play(panel, false);

    private void Play(GameObject panel, bool active)
    {
        DOTween.Sequence()
            .AppendCallback(() => gameObject.SetActive(true))
            .Append(_image.DOFade(1f, 0.3f))
            .AppendCallback(() => panel.SetActive(active))
            .Append(_image.DOFade(0f, 0.3f))
            .AppendCallback(() => gameObject.SetActive(false))
            .WaitForCompletion();
    }
}
