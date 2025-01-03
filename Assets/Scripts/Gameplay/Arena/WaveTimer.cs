using System;
using UnityEngine;
using TMPro;

public class WaveTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _time;
    public float Remain { get; set; }

    private void Update()
    {
        _time.text = new TimeSpan(0,0, (int)Remain).ToString(@"mm\:ss");
    }

    public void Set(TimeSpan startTime)
    {
        Remain = (float)startTime.TotalSeconds;
        _time.text = startTime.ToString(@"mm\:ss");
    }
}
