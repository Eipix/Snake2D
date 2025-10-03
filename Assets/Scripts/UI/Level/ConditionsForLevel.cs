using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConditionsForLevel : MonoBehaviour
{
    [SerializeField] private CountdownToStart _countdown;
    [SerializeField] private OnPauseOrWinOrDefeatPanelShow _panel;
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Health _health;
    [SerializeField] private SnakeCollision _leadingTrigger;
    [SerializeField] private SnakeMovement _snake;
    [SerializeField] private Effects _effects;
    [SerializeField] private SkinLoader _skinLoader;

    [SerializeField] private Image[] _currentStars;
    [SerializeField] private Sprite _filledStar;

    private Dictionary<LevelConditions, int> _conditionValues;

    private Condition[] _conditions;

    public int  DistractHedgehogs { get;  set; }
    public int KillAnts { get;  set; }
    public int FreezeHedgehogs { get;  set; }
    public int KillMouses { get; set; }
    public int PoisonMouses { get; set; }
    public int PoisonHedgehogs { get; set; }

    private bool[] _stars = new bool[3];
    public float Timer { get; private set; }

    private void Awake() => _conditions = SaveSerial.Instance.Data.Conditions;

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
            { LevelConditions.HoldOutSeconds, (int)Timer},
            { LevelConditions.PassLevelByEpicSkin, _skinLoader.CurrentSkin.Rarity == Rarity.Epic ? 1 : 0},

            { LevelConditions.PassByRareSkin, _skinLoader.CurrentSkin.Rarity == Rarity.Common ? 1 : 0},
            { LevelConditions.PassByLegendarySkin, _skinLoader.CurrentSkin.Rarity == Rarity.Legendary ? 1 : 0},
            { LevelConditions.KillMouses, KillMouses},
            { LevelConditions.PoisonMouses, PoisonMouses},
            { LevelConditions.PoisonHedgehogs, PoisonHedgehogs}
        };
    }

    private void Update()
    {
        if (Head().Name != LevelConditions.HoldOutSeconds)
            return;

        if (IsMainReached())
        {
            _panel.WinShow();
        }

        if (_countdown.LevelStarted)
            Timer += Time.deltaTime;
    }

    public void CalculateStars()
    {
        if (SaveSerial.Instance.Mode == LevelMode.Arena)
            return;

        if (SaveSerial.Instance.Mode == LevelMode.Custom)
            return;

        UpdateValues();

        for (int i = 0; i < _conditions.Length; i++)
        {
            if (IsMainReached() == false)
                break;

            var condition = _conditions[i];
            var name = _conditions[i].Name;

            _stars[i] = condition.IsComplete(_conditionValues[name]);
            _currentStars[i].sprite = _stars[i]
                ? _filledStar 
                : _currentStars[i].sprite;
        }


        int index = SaveSerial.Instance.Data.LevelIndex;
        string file = SaveSerial.JsonPaths.LevelStars;
        var oldStars = SaveSerial.Instance.Load(index, file, new bool[3]);
        for (int i = 0; i < oldStars.Length; i++)
        {
            if (_stars[i] == true)
                oldStars[i] = true;
        }
        SaveSerial.Instance.Save((index, oldStars), file);
    }

    public Condition Head() => _conditions[0];

    public bool IsMainReached()
    {
        UpdateValues();
        return Head().IsComplete(_conditionValues[Head().Name]);
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
        _conditionValues[LevelConditions.HoldOutSeconds] = (int)Timer;

        _conditionValues[LevelConditions.PassLevelByEpicSkin] = _skinLoader.CurrentSkin.Rarity == Rarity.Epic ? 1 : 0;
        _conditionValues[LevelConditions.PassByRareSkin] = _skinLoader.CurrentSkin.Rarity == Rarity.Common ? 1 : 0;
        _conditionValues[LevelConditions.PassByLegendarySkin] = _skinLoader.CurrentSkin.Rarity == Rarity.Legendary ? 1 : 0;
        _conditionValues[LevelConditions.KillMouses] = KillMouses;
        _conditionValues[LevelConditions.PoisonMouses] = PoisonMouses;
        _conditionValues[LevelConditions.PoisonHedgehogs] = PoisonHedgehogs;
    }
}
