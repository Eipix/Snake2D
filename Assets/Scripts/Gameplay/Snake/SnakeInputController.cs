using UnityEngine;
using UnityEngine.EventSystems;

public class SnakeInputController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private SnakeMovement _snakeMovement;
    [SerializeField] private GameObject[] _controllers;

    public Vector2 TouchPoint { get; private set; }
    public Vector2 Direction { get; set; }

    public bool isTouch { get; private set; }

    private void Awake()
    {
        if (Controller.Instance != null)
            _controllers[(int)Controller.Instance.Type].SetActive(true);
        else
            _controllers[0].SetActive(true);
    }

    private void OnEnable() => Controller.Instance.TypeChanged += ChangeController;
    private void OnDisable() => Controller.Instance.TypeChanged -= ChangeController;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            Direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            Direction = Vector2.right;
        }
    }

    public void ChangeController()
    {
        foreach (var controller in _controllers)
        {
            controller.SetActive(false);
        }
        _controllers[(int)Controller.Instance.Type].SetActive(true);
    }

    public void MoveHorizontal(int direction)
    {
        Direction = new Vector2(direction, 0f);
    }

    public void MoveVertical(int direction)
    {
        Direction = new Vector2(0f, direction);
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
