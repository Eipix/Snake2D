using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class Level : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private MissionRequirements _levels;

    [Header("Components In Children")]
    [SerializeField] private Image _whiteCircle;
    [SerializeField] private Image _icon;
    [SerializeField] private Image[] _stars;

    [Header("Sprites")]
    [SerializeField] private Sprite _receivedStar;
    [SerializeField] private Sprite _unlockLevelIcon;
    [SerializeField] private Sprite _filledWhiteCircle;

    [Header("Data")]
    [SerializeField] private Condition[] _conditions;
    [SerializeField] private EnemyData[] _enemyData;
    [SerializeField] private EnemyData[] _bosses;
    [SerializeField] private int _levelNumber;
    [SerializeField] private int _rottenAppleChance;
    [SerializeField] private int _maxStoneCount;
    [SerializeField] private int _maxEnemyCount;
    [SerializeField] private int _maxAppleCount;
    [SerializeField] private int _delayBeforeEnemySpawn;

    [field:Header("Indicator")]
    [field: SerializeField] public float Offset {get; private set; }

    private Button _button;
    private RectTransform _rectTransform;

    public LevelData Data { get; private set; }
    public Vector2 AnchoredPosition => _rectTransform.anchoredPosition;

    public int Number => _levelNumber;
    public bool IsUnlock => _button.interactable;

    private void Awake()
    {
        Data = new LevelData(_conditions, _enemyData, _bosses, _levelNumber - 1, _delayBeforeEnemySpawn, _rottenAppleChance,
                             _maxStoneCount, _maxEnemyCount, _maxAppleCount);
        _button = GetComponent<Button>();
        _rectTransform = GetComponent<RectTransform>();

        if (_levelNumber != 1)
            _button.interactable = false;

        Init();
    }

    public void OnLevelClick()
    {
        _levels.OnMissionClick(_levelNumber);
    }

    private void Init()
    {
        if (IsComplete(_levelNumber))
        {
            _whiteCircle.sprite = _filledWhiteCircle;
        }

        ValidateFields();
        TryUnlockLevel();
        AssignStars();
    }

    private void ValidateFields()
    {
        int totalChances = 0;
        foreach (var data in _enemyData)
        {
            totalChances += data.SpawnChances;
        }

        if (totalChances != 100 && _enemyData.Length > 0)
            throw new InvalidOperationException("Сумма всех шансов на спавн NPC не может быть больше или меньше 100%");

        if (_stars.Length != 3)
            throw new InvalidOperationException("В уровне должно быть 3 звезды");

        if (_conditions.Length != 3)
            throw new InvalidOperationException("В уровне должно быть 3 условия");
    }

    private void AssignStars()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].sprite = IsComplete(_levelNumber, i)
                    ? _receivedStar
                    : _stars[i].sprite;
        }
    }

    private bool TryUnlockLevel()
    {
        int previousLevel = _levelNumber - 1;
        if (IsComplete(previousLevel))
        {
            _icon.sprite = _unlockLevelIcon;
            _icon.SetNativeSize();
            _button.interactable = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public string GetConditionText()
    {
        return $"{_conditions[0].Description}\n{ _conditions[1].Description}\n{_conditions[2].Description}\n";
    }

    public bool IsComplete(int level, int indexOfStar = 0) => (bool)_saveSerial.LoadStars(level - 1).GetValue(indexOfStar);

    public bool IsComplete() => (bool)_saveSerial.LoadStars(_levelNumber - 1).GetValue(0);
}

[Serializable]
public struct EnemyData
{
    public Enemy EnemyPrefab;
    public int SpawnChances;
}

[Serializable]
public class LevelData
{
    public Condition[] Conditions;
    public EnemyData[] EnemyData;
    public EnemyData[] Bosses;
    public int LevelIndex;
    public int RottenAppleChance;
    public int MaxStoneCount;
    public int MaxEnemyCount;
    public int MaxAppleCount;
    public int DelayBeforeEnemySpawn;

    public LevelData(Condition[] conditions, EnemyData[] enemyData,EnemyData[] bosses, int levelIndex, int delayBeforeEnemySpawn, int rottenAppleChance, int maxStoneCount, int maxEnemyCount, int maxApple)
    {
        LevelIndex = levelIndex;
        DelayBeforeEnemySpawn = delayBeforeEnemySpawn;
        Conditions = conditions;
        EnemyData = enemyData;
        Bosses = bosses;
        RottenAppleChance = rottenAppleChance;
        MaxStoneCount = maxStoneCount;
        MaxEnemyCount = maxEnemyCount;
        MaxAppleCount = maxApple;
    }
}
