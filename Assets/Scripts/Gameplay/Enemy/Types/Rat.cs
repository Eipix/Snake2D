using UnityEngine;
using DG.Tweening;
using System;

public class Rat : Enemy
{
    [SerializeField] private RatDashTrigger _dashTrigger;
    [SerializeField] private Mouse _mousePrefab;

    private Tween _dashMove;

    private bool _isDash;
    private float _contusionTimer = 0f;
    private float _afterDashTimer = 2f;
    private int _contusionDuration = 5;
    private bool _isInit;

    private readonly int _afterdashDelay = 2;
    private readonly int _summonCount = 2;
    private readonly int _dashDamage = 2;

    public void InitMouses()
    {
        if (_isInit) return;
        References.Pool.AddEnemies(_mousePrefab, _summonCount);
        _isInit = true;
    }

    protected override void Start()
    {
        base.Start();
        Behaviour = BehaviourType.Aggressive;
        AddTargetExceptions(typeof(RottenApple));
        _dashTrigger.Init(this);
        SummonMouses();
    }

    protected override void Update()
    {
        if (Animator.GetBool(AnimationController.Contusion) || Animator.GetBool(AnimationController.Poisoning))
            _contusionTimer += Time.deltaTime;

        if(_contusionTimer > _contusionDuration)
        {
            _contusionTimer = 0f;
            Animator.SetBool(AnimationController.Contusion, false);
            Animator.SetBool(AnimationController.Poisoning, false);
            ResetAttackTriggers();
        }

        _afterDashTimer += Time.deltaTime;

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

    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isDash)
        {
            if (collision.gameObject.TryGetComponent(out Head head))
            {
                _dashMove.Kill();
                SkillAttack(_dashDamage);
            }
            else if (IsCollision(collision))
            {
                _dashMove.Kill();
            }
        }
    }

    protected override void OnTriggerStay2D(Collider2D collision)
    {
        if (_afterDashTimer > _afterdashDelay)
        {
            base.OnTriggerStay2D(collision);
        }
        else
        {
            ResetAttackTriggers();
        }
    }

    public override void TakeDamage(int damage = 1)
    {
        Animator.SetBool(AnimationController.Contusion, true);
    }

    public override void Poisoning()
    {
        Animator.SetBool(AnimationController.Poisoning, true);
    }

    public override void ResetAttackTriggers()
    {
        base.ResetAttackTriggers();
        Animator.ResetTrigger(AnimationController.Dash);
        Animator.ResetTrigger(AnimationController.Summon);
    }

    private void SummonMouses()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(false);
        sequence.AppendInterval(10);
        sequence.AppendCallback(() =>
        {
            if (References.Spawner.CanSpawn() == false)
                return;

            InitMouses();
            Animator.SetTrigger(AnimationController.Summon);
            for (int i = 0; i < _summonCount; i++)
            {
                References.Spawner.SpawnEnemy(_mousePrefab);
            }
        });
        sequence.SetLoops(-1);
    }

    public void Dash()
    {
        LookAt();
        _dashMove = transform.DOMove(GetSnakePosition(), 0.22f);
        _isDash = true;
    }

    //invoke in animation "Dash"
    private void LookAt()
    {
        var direction = (References.SnakeHead.transform.position - transform.position).normalized;
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    public void DisableDash()
    {
        _afterDashTimer = 0f;
        _isDash = false;
    }

    private bool IsCollision(Collider2D collision)
    {
        return collision.gameObject.TryGetComponent(out Stone stone)
            || collision.gameObject.TryGetComponent(out MapBorders fence)
            || collision.gameObject.TryGetComponent(out Snake snake)
            || collision.gameObject.TryGetComponent(out Enemy enemy);
    }

    public static new class AnimationController
    {
        public static string Dash = "Dash";
        public static string Summon = "Summon";
        public static string Contusion = "Contusion";
        public static string Poisoning = "Poisoning";
    }
}
