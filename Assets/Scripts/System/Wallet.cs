using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class Wallet : Singleton<Wallet>
{
    [SerializeField] private TextMeshProUGUI _redAppleBar;
    [SerializeField] private TextMeshProUGUI _goldAppleBar;

    public UnityEvent SpentRedApple;
    public UnityEvent SpentGoldApple;

    private Tween _redAppleText;
    private Tween _goldAppleText;

    public int RedAppleCount => SaveSerial.Instance.LoadApple().Item1;
    public int GoldAppleCount => SaveSerial.Instance.LoadApple().Item2;

    public void Init(TextMeshProUGUI redAppleBar, TextMeshProUGUI goldAppleBar)
    {
        _redAppleBar = redAppleBar;
        _goldAppleBar = goldAppleBar;
        UpdateBalance();
    }

    public bool IsEnoughGoldApples(int price)
    {
        if (price < 0 || GoldAppleCount < price)
            return false;

        return true;
    }

    public void UpdateBalance()
    {
        _redAppleText.CompleteIfActive();
        _goldAppleText.CompleteIfActive();

        _redAppleText = _redAppleBar.DOText(RedAppleCount.ToString("D8"), 1f, false, ScrambleMode.None).SetUpdate(true);
        _goldAppleText = _goldAppleBar.DOText(GoldAppleCount.ToString("D8"), 1f, false, ScrambleMode.None).SetUpdate(true);
    }

    public bool TryGetRedApple(int count, bool updateBalance = true, Action onCredited = null)
    {
        if (count < 0)
            return false;

        int redApple = RedAppleCount + count;
        SaveSerial.Instance.SaveApple(redApple, GoldAppleCount);

        if (updateBalance)
            UpdateBalance();

        return true;
    }

    public bool TryGetGoldApple(int count, bool updateBalance = true)
    {
        if (count < 0)
            return false;

        int goldApple = GoldAppleCount + count;
        SaveSerial.Instance.SaveApple(RedAppleCount, goldApple);

        if (updateBalance)
            UpdateBalance();

        return true;
    }

    public bool TrySpentApples(Apple apple, int count)
    {
        if(apple is RedApple)
        {
            return TrySpentRedApple(count);
        }
        else if(apple is GoldApple)
        {
            return TrySpentGoldApple(count);
        }
        else
        {
            throw new InvalidOperationException($"you try spent not spendable apple: {apple.GetType().Name}");
        }
    }

    public bool TrySpentRedApple(int price)
    {
        if (price < 0 || RedAppleCount < price)
            return false;

        Debug.Log($"Current red apple: {RedAppleCount}");
        Debug.Log($"Price: {price}");

        int redApple = RedAppleCount - price;
        Debug.Log($"Remain red apple: {redApple}");

        SaveSerial.Instance.SaveApple(redApple, GoldAppleCount);
        SaveSerial.Instance.Increment(price, SaveSerial.JsonPaths.SpentRedApples);

        UpdateBalance();
        SpentRedApple?.Invoke();
        return true;
    }

    public bool TrySpentGoldApple(int price)
    {
        if (price < 0 || GoldAppleCount < price)
            return false;

        int goldApple = GoldAppleCount - price;
        SaveSerial.Instance.SaveApple(RedAppleCount, goldApple);
        SaveSerial.Instance.Increment(price, SaveSerial.JsonPaths.SpentGoldApples);

        UpdateBalance();
        SpentGoldApple?.Invoke();
        return true;
    }
}
