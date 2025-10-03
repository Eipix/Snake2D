using UnityEngine;
using UnityEngine.EventSystems;

public class Swipes : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField] private SnakeInputController _snakeController;

    public void OnBeginDrag(PointerEventData eventData)
    {
        _snakeController.Direction = eventData.delta;
    }

    public void OnDrag(PointerEventData eventData) { }
}
