using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using TMPro;

public class Chapter : SerializedMonoBehaviour
{
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private Button _button;
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private TextMeshProUGUI _comingSoon;
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _lock;
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private ImageText _levelsCompleted;
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private ImageText _starsCollected;
    [FoldoutGroup("Components"), ChildGameObjectsOnly]
    [SerializeField] private Image _shadow;

    [FoldoutGroup("References")]
    [SerializeField] private Level[] _levels;
    [ShowInInspector, OdinSerialize] public bool IsUnlock { get; private set; }

    private Level _boss;

    private void Awake()
    {
        if (_levels != null && _levels.Length > 0)
            _boss = _levels[^1];
    }

    private void Start()
    {
        Action action = IsUnlock
            ? () => Unlock()
            : () => Block();

        action?.Invoke();
    }

    public void OnSelect()
    {
        _shadow.gameObject.SetActive(true);
    }

    [Button]
    public void Unlock()
    {
        IsUnlock = true;
        _button.interactable = true;
        _comingSoon.gameObject.SetActive(false);
        _lock.gameObject.SetActive(false);
        _levelsCompleted.gameObject.SetActive(true);
        _starsCollected.gameObject.SetActive(true);
        //Remove after
        _shadow.gameObject.SetActive(true);

        if (_levels == null || _levels.Length <= 0)
            return;

        GetStars();
        GetCompletedLevels();
    }

    [Button, ShowInInspector]
    private void Block()
    {
        IsUnlock = false;
        _button.interactable = false;
        _comingSoon.gameObject.SetActive(true);
        _lock.gameObject.SetActive(true);
        _levelsCompleted.gameObject.SetActive(false);
        _starsCollected.gameObject.SetActive(false);
        _shadow.gameObject.SetActive(false);
    }

    private int GetStars()
    {
        int collectedStars = 0;
        foreach (var level in _levels)
        {
            collectedStars += level.GetCollectedStars();
        }
        _starsCollected.Text.text = $"{collectedStars}/{_levels.Length * 3}";
        return collectedStars;
    }

    private int GetCompletedLevels()
    {
        int levelsCompleted = 0;
        foreach (var level in _levels)
        {
            if (level.IsCompleted)
                levelsCompleted++;
        }
        _levelsCompleted.Text.text = $"{levelsCompleted}/{_levels.Length}";
        return levelsCompleted;
    }

    public bool IsComplete() => _boss.IsCompleted;
}
