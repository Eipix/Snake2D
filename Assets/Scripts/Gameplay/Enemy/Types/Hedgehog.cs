using UnityEngine;
using UnityEngine.Events;

public class Hedgehog : Enemy
{
    public UnityEvent HedgehogDestroyed;

    protected override void Start()
    {
        base.Start();
        Behaviour = BehaviourType.Aggressive;
        AddTargetExceptions(typeof(RottenApple));
    }

    protected override void Update()
    {
        if (References.CountdownToStart.LevelStarted == false)
            return;

        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CheeseEffect effect))
        {
            References.Conditions.DistractHedgehogs++;
            effect.Eating(this);
        }

        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if ((apple is RottenApple) == false)
                apple.Deactivate();
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        SaveSerial.Instance.Increment(1, SaveSerial.JsonPaths.DestroyedHedgehogs);
        HedgehogDestroyed?.Invoke();
    }

    protected override void OnPoisoning()
    {
        base.OnPoisoning();
        References.Conditions.PoisonHedgehogs++;
    }

    protected override void OnFreeze()
    {
        base.OnFreeze();
        References.Conditions.FreezeHedgehogs++;
    }
}
