using UnityEngine;

public class Mouse : Enemy
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
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.TryGetComponent(out Apple apple))
        {
            if ((apple is RottenApple) == false)
                apple.Deactivate();
        }
    }
}
