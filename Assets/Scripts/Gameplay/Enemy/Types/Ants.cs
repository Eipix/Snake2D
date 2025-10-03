using UnityEngine;

public class Ants : Enemy
{
    protected override void Start()
    {
        base.Start();
        Behaviour = BehaviourType.Peaceful;
        AddTargetExceptions(typeof(Head));
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
            if (apple is RottenApple) TakeDamage();
            else Animator.SetTrigger(AnimationController.Eating);

            apple.Deactivate();
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        References.Conditions.KillAnts++;
    }

    public static new class AnimationController 
    {
        public static string Eating = "Eating";
    }
}
