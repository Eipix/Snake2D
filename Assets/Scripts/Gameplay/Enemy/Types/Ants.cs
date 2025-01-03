using UnityEngine;

public class Ants : Enemy
{
    private float _delay = 0f;

    protected override void Start()
    {
        base.Start();
        Behaviour = BehaviourType.Peaceful;
        AddTargetExceptions(typeof(Head));
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
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if (apple is RottenApple) TakeDamage();
            else Animator.SetTrigger(AnimationController.Eating);

            apple.Deactivate();
        }
    }

    public void IncreaseConditionDeath()
    {
        Conditions.KillAnts++;
    }

    public static new class AnimationController 
    {
        public static string Eating = "Eating";
    }
}
