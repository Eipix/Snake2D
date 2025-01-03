using UnityEngine;
using UnityEngine.EventSystems;

public class SnakeInputController : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SnakeMovement _snakeMovement;
    [SerializeField] private SaveSerial _saveSerial;

    public Vector2 TouchPoint { get; private set; }
    public bool isTouch { get; private set; }

    private float _deltaX, _deltaY;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            ChangeDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ChangeDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            ChangeDirection(Vector2.right);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _deltaX = eventData.delta.x;
        _deltaY = eventData.delta.y;
    }

    public void OnDrag(PointerEventData eventData) { }

    public float GetDeltaX() => _deltaX;
    public float GetDeltaY() => _deltaY;

    public void ChangeDirection(Vector2 vector2)
    {
        _deltaX = vector2.x;
        _deltaY = vector2.y;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IfPointerInValidZone(eventData);
    }

    public void OnPointerUp(PointerEventData eventData) => isTouch = false;

    public void IfPointerInValidZone(PointerEventData eventData)
    {
        TouchPoint = eventData.pointerPressRaycast.worldPosition;
        var hits = Physics2D.Raycast(TouchPoint, TouchPoint, 0.05f);

        if (hits.collider != null && (hits.collider.TryGetComponent(out MapBorders fence) || hits.collider.TryGetComponent(out Stone stone)))
            return;

        isTouch = true;
    }
}
