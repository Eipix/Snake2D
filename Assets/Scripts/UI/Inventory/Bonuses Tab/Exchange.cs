using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;

public class Exchange : MonoBehaviour
{
    [SerializeField] private RectTransform _slotForSell;
    [SerializeField] private RectTransform _slotForBuy;

    [SerializeField] private TMP_Text _counter;

    [SerializeField] private List<Offer> _offers = new List<Offer>();

    private BonusesTab _bonusesTab;
    private DateTime _endTime;

    private void OnDisable()
    {        
        PlayerPrefs.SetString("ExchangeTime", _endTime.ToString());
    }

    private void Start()
    {
        _bonusesTab = GetComponentInParent<BonusesTab>();

        var coin = Get(typeof(Coin));
        var goldPeach = Get(typeof(GoldPeach));
        var cheese = Get(typeof(Cheese));
        var bomb = Get(typeof(Bomb));
        var timer = Get(typeof(Timer));
        var peach = Get(typeof(Peach));
        var iceCube = Get(typeof(IceCube));
        var lightning = Get(typeof(Lightning));

        _offers.Add(new Offer(7, 1, cheese, goldPeach));
        _offers.Add(new Offer(9, 1, iceCube, coin));
        _offers.Add(new Offer(3, 1, peach, goldPeach));
        _offers.Add(new Offer(6, 1, bomb, coin));
        _offers.Add(new Offer(3, 1, lightning, goldPeach));
        _offers.Add(new Offer(7, 1, timer, coin));

        LoadOffer(_slotForSell, "SavedBonusForSell", 0);
        LoadOffer(_slotForBuy, "SavedBonusForBuy", 1);
        CheckTimeLeft();
    }

    private void Update()
    {
        var remainingTime = _endTime - DateTime.Now;    
        _counter.text = $"{remainingTime.Minutes}:{remainingTime.Seconds:D2}";

        if (DateTime.Now > _endTime)
        {
            UpdateOffer(_slotForSell, 0);
            UpdateOffer(_slotForBuy, 1);
        }
    }

    public Bonus Get(Type type)
    {
        return SaveSerial.Instance.BonusPrefabs
            .Where(bonus => bonus.GetType() == type)
            .FirstOrDefault();
    }

    public void OnExchangeClick()
    {
        if (PlayerPrefs.HasKey("SavedBonusForSell") && PlayerPrefs.HasKey("SavedBonusForBuy"))
        {
            int offerToTake = PlayerPrefs.GetInt("SavedBonusForSell");
            int amountToTake = _offers[offerToTake].Amounts[0];

            if (_offers[offerToTake].Bonuses[0].Amount >= amountToTake)
            {
                ConfirmationNotification.Instance.Show(() => OnConfirmClick());
            }
            else
            {
                string translatedText = Notification.Instance.LangNotEnoughRes.Translate;
                Notification.Instance.Notify(translatedText);
            }
        }
        else
        {
            Debug.LogError("Ошибка. Не удалось получить PlayerPrefs.HasKey(SavedBonusForSell) && PlayerPrefs.HasKey(SavedBonusForBuy)");
        }
    }
   
    public void OnConfirmClick()
    {
        int offerToTake = PlayerPrefs.GetInt("SavedBonusForSell");
        int offerToGive = PlayerPrefs.GetInt("SavedBonusForBuy");

        int amountToTake = _offers[offerToTake].Amounts[0];
        int amountToGive = _offers[offerToTake].Amounts[1];

        _offers[offerToTake].Bonuses[0].Decrese(amountToTake);
        _offers[offerToGive].Bonuses[1].Add(amountToGive);

        _bonusesTab.UpdateCells();

        Debug.Log("Exchange result was saved in sdk");

#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
    }

    public void LoadOffer(RectTransform slot, string key, int LeftRightBonus)
    {
        if (PlayerPrefs.HasKey(key))
        {
            int index = PlayerPrefs.GetInt(key);
            var amount = slot.GetComponentInChildren<TMP_Text>();

            Instantiate(_offers[index].Bonuses[LeftRightBonus], slot.transform);
            amount.text = _offers[index].Amounts[LeftRightBonus].ToString();
        }
        else
        {
            Debug.Log("not key in exchange");
            var amount = slot.GetComponentInChildren<TMP_Text>();
            int randomOffer = UnityEngine.Random.Range(0, _offers.Count);

            Instantiate(_offers[randomOffer].Bonuses[LeftRightBonus].gameObject, slot.transform);
            amount.text = _offers[randomOffer].Amounts[LeftRightBonus].ToString();
            PlayerPrefs.SetInt(LeftRightBonus == 0 ? "SavedBonusForSell" : "SavedBonusForBuy", randomOffer);
        }
    }

    public void UpdateOffer(RectTransform slot, int LeftRightBonus)
    {
        _endTime = DateTime.Now.AddMinutes(4 - (DateTime.Now.Minute % 5)).AddSeconds(60 - DateTime.Now.Second);

        Destroy(slot.GetComponentInChildren<Bonus>()?.gameObject);

        var amount = slot.GetComponentInChildren<TMP_Text>();
        int randomOffer = UnityEngine.Random.Range(0, _offers.Count);

        Instantiate(_offers[randomOffer].Bonuses[LeftRightBonus].gameObject, slot.transform);
        amount.text = _offers[randomOffer].Amounts[LeftRightBonus].ToString();
        PlayerPrefs.SetInt(LeftRightBonus == 0 ? "SavedBonusForSell" : "SavedBonusForBuy", randomOffer);
    }

    public void CheckTimeLeft()
    {
        if (PlayerPrefs.HasKey("ExchangeTime"))
        {
            string savedTime = PlayerPrefs.GetString("ExchangeTime");
            if (DateTime.Now > DateTime.Parse(savedTime))
            {
                UpdateOffer(_slotForSell, 0);
                UpdateOffer(_slotForBuy, 1);
            }
        }
        _endTime = DateTime.Now.AddMinutes(4 - (DateTime.Now.Minute % 5)).AddSeconds(60 - DateTime.Now.Second);
    }
}

public class Offer
{
    public Bonus[] Bonuses { get; private set; }
    public int[] Amounts { get; private set; }

    public Offer(int amountToSpend, int amountToReceive, Bonus bonusForSell, Bonus bonusForBuy)
    {
        Bonuses = new Bonus[2] { bonusForSell, bonusForBuy };
        Amounts = new int[2] { amountToSpend, amountToReceive };
    }
}

