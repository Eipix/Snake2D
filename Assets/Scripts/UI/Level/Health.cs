using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private SnakeMovement _snake;
    [SerializeField] private Effects _effects;
    [SerializeField] private ParametrAnimation _parametr;

    [SerializeField] private GameObject _prefabHealth;
    [SerializeField] private GameObject _prefabEmptyHealth;

    public UnityAction<Type> SkillActivated;

    private OnPauseOrWinOrDefeatPanelShow _OnPauseOrWinOrDefeatPanelShow;
    private Image _firstHealth;
    private Sequence _blink;

    public int TotalLostHealthCount { get; private set; }
    public int CurrentHealths { get; private set; }
    
    private int _healthCount = 3;
    private System.Random rand = new System.Random();
    private int _luck = 0;

    private void OnEnable() => SkillActivated += _parametr.IconFlyAway;

    private void OnDisable() => SkillActivated -= _parametr.IconFlyAway;

    private void Start()
    {
        _OnPauseOrWinOrDefeatPanelShow = GetComponentInParent<OnPauseOrWinOrDefeatPanelShow>();
        _luck = _saveSerial.LoadLuck();
        _healthCount = _saveSerial.LoadHealth();
        CurrentHealths = _healthCount;
        UpdateValue();
    }

    public void UpdateValue()
    {
        Clear();

        int count = 0;
        int healthsLost = _healthCount - CurrentHealths;

        for (int i = 0; i < healthsLost; i++)
        {
            var position = new Vector2(transform.position.x - (0.65f * count), transform.position.y);
            Instantiate(_prefabEmptyHealth, position, Quaternion.identity, transform);
            count++;   
        }

        for (int i = 0; i < CurrentHealths; i++)
        {
            var position = new Vector2(transform.position.x - (0.65f * count), transform.position.y);
            var health = Instantiate(_prefabHealth, position, Quaternion.identity, transform);
            _firstHealth = i == 0 ? health.GetComponent<Image>() : _firstHealth;
            count++;
        }
    }
    
    public void Recovery()
    {
        if (CurrentHealths < _healthCount)
        {
            CurrentHealths++;
            UpdateValue();
        }
    }

    public void Lost(int damage = 1)
    {
        bool isLuck = rand.withProbability(_luck);
        if (!isLuck && _effects.IsInvincible == false)
        {
            TotalLostHealthCount += damage;
            CurrentHealths -= damage;

            StartCoroutine(HeartBlink());
            StartCoroutine(_snake.HitAnimation());
        }

        if (isLuck)
        {
            SkillActivated?.Invoke(typeof(LuckParametr));
        }
    }

    private IEnumerator HeartBlink()
    {
        _blink = DOTween.Sequence()
            .Append(_firstHealth.DOFade(0, 0f))
            .AppendInterval(0.05f)
            .Append(_firstHealth.DOFade(1, 0f))
            .AppendInterval(0.05f)
            .SetLoops(5);

        yield return _blink.WaitForCompletion();
        UpdateValue();
        CheckDefeate();
    }
    
    private void Clear()
    {
        var healths = GetComponentsInChildren<Image>();
        foreach (var health in healths)
            health.gameObject.SetActive(false);
    }

    private void CheckDefeate()
    {
        if (CurrentHealths < 1)
            _OnPauseOrWinOrDefeatPanelShow.DefeatShow();
    }
}
