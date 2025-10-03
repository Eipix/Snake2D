using UnityEngine;
using UnityEngine.Events;

public class Mouse : Enemy
{
    public UnityEvent MouseDestroyed;

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
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if ((apple is RottenApple) == false)
                apple.Deactivate();
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        SaveSerial.Instance.Increment(1, SaveSerial.JsonPaths.DestroyedMouses);
        References.Conditions.KillMouses++;
        MouseDestroyed?.Invoke();
    }

    protected override void OnPoisoning()
    {
        base.OnPoisoning();
        References.Conditions.PoisonMouses++;
    }
}
