using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;
using TMPro;

public class Exchange : MonoBehaviour
{
    [SerializeField] private RectTransform _slotForSell;
    [SerializeField] private RectTransform _slotForBuy;

    [SerializeField] private TMP_Text _counter;
    [SerializeField] private RectTransform _confirmationPanel;
    [SerializeField] private Notification _notification;

    [SerializeField] private Button _confirm;
    [SerializeField] private Button _cancel;

    [SerializeField] private Bonus[] _prefabBonuses;
    [SerializeField] private List<Offer> _offers = new List<Offer>();

    private BonusesTab _bonusesTab;
    private DateTime _endTime;

    public UnityAction<string> Clicked;

    private void OnEnable()
    {
        Clicked += _notification.Notify;
    }

    private void OnDisable()
    {        
        PlayerPrefs.SetString("ExchangeTime", _endTime.ToString());
        Clicked -= _notification.Notify;
    }

    private void Start()
    {
        _bonusesTab = GetComponentInParent<BonusesTab>();
        _confirmationPanel.gameObject.SetActive(false);

        _offers.Add(new Offer(7, 1, _prefabBonuses[1], _prefabBonuses[3]));
        _offers.Add(new Offer(9, 1, _prefabBonuses[4], _prefabBonuses[2]));
        _offers.Add(new Offer(3, 1, _prefabBonuses[6], _prefabBonuses[3]));
        _offers.Add(new Offer(6, 1, _prefabBonuses[0], _prefabBonuses[2]));
        _offers.Add(new Offer(3, 1, _prefabBonuses[5], _prefabBonuses[3]));
        _offers.Add(new Offer(7, 1, _prefabBonuses[7], _prefabBonuses[2]));

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

    public void OnExchangeClick()
    {
        if (PlayerPrefs.HasKey("SavedBonusForSell") && PlayerPrefs.HasKey("SavedBonusForBuy"))
        {
            int offerToTake = PlayerPrefs.GetInt("SavedBonusForSell");
            int amountToTake = _offers[offerToTake].Amounts[0];

            if (_offers[offerToTake].Bonuses[0].GetAmount() >= amountToTake)
            {               
                _confirmationPanel.gameObject.SetActive(true);
            }
            else
            {
                Clicked?.Invoke("Недостаточно ресурсов");
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

        _confirmationPanel.gameObject.SetActive(false);     
    }

    public void OnCancelClick() 
    {
        _confirmationPanel.gameObject.SetActive(false);
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
    }

    public void UpdateOffer(RectTransform slot, int LeftRightBonus)
    {
        _confirmationPanel.gameObject.SetActive(false);
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

