using UnityEngine;
using UnityEngine.EventSystems;

namespace Eipix.UI
{
    [RequireComponent(typeof(Swiper))]
    public class Swipes : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        [SerializeField] private SwipeMethod _swipeMethod;

        private Swiper _swiper;

        private Vector2 _direction;

        public enum SwipeMethod
        {
            Horizontal,
            Vertical
        }

        private void Awake() => _swiper = GetComponent<Swiper>();

        public void OnBeginDrag(PointerEventData eventData) => _direction = eventData.delta.GetDirection();
        
        public void OnDrag(PointerEventData eventData) { }

        public void OnEndDrag(PointerEventData eventData)
        {
            switch (_swipeMethod)
            {
                case SwipeMethod.Horizontal:
                    Move(_direction.x);
                    break;
                case SwipeMethod.Vertical:
                    Move(_direction.y);
                    break;
                default:
                    break;
            }
        }

        private void Move(float axis)
        {
            if (axis > 0)
            {
                _swiper.MoveBack();
            }
            else if (axis != 0)
            {
                _swiper.MoveForward();
            }
        }
    }
}
