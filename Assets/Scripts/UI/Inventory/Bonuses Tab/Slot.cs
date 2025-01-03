using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField] private SlotReload _reload;
    [SerializeField] private Image _selected;

    private BonusesTab _bonusesTab;

    public void OnInventorySlotClick()
    {
#nullable enable
        Bonus? bonus = GetComponentInChildren<Bonus>();
        if (bonus != null)
        {
            _bonusesTab = GetComponentInParent<BonusesTab>();
            _bonusesTab.MoveFromSlotToCell(bonus);
        }
#nullable disable
    }

    public void OnBonusClick()
    {
#nullable enable
        Bonus? bonus = GetComponentInChildren<Bonus>();
        bonus?.Effect();
#nullable disable
    }

    public void ReloadActive(bool active) => _reload.gameObject.SetActive(active);

    public void EnableSelected() => _selected.gameObject.SetActive(true);

    public void DisableSelected() => _selected.gameObject.SetActive(false);
}
