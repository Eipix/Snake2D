using UnityEngine;

public class InventoryMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private Slots _slots;
    [SerializeField] private GameObject _missionRequirements;

    public void OnBackClick()
    {
        if (_missionRequirements.activeSelf)
            _slots.UpdateSlots();

        _inventoryPanel.SetActive(false);
    }
}
