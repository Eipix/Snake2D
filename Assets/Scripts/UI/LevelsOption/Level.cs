using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MissionRequirements _levels;

    [Header("Components In Children")]
    [SerializeField] private Image[] _stars;
    [SerializeField] private Image _whiteCircle;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _rectTransform;

    [Header("Sprites")]
    [SerializeField] private Sprite _receivedStar;
    [SerializeField] private Sprite _unlockLevelIcon;
    [SerializeField] private Sprite _filledWhiteCircle;

    [SerializeField] private LevelData _data;

    [field:Header("Indicator")]
    [field: SerializeField] public float Offset {get; private set; }

    public Vector2 Position => _rectTransform.anchoredPosition;
    public LevelData Data => _data;

    public bool[] Stars => SaveSerial.Instance.Load(_data.LevelIndex, SaveSerial.JsonPaths.LevelStars, new bool[3]);
    public bool IsCompleted => Stars[0] == true;
    public bool IsUnlock => IsComplete(_data.LevelIndex - 1) || _data.LevelIndex == 0;

    private void Awake()
    {
        if (_data.LevelIndex != 0)
            _button.interactable = false;

        Init();
    }

    private void OnEnable() => Init();

    private void Init()
    {
        if (IsComplete(_data.LevelIndex))
        {
            _whiteCircle.sprite = _filledWhiteCircle;
        }

        ValidateFields();
        TryUnlockLevel();
        AssignStars();
    }

    public void OnLevelClick()
    {
        _levels.OnMissionClick(_data.LevelIndex);
    }

    private void ValidateFields()
    {
        int totalChances = 0;
        foreach (var data in _data.EnemyData)
        {
            totalChances += data.SpawnChances;
        }

        if (totalChances != 100 && _data.EnemyData.Length > 0)
            throw new InvalidOperationException("Сумма всех шансов на спавн NPC не может быть больше или меньше 100%");

        if (_stars.Length != 3)
            throw new InvalidOperationException("В уровне должно быть 3 звезды");

        if (_data.Conditions.Length != 3)
            throw new InvalidOperationException("В уровне должно быть 3 условия");
    }

    private void AssignStars()
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i].sprite = IsComplete(_data.LevelIndex, i)
                    ? _receivedStar
                    : _stars[i].sprite;
        }
    }

    private bool TryUnlockLevel()
    {
        int previousLevel = _data.LevelIndex - 1;
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
        return $"{_data.Conditions[0].Text.Translate}\n{ _data.Conditions[1].Text.Translate}\n{_data.Conditions[2].Text.Translate}\n";
    }

    public int GetCollectedStars() => Stars.Count(stars => stars == true);

    public bool IsComplete(int levelIndex, int indexOfStar = 0)
    {
        bool[] stars = SaveSerial.Instance.Load(levelIndex, SaveSerial.JsonPaths.LevelStars, new bool[3]);
        return stars[indexOfStar];
    }
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
}
