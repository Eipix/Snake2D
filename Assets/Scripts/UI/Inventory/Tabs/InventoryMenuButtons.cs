using UnityEngine;

public class InventoryMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Slots _slots;
    [SerializeField] private GameObject _missionRequirements;

    public void OnBackClick()
    {
        UpdateSlotsIfActive();
        _inventoryPanel.SetActive(false);
    }

    public void UpdateSlotsIfActive()
    {
        if (_missionRequirements.activeSelf)
            _slots.UpdateSlots();
    }
}
