using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [SerializeField] private RectTransform _frame;
    [SerializeField] private Vector2 _targetPosition;

    private void Awake() => _frame = transform as RectTransform;

    public void Move(RectTransform target)
    {
        _frame.SetParent(target);
        _frame.anchoredPosition = _targetPosition;
    }
}
