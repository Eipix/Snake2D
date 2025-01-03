using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;

public class Slots : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private Bonus[] _bonusPrefabs;

    private Slot[] _slots = new Slot[3];
    private SlotCross[] _crosses = new SlotCross[3];

    private void OnEnable()
    {
        Init();
        UpdateSlots();
    }

    private void Start()
    {
        Init();
        UpdateSlots();
    }

    private void Init()
    {
        _slots = GetComponentsInChildren<Slot>();
        for (int i = 0; i < _slots.Length; i++)
            _crosses[i] = _slots[i].GetComponentInChildren<SlotCross>();
    }
    
    public void UpdateSlots()
    {
        ClearSlots();
        SpawnBonuses();
    }

    private void SpawnBonuses()
    {
        var bonusesInSlots = FilterArray(_bonusPrefabs, _saveSerial.LoadBonusInSlots());

        for (int i = 0; i < bonusesInSlots.Count; i++)
        {
            var bonus = Instantiate(bonusesInSlots.ElementAt(i).Key.gameObject, _slots[i].transform);
            bonus.transform.SetAsFirstSibling();

            _slots[i].GetComponentInChildren<TMP_Text>().text = bonusesInSlots.ElementAt(i).Value.ToString();
            _crosses[i].GetComponent<Image>().enabled = false;
        }  
    }

    private void ClearSlots()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            Bonus bonus = _slots[i].GetComponentInChildren<Bonus>();
            if (bonus != null)
                Destroy(bonus.gameObject);
            _crosses[i].GetComponent<Image>().enabled = true;
            _slots[i].GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    public void UpdateBonuses()
    {
#nullable enable
        for (int i = 0; i < _slots.Length; i++)
        {
            Bonus? bonus = _slots[i].GetComponentInChildren<Bonus>();
            if(bonus != null)
            {
                if(_saveSerial.LoadBonusInSlots().ContainsKey(bonus.GetType().ToString()))
                {
                    _slots[i].GetComponentInChildren<TMP_Text>().text = _saveSerial.LoadBonusInSlots()[bonus.GetType().ToString()].ToString();
                }
                else
                {
                    Destroy(bonus.gameObject);
                    _crosses[i].GetComponent<Image>().enabled = true;
                    _slots[i].GetComponentInChildren<TMP_Text>().text = "";
                }

            }
        }
#nullable disable
    }

    private Dictionary<Bonus, int> FilterArray(Bonus[] bonuses, Dictionary<string, int> savedBonuses)
    {
        Dictionary<Bonus, int> filteredDict = new Dictionary<Bonus, int>();
        for (int i = 0; i < savedBonuses.Count; i++)
        {
            for (int j = 0; j < bonuses.Length; j++)
            {
                if (bonuses[j].GetType().ToString() == savedBonuses.ElementAt(i).Key)
                {
                    filteredDict.Add(bonuses[j], savedBonuses.ElementAt(i).Value);
                    break;
                }
            }
        }
        return filteredDict;
    }
}
