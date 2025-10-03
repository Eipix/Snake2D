using UnityEngine;
using DG.Tweening;
using TMPro;
using System;
using System.Collections;

public class MainConditionViewer : MonoBehaviour
{
    [SerializeField] private CountdownToStart _countdown;
    [SerializeField] private ConditionsForLevel _conditions;
    [SerializeField] private AdditionApples _appleCounter;

    [SerializeField] private GameObject _apples;
    [SerializeField] private GameObject _timer;
    [SerializeField] private TextMeshProUGUI _viewer;

    private TimeSpan _holdOutTime;
    private LevelConditions _headName;
    private int _targetValue;

    private void Awake()
    {
        if (SaveSerial.Instance.Mode == LevelMode.Arena)
            gameObject.SetActive(false);
    }

    private void Start()
    {
        var head = _conditions.Head();
        _headName = head.Name;
        _targetValue = head.Value;
        StartObserve();
    }

    public void StartObserve()
    {
        if(_headName == LevelConditions.HoldOutSeconds)
        {
            _timer.SetActive(true);
            StartCoroutine(StartTimer());
        }
        else
        {
            _apples.SetActive(true);
            UpdateApple();
            _appleCounter.RedAppleCollected.AddListener(() => UpdateApple());
            _appleCounter.GoldAppleCollected.AddListener(() => UpdateApple());
        }
    }

    public IEnumerator StartTimer()
    {
        UpdateTimer();
        yield return new WaitUntil(() => _countdown.LevelStarted);
        DOTween.Sequence()
            .AppendCallback(() => UpdateTimer())
            .AppendInterval(1f)
            .SetLoops(_targetValue);
    }

    private void UpdateTimer()
    {
        _holdOutTime = new TimeSpan(0, 0, 0, (int)(_targetValue - _conditions.Timer));

        if (_holdOutTime.TotalSeconds <= 30)
            _viewer.text = $": <color=red>{_holdOutTime.ToString(@"mm\:ss")}</color>";
        else
            _viewer.text = $": {_holdOutTime.ToString(@"mm\:ss")}";
    }

    public void UpdateApple()
    {
        int currentValue = _appleCounter.NominallyRedAppleCollected + _appleCounter.ActuallyGoldAppleCollected;
        _viewer.text = $": <color=#BBFF00>{currentValue}</color>/<color=green>{_targetValue}</color>";
    }
}
