using System;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(NewWaveAnimation))]
[RequireComponent(typeof(WaveTimer))]
[RequireComponent(typeof(WavePopup))]
public class WaveChanger : MonoBehaviour
{
    [SerializeField] private CountdownToStart _countdown;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private TextMeshProUGUI _number;
    [SerializeField] private int _waveDuration;

    public UnityAction<int> OnWaveChanged;

    private NewWaveAnimation _newWaveAnimation;
    private WaveTimer _timer;
    private WavePopup _popup; 

    private int _waveIndex = 0;

    private void Awake()
    {
        _number.text = "1";
        _newWaveAnimation = GetComponent<NewWaveAnimation>();
        _timer = GetComponent<WaveTimer>();
        _popup = GetComponent<WavePopup>();
    }

    private void OnEnable()
    {
        OnWaveChanged += _popup.GetReward;
        ChangeWave(_waveIndex);
        Debug.LogWarning("Data was changed by arena mode");
    }

    private void OnDisable() => OnWaveChanged -= _popup.GetReward;

    private void Update()
    {
        if (_countdown.LevelStarted == false)
            return;

        if (WaveComplete())
        {
            OnWaveChanged?.Invoke(_waveIndex);
            ChangeWave(++_waveIndex);
        }

        if (_waveIndex < SaveSerial.Instance.Waves.Length)
            _timer.Remain -= Time.deltaTime;
    }

    private void ChangeWave(int index)
    {
        if (index >= SaveSerial.Instance.Waves.Length)
            return;

        SaveSerial.Instance.Data = SaveSerial.Instance.Waves[index];
        _number.text = (index + 1).ToString();

        _newWaveAnimation.PlayOneShot();
        _enemySpawner.SpawnToTheLimit();
        _timer.Set(new TimeSpan(0, 0, _waveDuration));
    }

    private bool WaveComplete() => _timer.Remain < 0f && _waveIndex < SaveSerial.Instance.Waves.Length;
}
