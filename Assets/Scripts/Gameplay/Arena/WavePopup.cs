using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WavePopup : MonoBehaviour
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private RectTransform _popup;
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _goldAppleBalance;
    
    private Dictionary<int, int> _waveRewards = new Dictionary<int, int>();
    private Sequence _showPopup;

    private Vector2 _showPosition = new Vector2(-706, 700);
    private Vector2 _hidePosition = new Vector2(-1140, 700);

    private void Awake()
    {
        _waveRewards.Add(0, 3);
        _waveRewards.Add(1, 8);
        _waveRewards.Add(2, 20);
        _waveRewards.Add(3, 50);
        _waveRewards.Add(4, 100);
    }

    public void GetReward(int index)
    {
        _showPopup = DOTween.Sequence();
        _showPopup.OnStart(() =>
        {
            _additionApples.AddGoldApple(_waveRewards[index]);
            _popup.anchoredPosition = _hidePosition;
            _content.text = $"Wave {index + 1} complete:\n<color=yellow>gold apple x{_waveRewards[index]}</color>";
        });
        _showPopup.Append(_popup.DOAnchorPos(_showPosition, 0.5f));
        _showPopup.AppendInterval(1.5f);
        _showPopup.Append(_popup.DOAnchorPos(_hidePosition, 0.5f));
    }
}
