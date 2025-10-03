using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ConfirmationNotification : Singleton<ConfirmationNotification>
{
    [SerializeField] private GameObject _notification;
    [SerializeField] private Button _agree;
    [SerializeField] private Button _cancel;

    private UnityAction _current;

    private void Start()
    {
        _cancel.onClick.AddListener(Close);
    }

    public void Show(UnityAction action)
    {
        _current = action;

        _agree.onClick.AddListener(action);
        _agree.onClick.AddListener(Close);

        _notification.SetActive(true);
    }

    private void Close()
    {
        _agree.onClick.RemoveListener(_current);
        _agree.onClick.RemoveListener(Close);

        _notification.SetActive(false);
    }
}
