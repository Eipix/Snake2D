using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavePopup : MonoBehaviour
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private RectTransform _popup;
    [SerializeField] private TextMeshProUGUI _content;
    [SerializeField] private TextMeshProUGUI _goldAppleBalance;
    [SerializeField] private Sprite _skull;
    
    private Dictionary<int, int> _waveRewards = new Dictionary<int, int>();
    private TranslatableString _langWaveComplete = new TranslatableString();

    private void Awake()
    {
        _langWaveComplete.TranslateTexts = new string[]
        {
            "Wave completed: golden apples",
            "Волна завершена: золотые яблоки",
            "Dalga tamamlandı: altın elmalar"
        };

        _waveRewards.Add(0, 3);
        _waveRewards.Add(1, 8);
        _waveRewards.Add(2, 20);
        _waveRewards.Add(3, 50);
        _waveRewards.Add(4, 100);
    }

    public void GetReward(int index)
    {
        _additionApples.AddGoldApple(_waveRewards[index]);
        string text = ExtractTranslatedText(index);
        SideNotification.Instance.Show(text, _skull);
    }

    public string ExtractTranslatedText(int index)
    {
        string[] words = _langWaveComplete.Translate.Split(" ");
        return $"{words[0]} {index + 1} {words[1]}\n<color=yellow>{words[2]} {words[3]} x{_waveRewards[index]}</color>";
    }
}
