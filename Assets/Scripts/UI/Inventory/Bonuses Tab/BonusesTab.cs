using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusesTab : MonoBehaviour
{
    [SerializeField] private InventoryArt _inventoryArt;
    [SerializeField] private GameObject _prefabCell;

    private List<GameObject> _cells = new List<GameObject>();
    private Slots _slots;
    private GameObject _content;

    private bool _isInit = false;

    private void OnEnable()
    {
        if (!_isInit) Init();
        UpdateCells();
    }

    private void Init()
    {
        _slots = GetComponentInChildren<Slots>();
        _content = GetComponent<ScrollRect>().content.gameObject;
        _isInit = true;
    }

    public void SetCellOnUnselectState()
    {
        foreach (var cell in _cells)
        {
            var selectedCell = cell.GetComponentInChildren<SelectedCell>();
            selectedCell.GetComponent<Image>().enabled = false;
        }
    }

    private void ClearCells()
    {
        if(_cells != null)
        {           
            foreach (var cell in _cells)
                Destroy(cell.gameObject);     
        }
        _cells = new List<GameObject>();
    }

    public void UpdateCells()
    {
        ClearCells();
        Queue<Bonus> bonuses = new Queue<Bonus>(SaveSerial.Instance.BonusPrefabs);
        for (int i = 0; i < 8; i++)
        {
            var cell = Instantiate(_prefabCell, _content.transform);
            PlaceBonuses(bonuses, cell);
            CheckBonusInCell(cell);
            _cells.Add(cell);
        }
        AmountInCells();
    }

    private void PlaceBonuses(Queue<Bonus> bonuses, GameObject cell)
    {
        while (bonuses.Count > 0)
        {
            if (bonuses.Peek().Amount > 0)
            {
                Instantiate(bonuses.Peek(), cell.transform);
                bonuses.Dequeue();
                break;
            }
            else
            {
                bonuses.Dequeue();
            }
        }
    }

    private void CheckBonusInCell(GameObject cell)
    {
#nullable enable
        Bonus? child = cell.GetComponentInChildren<Bonus>();
        if (child == null)
            cell.GetComponent<Button>().interactable = false;
#nullable disable
    }
    
    private int AmountInCells()
    {
        var bonusPrefabs = SaveSerial.Instance.BonusPrefabs;
        int allBonusAmount = 0;
        Bonus first = null;

        foreach (var bonus in bonusPrefabs)
        {
            if (bonus.Amount > 0)
            {
                first = bonus;
                break;
            }
        }

        foreach (var bonus in bonusPrefabs)
            allBonusAmount += bonus.Amount;

        if (allBonusAmount == 0)
            _inventoryArt.ArtEmpty();
        else if (first != null)
            _inventoryArt.ChangeArt(first);

        return allBonusAmount;
    }
    
    public void ChangeArtSprites(Bonus bonus) => _inventoryArt.ChangeArt(bonus);
    
    public void MoveFromCellToSlot(Bonus bonus)
    {
        var bonusInSlots = SaveSerial.Instance.LoadBonusInSlots();
        bool Contains = bonusInSlots.TryGetValue(bonus.GetType().ToString(), out int value);
        bool LessThanMaxToAdd = value < bonus.MaxToAdd;
        bool NotContains = bonusInSlots.Count < 3;

        if ((NotContains && LessThanMaxToAdd) || (Contains && LessThanMaxToAdd))
        {            
            bonus.Decrese();
            SaveSerial.Instance.SaveBonusInSlots(bonus);
            _slots.UpdateSlots();
            UpdateCells();
        }
    }

    public void MoveFromSlotToCell(Bonus bonus)
    {
        bonus.Add();
        SaveSerial.Instance.SaveBonusInSlots(bonus, -1);
        _slots.UpdateSlots();
        UpdateCells();
    }

    public void OnClearSlotsClick()
    {
        Slot[] slots = GetComponentsInChildren<Slot>();

        foreach (var slot in slots)
        {
            if(slot.GetComponentInChildren<Bonus>() != null)
            {
                Bonus bonus = slot.GetComponentInChildren<Bonus>();
                int amountInSlot;
                if (SaveSerial.Instance.LoadBonusInSlots().TryGetValue(bonus.GetType().ToString(), out amountInSlot))
                {
                    bonus.Add(amountInSlot);
                    SaveSerial.Instance.SaveBonusInSlots(bonus, - amountInSlot);
                }
            }
        }
        _slots.UpdateSlots();
        UpdateCells();
    }
}
