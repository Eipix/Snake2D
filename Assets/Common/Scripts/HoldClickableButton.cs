using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class HoldClickableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _holdDuration;

    public UnityEvent OnClicked;
    public UnityEvent OnHoldClicked;

    private readonly float _cooldown = 0.1f;

    private bool _isHoldingButton;
    private float _cooldownElapsed;
    private float _elapsedTime;

    public void OnPointerDown(PointerEventData eventData) => ToggleHoldingButton(true);

    private void ToggleHoldingButton(bool isPointerDown)
    {
        _isHoldingButton = isPointerDown;

        if (isPointerDown)
            _elapsedTime = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ManageButtonInteraction(true);
        ToggleHoldingButton(false);
    }

    private void ManageButtonInteraction(bool isPointerUp = false)
    {
        if (!_isHoldingButton)
            return;

        if (isPointerUp)
        {
            OnClicked?.Invoke();
            return;
        }

        _elapsedTime += Time.deltaTime;
        var isHoldClickDurationReached = _elapsedTime > _holdDuration;


        if (isHoldClickDurationReached)
        {
            _cooldownElapsed += Time.deltaTime;
            if (_cooldownElapsed > _cooldown)
            {
                OnHoldClicked?.Invoke();
                _cooldownElapsed = 0f;
            }
        }
    }

    private void Update() => ManageButtonInteraction();
}
