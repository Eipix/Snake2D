using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(LuckySpin))]
public class AdSpin : MonoBehaviour
{
    [SerializeField] private AdSpinCounter _counter;
    [SerializeField] private Button _ad;
    //[SerializeField] private Notification _notification;

    public UnityAction SpinSpent;
    public UnityAction Restarted;

    Translatable<string> _adsErrorText = new Translatable<string>
    {
        Translated = new string[] { "An error occured", "Произошла ошибка", "Bir hata oluştu" }
    };
    Translatable<string> _notAvailable = new Translatable<string>
    {
        Translated = new string[] 
        { "Will be restored at 7 pm",
          "Будут восстановлены в 19:00",
          "19:00'da restore edilecek" }
    };

    private LuckySpin _luckySpin;

    private DateTime _waitingTime;
    public int CompletedSpin { get; private set; }

    public readonly int MaxSpin = 3;
    private readonly int _hourToUpdate = 19;

    private void Awake()
    {
        _luckySpin = GetComponent<LuckySpin>();
        _waitingTime = SaveSerial.Instance.Load<DateTime>(SaveSerial.JsonPaths.SpinAdTime);
        CompletedSpin = SaveSerial.Instance.Load<int>(SaveSerial.JsonPaths.SpinAdAttempts);
        _counter.UpdateCount();
    }

    //Invoke extern
    public void FreeSpin()
    {
        if (IsCooldownEnd())
        {
            CompletedSpin++;
            TryRestartCooldown();
            Save();
            StartCoroutine(_luckySpin.RotateWheel(0, _luckySpin.LastReward));
            SpinSpent?.Invoke();
        }
        else
        {
            Notification.Instance.Notify(_notAvailable.Translate);
        }
    }

    public void OnAdButtonClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (IsCooldownEnd())
        {
        YandexMarkups.Instance.ShowRewardedAd(transform.GetPathInHierarchy(), nameof(_luckySpin.GetReward), nameof(FreeSpin), () =>
        {
            _luckySpin.GetReward();
            FreeSpin();
        });
        }
        else
        {
             Notification.Instance.Notify(_notAvailable.Translate);
        }
#endif
    }

    public bool SpinAvailable() => CompletedSpin < MaxSpin;

    public bool IsCooldownEnd()
    {
        var mainTime = DateTime.MinValue;
#if UNITY_WEBGL && !UNITY_EDITOR
        mainTime = YandexMarkups.Instance.ServerTime;
        Debug.Log($"time {mainTime}, waitingTime: {_waitingTime}, server > wait: {mainTime > _waitingTime}");
#endif
        return mainTime > _waitingTime;
    }

    private void Save()
    {
        SaveSerial.Instance.Save(_waitingTime, SaveSerial.JsonPaths.SpinAdTime);
        SaveSerial.Instance.Save(CompletedSpin, SaveSerial.JsonPaths.SpinAdAttempts);
    }

    private bool TryRestartCooldown()
    {
        if (SpinAvailable() == false && IsCooldownEnd())
        {
            var mainTime = DateTime.MinValue;
#if UNITY_WEBGL && !UNITY_EDITOR
            mainTime = YandexMarkups.Instance.ServerTime;
#endif
            var addTime = Extensions.GetHourToAdd(mainTime, _hourToUpdate);
            _waitingTime = mainTime.Add(addTime);
            CompletedSpin = 0;
            Save();
            Restarted?.Invoke();
            return true;
        }
        return false;
    }
}
