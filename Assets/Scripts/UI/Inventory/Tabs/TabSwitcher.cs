using UnityEngine;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private GameObject[] _tabButtons;
    [SerializeField] private Sprite _tabPressed;
    [SerializeField] private Sprite _tabNotPressed;

    public void OnTabClick(int tabIndex)
    {
        SwitchTab((Tabs)tabIndex);        
    }

    private void SwitchTab(Tabs tab)
    {
        DisableAllTabs();
        _tabs[(int)tab].SetActive(true);
        _tabButtons[(int)tab].GetComponent<Image>().sprite = _tabPressed;
    }

    private void DisableAllTabs()
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].SetActive(false);
            _tabButtons[i].GetComponent<Image>().sprite = _tabNotPressed;
        }
    }
}
public enum Tabs
{
    Skins,
    Upgrade,
    Bonuses
}
