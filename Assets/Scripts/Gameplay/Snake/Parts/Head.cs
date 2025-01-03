using UnityEngine;

public class Head : Snake
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Effects _effects;
    public bool IsAppleEaten { get; private set; }

    private Coroutine _poisoning;

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
                _additionApples.AddApple();
                _additionApples.MoveToBar(redApple);
            }
            else if (collision.gameObject.TryGetComponent(out GoldApple goldApple))
            {
                IsAppleEaten = true;
                _additionApples.AddGoldApple();
                _additionApples.MoveToBar(goldApple);
            }
            apple.Deactivate();
        }

        if (collision.collider.TryGetComponent(out Cherry cherry))
        {
            _effects.Healing();
            cherry.gameObject.SetActive(false);
        }
    }

    public void StopPoisoning()
    {
        if (_poisoning != null)
            StopCoroutine(_poisoning);      
    }
    
    public void AppleEatenComplete() =>  IsAppleEaten = false;
}
