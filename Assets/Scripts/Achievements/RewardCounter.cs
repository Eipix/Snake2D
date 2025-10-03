using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class RewardCounter : Counter
{
    [SerializeField] private List<Achievement> _achievements;

    protected override void OnEnable()
    {
        foreach (var achievement in _achievements)
        {
            var conditions = achievement.Conditions;
            foreach (var condition in conditions)
            {
                condition.Collected += UpdateCount;
                condition.Completed += UpdateCount;
            }
        }
        SaveSerial.Instance.DataLoaded += UpdateCount;
        UpdateCount();
    }

    protected override void OnDisable()
    {
        foreach (var achievement in _achievements)
        {
            var conditions = achievement.Conditions;
            foreach (var condition in conditions)
            {
                condition.Collected -= UpdateCount;
                condition.Completed -= UpdateCount;
            }
        }
        SaveSerial.Instance.DataLoaded -= UpdateCount;
    }

    [Button]
    public override void UpdateCount()
    {
        int value = 0;
        foreach (var achievement in _achievements)
        {
            achievement.Init();
            if (achievement.HaveCollectable())
                value++;
        }

        Circle.gameObject.SetActive(value > 0);
        Count.text = value.ToString();
    }
}
