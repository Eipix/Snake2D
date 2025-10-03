using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System;

public class AchievementView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Achievement _achievement;
    [SerializeField] private CollectAnimation _collection;
    [SerializeField] private RectTransform _balanceRedApples;
    [SerializeField] private RectTransform _balanceGoldenApples;
    
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private AchievementComplete _completion;
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private ProgressBar _progressBar;
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private Image _rewardIcon;
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private Button _collect;
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private TextMeshProUGUI _description;
    [SerializeField, FoldoutGroup("Child Components"), ChildGameObjectsOnly]
    private TextMeshProUGUI _rewardCount;

    [SerializeField]
    private Translatable<string> _langDescription = new Translatable<string>
    {
        Translated = new string[3]
    };

    private AchievementCondition _current;
    private RectTransform _rectTransform;

    private Vector2 _range;

    private int _currentIndex = 0;
    private int _conditionsLength = 0;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _achievement.Init();
        _conditionsLength = _achievement.Conditions.Length;
        _range = new Vector2(0, _conditionsLength);
    }

    private void OnEnable()
    {
        _achievement.UpdateData();
        _collect.interactable = false;
        _current = GetFirstOrDeafult(cond => cond.IsCompleted == false || cond.IsCollected == false);

        if (_current == null)
        {
            Switch(_conditionsLength - 1);
            _progressBar.Fill(_range, _currentIndex);
            _completion.SetCompleted(_rectTransform);
            _progressBar.SetMax();
            return;
        }

        TryTakeCollectible();
        Switch(_currentIndex);
        _progressBar.Fill(_range, _currentIndex);
    }

    public void OnCollectClick()
    {
        _current.Collect((_achievement.Key, _achievement.Datas));
        _collection.transform.position = _rewardIcon.transform.position;
        _collection.SetParametrs(_current.Reward.Shadowed, _current.Reward is RedApple
                                                                  ? _balanceRedApples 
                                                                  : _balanceGoldenApples);
        StartCoroutine(_collection.Play(_current.RewardCount));
        Wallet.Instance.UpdateBalance();

        if(_currentIndex >= _conditionsLength - 1)
        {
            _progressBar.DOFill(_range, _currentIndex + 1, 0.3f);
            _completion.PlayOneShot();
            return;
        }

        _collect.interactable = false;
        _progressBar.DOFill(_range, _currentIndex + 1, 0.3f);
        if(TryTakeCollectible() == false)
        {
            _current = GetFirstOrDeafult(cond => !cond.IsCompleted);
        }
        Switch(_currentIndex);
    }

    private bool TryTakeCollectible()
    {
        var collectable = GetFirstOrDeafult(cond => cond.IsCompleted && cond.IsCollected == false);
        bool notNull = collectable != null;
        if (notNull)
        {
            _current = collectable;
            _collect.interactable = true;
        }
        return notNull;
    }

    private void Switch(int index)
    {
        var conditions = _achievement.Conditions;

        _description.text = $"{_langDescription.Translate} <color=#BBFF00>{_achievement.CurrentValue}/</color><color=#42C452>{conditions[index].TargetValue}</color>";
        _rewardCount.text = $"x{conditions[index].RewardCount}";

        _rewardIcon.sprite = conditions[index].Reward.Shadowed;
        _rewardIcon.SetNativeSize();
    }

    private AchievementCondition GetFirstOrDeafult(Predicate<AchievementData> predicate)
    {
        var conditions = _achievement.Conditions;
        for (int i = 0; i < _conditionsLength; i++)
        {
            if (predicate.Invoke(conditions[i].Data))
            {
                _currentIndex = i;
                return conditions[i];
            }
        }
        return null;
    }
}
