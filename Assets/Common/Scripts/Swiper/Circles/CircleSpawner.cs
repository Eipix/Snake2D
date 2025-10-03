using UnityEngine;

namespace Eipix.UI
{
    public class CircleSpawner : MonoBehaviour
    {
        [SerializeField] private SwapCircle _circlePrefab;

        private SwapCircle[] _circles;
        private SwapCircle _currentCircle;
        private Swiper _swiper;

        private void Awake()
        {
            _swiper = transform.GetComponentInParent<Swiper>();

            if (_swiper == null)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            _circles = new SwapCircle[_swiper.Count];
            for (int i = 0; i < _circles.Length; i++)
            {
                _circles[i] = Instantiate(_circlePrefab, transform as RectTransform);
            }
            _currentCircle = _circles[0].Init().On();
        }

        private void OnEnable() => _swiper.SwipeStarted += NextCircle;

        private void OnDisable() => _swiper.SwipeStarted -= NextCircle;

        private void NextCircle()
        {
            if (_currentCircle == null)
                return;

            _currentCircle.Off();
            _currentCircle = _circles[_swiper.Index];
            _currentCircle.On();
        }
    }
}
