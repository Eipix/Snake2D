using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(NewWaveAnimation))]
[RequireComponent(typeof(WaveTimer))]
[RequireComponent(typeof(WavePopup))]
public class WaveChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] _waveNumbers;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private Image _number;
    [SerializeField] private int _waveDuration;

    public UnityAction<int> OnWaveChanged;

    private NewWaveAnimation _newWaveAnimation;
    private WaveTimer _timer;
    private WavePopup _popup; 

    private int _waveIndex = 0;

    private void Awake()
    {
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

    private void OnDisable()
    {
        OnWaveChanged -= _popup.GetReward;
    }

    private void Update()
    {
        if(_timer.Remain < 0f && _waveIndex < _saveSerial.Waves.Length)
        {
            OnWaveChanged?.Invoke(_waveIndex);
            ChangeWave(++_waveIndex);
        }

        if (_waveIndex < _saveSerial.Waves.Length)
            _timer.Remain -= Time.deltaTime;
    }

    private void ChangeWave(int index)
    {
        if (index >= _saveSerial.Waves.Length)
            return;

        _saveSerial.Data = _saveSerial.Waves[index];
        _number.sprite = _waveNumbers[index];
        _number.SetNativeSize();

        _newWaveAnimation.PlayOneShot();
        _enemySpawner.SpawnToTheLimit();
        _timer.Set(new TimeSpan(0, 0, _waveDuration));
    }
}
