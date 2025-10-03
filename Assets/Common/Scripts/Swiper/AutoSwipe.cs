using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace Eipix.UI
{
    [RequireComponent(typeof(Swiper))]
    public class AutoSwipe : MonoBehaviour
    {
        [SerializeField] private Direction _direction;
        [SerializeField] private float _cooldown = 5f;

        private Dictionary<Direction, Action> _directions = new Dictionary<Direction, Action>();
        private Swiper _swiper;
        private Sequence _autoSwiping;

        public enum Direction
        {
            Forward,
            Back
        }

        private void Awake()
        {
            _directions.Add(Direction.Forward, () => _swiper.MoveForward());
            _directions.Add(Direction.Back, () => _swiper.MoveBack());

            _swiper = GetComponent<Swiper>();
        }

        private void OnDisable() => _autoSwiping.Kill(true);

        private void Start()
        {
            _autoSwiping = DOTween.Sequence()
                .AppendInterval(_cooldown)
                .AppendCallback(() => _directions[_direction].Invoke())
                .SetLoops(-1);
        }
    }
}
