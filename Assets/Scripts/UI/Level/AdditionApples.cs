using System;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class AdditionApples : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private ConditionsForLevel _conditions;
    [SerializeField] private OnPauseOrWinOrDefeatPanelShow _panels;
    [SerializeField] private ParametrAnimation _parametr;

    [Header("Components")]
    [SerializeField] private TMP_Text _appleBar;
    [SerializeField] private TMP_Text _goldAppleBar;
    [SerializeField] private RectTransform _redBar;
    [SerializeField] private RectTransform _goldBar;

    [Header("Apples Mockup")]
    [SerializeField] private Image _redMockup;
    [SerializeField] private Image _goldMockup;
    [SerializeField] private RectTransform _applePointBar;
    [SerializeField] private RectTransform _goldApplePointBar;

    public UnityAction<Type> SkillActivated;

    private System.Random _rand = new System.Random();
    private Sequence _moveToBar;

    public int ActuallyCollected => NominallyCollected + RedAppleDoubleCount;
    public int ActuallyRedAppleCollected => NominallyRedAppleCollected + RedAppleDoubleCount;
    public int NominallyCollected => NominallyRedAppleCollected + ActuallyGoldAppleCollected;
    public int NominallyRedAppleCollected { get; private set; }
    public int ActuallyGoldAppleCollected { get; private set; }
    public int RedAppleDoubleCount { get; private set; }

    private int _apple2xChance = 0;

    private void OnEnable() => SkillActivated += _parametr.IconFlyAway;

    private void OnDisable() => SkillActivated -= _parametr.IconFlyAway;

    private void Start()
    {
        (int redAppleAmount, int goldAppleAmount) = _saveSerial.LoadApple();
        _apple2xChance = 5 * _saveSerial.LoadControl();

        _appleBar.text = redAppleAmount.ToString("D8");
        _goldAppleBar.text = goldAppleAmount.ToString("D8");
    }

    public void AddApple(int count = 1)
    {
        TryDoubleApple(ref count);

        NominallyRedAppleCollected++;
        _appleBar.text = (int.Parse(_appleBar.text) + count).ToString("D8");

        (int redApple, int goldApple) = _saveSerial.LoadApple();
        _saveSerial.SaveApple(redApple + count, goldApple);

        if (AllAppleCollected()) 
            _panels.WinShow();
    }

    public void AddGoldApple(int count = 1)
    {
        ActuallyGoldAppleCollected++;
        _goldAppleBar.text = (int.Parse(_goldAppleBar.text) + count).ToString("D8");

        (int redApple, int goldApple) = _saveSerial.LoadApple();
        _saveSerial.SaveApple(redApple, goldApple + count);

        if (AllAppleCollected())
            _panels.WinShow();
    }

    private void TryDoubleApple(ref int count)
    {
        if (_rand.withProbability(_apple2xChance))
        {
            RedAppleDoubleCount += count;
            count *= 2;
            SkillActivated?.Invoke(typeof(AppleDouble));
            Debug.Log($"Apple was doubled: {count}");
        }
    }

    private bool AllAppleCollected()
    {
        if (_conditions.Main().Name != LevelConditions.CollectApples)
            return false;

        return _conditions.IsMainReached();
    }

    public void MoveToBar(Apple apple)
    {
        if (apple is RottenApple)
            return;

        _moveToBar.Complete();

        var appleToMove = apple.GetType() == typeof(RedApple) ? _redMockup : _goldMockup;
        var bar = apple.GetType() == typeof(RedApple) ? _redBar : _goldBar;
        var target = apple.GetType() == typeof(RedApple) ? _applePointBar : _goldApplePointBar;

        appleToMove.transform.position = apple.gameObject.transform.position;
        appleToMove.transform.gameObject.SetActive(true);

        _moveToBar = DOTween.Sequence()
            .Append(appleToMove.transform.DOMove(target.position, 0.3f))
            .AppendCallback(() => appleToMove.transform.gameObject.SetActive(false))
            .Append(bar.DOPunchScale(Vector3.one * 0.3f, 0.3f));
    }
}
