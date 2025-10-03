using UnityEngine;
using UnityEngine.Events;

public class Head : Snake
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Effects _effects;

    public UnityEvent Eated;

    private Coroutine _poisoning;

    public bool IsAppleEaten { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if (collision.gameObject.TryGetComponent(out RottenApple rottenApple))
            {
                _poisoning = StartCoroutine(_effects.RottenApple());
            }
            else if (collision.gameObject.TryGetComponent(out RedApple redApple))
            {
                IsAppleEaten = true;
                _additionApples.AddRedApple();
                _additionApples.MoveToBar(redApple);
            }
            else if (collision.gameObject.TryGetComponent(out GoldApple goldApple))
            {
                IsAppleEaten = true;
                _additionApples.AddGoldApple();
                _additionApples.MoveToBar(goldApple);
            }
            apple.Deactivate();
            Eated?.Invoke();
        }

        if (collision.collider.TryGetComponent(out Cherry cherry))
        {
            _effects.Healing();
            cherry.gameObject.SetActive(false);
            Eated?.Invoke();
        }
    }

    public void StopPoisoning()
    {
        if (_poisoning != null)
            StopCoroutine(_poisoning);      
    }
    
    public void AppleEatenComplete() =>  IsAppleEaten = false;
}
