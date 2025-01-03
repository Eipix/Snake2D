using UnityEngine;

public class LevelOptionsMenuButtons : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenu;
    [SerializeField] private GameObject _levelOptions;

    public void OnHomeClick()
    {
        _mainMenu.SetActive(true);
        _levelOptions.SetActive(false);
    }
}
