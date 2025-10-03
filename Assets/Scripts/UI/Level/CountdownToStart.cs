using UnityEngine;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.Events;

public class CountdownToStart : MonoBehaviour
{
    [SerializeField] private SnakeMovement _movement;
    [SerializeField] private GameObject _countdown;

    public UnityEvent Loaded;
    private TMP_Text _countdownText;

    private float _speed;

    public bool LevelStarted { get; private set; }

    private void Start()
    {
        Loaded?.Invoke();
        _countdownText = _countdown.GetComponentInChildren<TMP_Text>();
        _speed = _movement.Speed;
        _movement.ChangeSpeed(-_speed);
    }

    public IEnumerator CountdownStart()
    {
        _countdown.SetActive(true);
        yield return _countdownText.DOText("3", 1f).SetUpdate(false).WaitForCompletion();
        yield return _countdownText.DOText("2", 1f).SetUpdate(false).WaitForCompletion();
        yield return _countdownText.DOText("1", 1f).SetUpdate(false).WaitForCompletion();
        _countdown.SetActive(false);
        _movement.ChangeSpeed(_speed);
        LevelStarted = true;
    }

    public void OnPressStartClick()
    {
        StartCoroutine(CountdownStart());
    }
}
