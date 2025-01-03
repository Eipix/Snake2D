using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionsForLevel : MonoBehaviour
{
    [SerializeField] private OnPauseOrWinOrDefeatPanelShow _panel;
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Health _health;
    [SerializeField] private SnakeCollision _leadingTrigger;
    [SerializeField] private SnakeMovement _snake;
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private Effects _effects;
    [SerializeField] private SkinLoader _skinLoader;

    [SerializeField] private Image[] _currentStars;
    [SerializeField] private Sprite _filledStar;

    private Dictionary<LevelConditions, int> _conditionValues;

    public int  DistractHedgehogs { get;  set; }
    public int KillAnts { get;  set; }
    public int FreezeHedgehogs { get;  set; }

    private bool[] _stars = new bool[3];
    private float _timer;

    private void Start()
    {
        _conditionValues = new Dictionary<LevelConditions, int>()
        {
            { LevelConditions.CollectApples, _additionApples.NominallyCollected },
            { LevelConditions.CollectGoldApples, _additionApples.ActuallyGoldAppleCollected },
            { LevelConditions.LoseHealthNoMoreThan, _health.TotalLostHealthCount },
            { LevelConditions.AvoidHittingRocks, _leadingTrigger.StoneCollisionCount },
            { LevelConditions.PassDistance, _snake.DistanceTraveled },
            { LevelConditions.NoBomb, _effects.BombUsedCount},
            { LevelConditions.DistractHedgehogs, DistractHedgehogs},
            { LevelConditions.KillAnts, KillAnts},
            { LevelConditions.FreezeHedgehogs, FreezeHedgehogs},
            { LevelConditions.HoldOutSeconds, (int)_timer},
            { LevelConditions.PassLevelByEpicSkin,  Convert.ToInt32(_skinLoader.IsSkinEpic)}
        };
    }

    private void Update()
    {
        if (Main().Name != LevelConditions.HoldOutSeconds)
            return;

        if (IsMainReached())
        {
            _panel.WinShow();
        }

        _timer += Time.deltaTime;
    }

    public void StarsCheck()
    {
        UpdateValues();

        for (int i = 0; i < _saveSerial.Data.Conditions.Length; i++)
        {
            _stars[i] = _saveSerial.Data.Conditions[i].IsComplete(_conditionValues[_saveSerial.Data.Conditions[i].Name]);
            _currentStars[i].sprite = _stars[i] 
                ? _filledStar 
                : _currentStars[i].sprite;
        }

        _saveSerial.SaveStars(_stars);
    }

    public Condition Main() => _saveSerial.Data.Conditions[0];

    public bool IsMainReached()
    {
        UpdateValues();
        return Main().IsComplete(_conditionValues[Main().Name]);
    }

    private void UpdateValues()
    {
        _conditionValues[LevelConditions.CollectApples] = _additionApples.NominallyCollected;
        _conditionValues[LevelConditions.CollectGoldApples] = _additionApples.ActuallyGoldAppleCollected;
        _conditionValues[LevelConditions.LoseHealthNoMoreThan] = _health.TotalLostHealthCount;
        _conditionValues[LevelConditions.AvoidHittingRocks] = _leadingTrigger.StoneCollisionCount;
        _conditionValues[LevelConditions.PassDistance] = _snake.DistanceTraveled;
        _conditionValues[LevelConditions.NoBomb] = _effects.BombUsedCount;
        _conditionValues[LevelConditions.DistractHedgehogs] = DistractHedgehogs;
        _conditionValues[LevelConditions.KillAnts] = KillAnts;
        _conditionValues[LevelConditions.FreezeHedgehogs] = FreezeHedgehogs;
        _conditionValues[LevelConditions.HoldOutSeconds] = (int)_timer;
        _conditionValues[LevelConditions.PassLevelByEpicSkin] = Convert.ToInt32(_skinLoader.IsSkinEpic);
    }
}
