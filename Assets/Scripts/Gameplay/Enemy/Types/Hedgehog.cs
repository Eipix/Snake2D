using UnityEngine;

public class Hedgehog : Enemy
{
    private float _delay = 0f;

    protected override void Start()
    {
        base.Start();
        Behaviour = BehaviourType.Aggressive;
        AddTargetExceptions(typeof(RottenApple));
    }

    protected override void Update()
    {
        _delay += Time.deltaTime;

        if (_delay < 3f)
            return;

        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CheeseEffect effect))
        {
            Conditions.DistractHedgehogs++;
            effect.Eating(this);
        }

        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if ((apple is RottenApple) == false)
                apple.Deactivate();
        }
    }

    public void IncreaseConditionFreeze()
    {
        Conditions.FreezeHedgehogs++;
    }
}
