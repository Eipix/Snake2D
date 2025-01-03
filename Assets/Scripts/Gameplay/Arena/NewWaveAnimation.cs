using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewWaveAnimation : MonoBehaviour
{
    [SerializeField] private Image _eyesLight;
    [SerializeField] private Image _title;
    [SerializeField] private Image _number;

    public Sequence PlayOneShot()
    {
        return DOTween.Sequence()
               .OnStart(() => _eyesLight.color = _eyesLight.color.Invisible())
               .Append(_eyesLight.DOFade(1f, 0.25f))
               .Join(_title.transform.DOPunchScale(Vector2.one * 0.5f, 0.3f))
               .Join(_number.transform.DOPunchScale(Vector2.one, 0.3f))
               .Append(_eyesLight.DOFade(0f, 0.25f));
    }
}
