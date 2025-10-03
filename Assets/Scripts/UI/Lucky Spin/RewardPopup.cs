using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class RewardPopup : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private Image _reward;
    [SerializeField] private Image _popup;
    [SerializeField] private Image _outLight;

    public readonly float MaxFade = 0.85f;

    private void Start()
    {
        _popup.color = new Color(0, 0, 0, MaxFade);
        _reward.color = new Color(1, 1, 1, 0);
        _title.color = new Color(1, 1, 1, 0);
        _outLight.color = new Color(1, 1, 1, 0);
    }

    public YieldInstruction Show(float duration)
    {
        _popup.gameObject.SetActive(true);
        Sequence sequence = DOTween.Sequence()
            .Append(_popup.DOFade(MaxFade, 1f))
            .Join(_title.DOFade(1f, 1f))
            .Join(_description.DOFade(1f, 1f))
            .Join(_reward.DOFade(1f, 1f))
            .Join(_outLight.DOFade(1f, 1f));

        return sequence.WaitForCompletion();
    }

    public IEnumerator Close()
    {
        Sequence sequence = DOTween.Sequence()
            .Append(_popup.DOFade(0f, 1f))
            .Join(_title.DOFade(0f, 1f))
            .Join(_description.DOFade(0f, 1f))
            .Join(_reward.DOFade(0f, 1f))
            .Join(_outLight.DOFade(0f, 1f));

        yield return sequence.WaitForCompletion();
        _popup.gameObject.SetActive(false);
    }

    public void ChangeValues(Sprite sprite, string description, Color color)
    {
        _description.text = description;
        _description.color = color;
        _reward.sprite = sprite;
        _reward.SetNativeSize();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StartCoroutine(Close());
    }

    public void OnPointerDown(PointerEventData eventData) { }
}
