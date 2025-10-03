using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public abstract class Enemy : MonoBehaviour, IHealth, IDamageable
{
    [field: SerializeField] public SpriteRenderer Renderer { get; private set; }
    [SerializeField] private ParticleSystem _freezing;

    [field: Header("Characteristics")]
    [field: SerializeField] public int Health { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field: SerializeField] public Rank Tier { get; set; }
    [SerializeField] private float _rotationSpeed;

    private List<Transform> _targets = new List<Transform>();
    private List<Type> _exceptions = new List<Type>();

    private Rigidbody2D _body;
    private Component _exception;

    public EnemyReferences References { get; private set; }
    public Animator Animator { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public BehaviourType Behaviour { get; protected set; }
    public Transform Target { get; private set; }

    private int _defaultHealth;
    private float _timer = 0f;
    public int IsMoveBlock { get; set; }

    protected virtual void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();

        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        _defaultHealth = Health;
    }

    public void Init(EnemyReferences references) => References = references;

    protected virtual void Update()
    {
        if (Convert.ToBoolean(IsMoveBlock))
        {
            Agent.SetDestination(transform.position);
            return;
        }

        FindClosest();
        LookAtTarget(Time.deltaTime * _rotationSpeed);
        Agent.SetDestination(Target.position);
        _targets.Clear();

        if (_body.IsSleeping())
            _body.WakeUp();

        _timer += Time.deltaTime;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out CheeseEffect effect))
        {
            effect.Eating(this);
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (Behaviour == BehaviourType.Peaceful)
            return;

        if (_timer > 1.5f)
        {
            if (collision.gameObject.TryGetComponent(out Head head))
            {
                ApplyDamage();
                _timer = 0f;
            }
        }
    }

    protected virtual void ApplyDamage()
    {
        Animator.SetTrigger(AnimationController.Attack);
    }

    public void LookAtTarget(float time = 1)
    {
        Vector2 direction = (Agent.steeringTarget - transform.position).normalized;
        float signedAngle = Vector2.SignedAngle(Vector2.up, direction);
        var rotation = Quaternion.Euler(0, 0, signedAngle);
        var result = Quaternion.Lerp(transform.rotation, rotation, time);
        transform.rotation = result;
    }

    private void GetTargets()
    {
        var cheese = References.Area.GetComponentInChildren<CheeseEffect>();
        List<Apple> apples = References.Area.GetApples().ToList();

        if (cheese != null)
        {
            _targets.Add(cheese.transform);
            return;
        }

        if (apples.Count < 2) return;

        foreach (var target in apples)
        {
            _targets.Add(target.transform);
        }
        _targets.Add(References.SnakeHead.transform);
        return;
    }

    private void FilteredTargets()
    {
        List<Transform> exceptions = new List<Transform>();
        foreach (var target in _targets)
        {
            foreach (var exception in _exceptions)
            {
                _exception = target.GetComponent(exception);
                if (_exception != null && target.TryGetComponent(out _exception))
                {
                    exceptions.Add(target);
                }
            }
        }

        for (int i = 0; i < exceptions.Count; i++)
        {
            _targets.Remove(exceptions[i]);
        }
    }

    private void FindClosest()
    {
        GetTargets();
        FilteredTargets();

        float closestDistance = Mathf.Infinity;
        foreach (var target in _targets)
        {
            float targetDistance = (transform.position - target.transform.position).sqrMagnitude;
            if (targetDistance < closestDistance)
            {
                closestDistance = targetDistance;
                Target = target.transform;
            }
        }
    }

    public void AddTargetExceptions(params Type[] exceptions)
    {
        _exceptions.AddRange(exceptions);
    }

    public virtual void TakeDamage(int damage = 1)
    {
        Animator.SetTrigger(AnimationController.Damage);
        Health -= damage;
        CheckDeath();
    }

    public virtual void CheckDeath()
    {
        if (Health < 1)
        {
            Animator.SetTrigger(AnimationController.Death);
            OnDeath();
        }
        else
        {
            Animator.SetTrigger(AnimationController.Move);
        }
    }

    public virtual void OnDeath() { }

    public void Revive()
    {
        if (_defaultHealth > 0)
            Health = _defaultHealth;

        if (Animator == null)
            return;

        ResetAttackTriggers();
        if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationController.Death))
            Animator.SetTrigger(AnimationController.Revive);
    }

    public Vector2 GetSnakePosition() => References.SnakeHead.transform.position;

    public virtual Vector2 GetSpawnPosition(SpriteRenderer borders, float offset = 1.5f)
    {
        Vector2 center = borders.bounds.center;
        Vector2 size = borders.bounds.size;

        Vector2 leftEdge = new Vector2(center.x - size.x / 2 - offset, 0);
        Vector2 rightEdge = new Vector2(center.x + size.x / 2 + offset, 0);
        Vector2 topEdge = new Vector2(0, center.y + size.y / 2 + offset);
        Vector2 bottomEdge = new Vector2(0, center.y - size.y / 2 - offset);

        leftEdge.y += UnityEngine.Random.Range(bottomEdge.y, topEdge.y);
        rightEdge.y += leftEdge.y;
        topEdge.x += UnityEngine.Random.Range(leftEdge.x, rightEdge.x);
        bottomEdge.x += topEdge.x;

        Vector2[] edges = new Vector2[4] { leftEdge, rightEdge, topEdge, bottomEdge };

        var result = Tier == Rank.Simple
            ? edges[UnityEngine.Random.Range(0, edges.Length)]
            : Vector2.zero;

        return result;
    }

    public virtual void ResetAttackTriggers()
    {
        if (Behaviour == BehaviourType.Aggressive)
        {
            Animator.ResetTrigger(AnimationController.Attack);
        }
    }
    public virtual void Poisoning()
    {
        Animator.SetTrigger(AnimationController.Rotten);
        OnPoisoning();
    }

    protected virtual void OnPoisoning() { }
    protected virtual void OnFreeze() { }

    //event in "Rotten" Animation
    public void RemoveHealth(int damage = 1)
    {
        Health -= damage;
        CheckDeath();
    }

    //event in "Death" animation
    public void Death() => References.Spawner.OnDeath(this);

    //event in "Attack" animation
    public void Attack() => References.SnakeHealth.Lost(Damage);

    public void SkillAttack(int damage) => References.SnakeHealth.Lost(damage);

    // events in "Freeze" animation
    public void FreezingEnable()
    {
        Animator.SetBool(AnimationController.Freezing, true);
        _freezing.gameObject.SetActive(true);
        _freezing.Play();
        OnFreeze();
    }
    public void FreezingDisable()
    {
        Animator.SetBool(AnimationController.Freezing, false);
        _freezing.Stop();
        _freezing.gameObject.SetActive(false);
    }

    public static class AnimationController
    {
        public const string Move = "Move";
        public const string Attack = "Attack";
        public const string Damage = "Damage";
        public const string Freezing = "Freezing";
        public const string Rotten = "Rotten";
        public const string Death = "Death";
        public const string Revive = "Revive";
    }

    public enum BehaviourType
    {
        Aggressive,
        Peaceful
    }

    public enum Rank
    {
        Simple,
        HalfBoss,
        Boss
    }
}

[Serializable]
public class EnemyReferences
{
    [field: SerializeField] public Health SnakeHealth { get; private set; }
    [field: SerializeField] public Head SnakeHead { get; private set; }
    [field: SerializeField] public GenerationArea Area { get; private set; }
    [field: SerializeField] public CountdownToStart CountdownToStart { get; private set; }
    [field: SerializeField] public ConditionsForLevel Conditions { get; private set; }
    [field: SerializeField] public EnemySpawner Spawner { get; private set; }
    [field: SerializeField] public EnemyPool Pool { get; private set; }
}
