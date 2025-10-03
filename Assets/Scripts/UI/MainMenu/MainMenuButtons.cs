using System;
using TMPro;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    [SerializeField] private PunchablePopup _infoWindow;
    [SerializeField] private TextMeshProUGUI _redAppleBar;
    [SerializeField] private TextMeshProUGUI _goldAppleBar;
    [SerializeField] private GameObject _levelsOption;
    [SerializeField] private GameObject _inventory;

    private void Start()
    {
        InitSkin();

        LoadWindow(_levelsOption, "Back");
        LoadWindow(_inventory, "EnterToInventory");

        SoundsPlayer.Instance.PlayMusic(SoundsPlayer.Instance.MenuTheme);
        Wallet.Instance.Init(_redAppleBar, _goldAppleBar);

        if(SaveSerial.Instance.Load(SaveSerial.JsonPaths.ShowTutorialWindow, true))
        {
            _infoWindow.Open();
            SaveSerial.Instance.Save(false, SaveSerial.JsonPaths.ShowTutorialWindow);
        }
    }

    public void LoadWindow(GameObject gameObject, string playerPrefs)
    {
        var willOpen = Convert.ToBoolean(PlayerPrefs.GetInt(playerPrefs, 0));
        gameObject.SetActive(willOpen);
        PlayerPrefs.SetInt(playerPrefs, 0);
    }
    
    //Set and save default snake
    private void InitSkin()
    {
        //set default skin if null
        if(SaveSerial.Instance.LoadCurrentSkinType() == "")
        {
            Debug.LogWarning("Pagko is set by default");
            SaveSerial.Instance.SaveCurrentSkin(null);
        }
    }
}
