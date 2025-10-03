using UnityEngine;

public class Settings : Singleton<Settings>
{
    [SerializeField] private PunchablePopup _popup;
    public PunchablePopup Popup => _popup;
}
