using UnityEngine;
using TMPro;
using DG.Tweening;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _luckySpin;
    [SerializeField] private TabSwitcher _inventory;
    [SerializeField] private GameObject _levelsOption;
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private TMP_Text _prefabAppleBar;
    [SerializeField] private TMP_Text _prefabGoldAppleBar;

    void Start()
    {
        _inventory.gameObject.SetActive(false);

        var backClicked = System.Convert.ToBoolean(PlayerPrefs.GetInt("Back"));

        _mainMenu.SetActive(!backClicked);
        _levelsOption.SetActive(backClicked);

        PlayerPrefs.SetInt("Back", 0);

        UpdateAppleBarsInMenu();
    }

    public void OnPlayClick()
    {
        _levelsOption.SetActive(true);
    }
    
    public void EnableLuckySpin()
    {
        _luckySpin.SetActive(true);
    }

    public void DisableLuckySpin()
    {
        _luckySpin.SetActive(false);
    }

    public void OnInventoryClick()
    {
        _inventory.gameObject.SetActive(true);
        _inventory.OnTabClick((int)Tabs.Bonuses);
    }

    public void UpdateAppleBarsInMenu()
    {
        (int redAppleAmount, int goldAppleAmount) = _saveSerial.LoadApple();

        _prefabGoldAppleBar.DOText(goldAppleAmount.ToString("D8"), 1f, false, ScrambleMode.None);
        _prefabAppleBar.DOText(redAppleAmount.ToString("D8"), 1f, false, ScrambleMode.None);
    }
}
