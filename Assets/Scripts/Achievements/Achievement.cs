using System.Linq;
using UnityEngine;
using static SaveSerial;

public abstract class Achievement : MonoBehaviour
{
    [field: SerializeField] protected Sprite Icon { get; private set; }
    [SerializeField] private Translatable<string> _langName = new Translatable<string>
    {
        Translated = new string[] { "\"pioneer\"", "\"первопроходец\"", "\"öncü\"" }
    };
    [field: SerializeField] public AchievementCondition[] Conditions { get; private set; }
    public AchievementData[] Datas => Conditions.Select(cond => cond.Data).ToArray();

    protected abstract string SaveFile { get; }
    public int CurrentValue => SaveSerial.Instance.Load<int>(SaveFile);
    public string Key => GetType().Name;

    private bool _isInit;

    protected virtual void Awake() => Init();

    public void Init()
    {
        foreach (var condition in Conditions)
        {
            condition.Data = new AchievementData();
        }

        UpdateData();
    }

    public void UpdateData()
    {
        var data = SaveSerial.Instance.Load<string, AchievementData[]>(Key, JsonPaths.Achievements)
                     ?? new AchievementData[0];

        int dataLength = data.Length;
        for (int i = 0; i < dataLength; i++)
        {
            Conditions[i].Data = data[i];
        }
    }

    public bool HaveCollectable() => Datas.Count(data => data.IsCompleted && !data.IsCollected) > 0;

    public virtual void CheckCompletion()
    {
        TryInit();
        var currentIndex = GetFirstNotCompleted();

        if (currentIndex > Conditions.Length - 1)
            return;

        var current = Conditions[currentIndex];
        if (CurrentValue < current.TargetValue)
            return;

        current.Complete((Key, Datas));
        ShowNotification(current);
        CheckCompletion();
    }

    private void ShowNotification(AchievementCondition current)
    {
        string notifyText = $"{_langName.Translate}\n<color=#BBFF00>{current.Reward.LangName.Translate} x{current.RewardCount}</color>";
        SideNotification.Instance.Show(notifyText, Icon);
    }

    private int GetFirstNotCompleted()
    {
        UpdateData();
        for (int i = 0; i < Conditions.Length; i++)
        {
            if (Conditions[i].Data.IsCompleted == false)
            {
                return i;
            }
        }
        return Conditions.Length;
    }

    private void TryInit()
    {
        if (_isInit == false)
        {
            Init();
            _isInit = true;
            //Debug.Log($"({Key}) Init");
        }
    }
}
