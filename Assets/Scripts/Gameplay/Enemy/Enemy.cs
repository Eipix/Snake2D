using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour, IHealth, IDamageable
{
    [Header("References")]
    [SerializeField] private Health _snakeHealth;
    [SerializeField] private Head _snakeHead;
    [SerializeField] private GenerationArea _area;
    [field: SerializeField] protected EnemySpawner Spawner { get; private set; }
    [field:SerializeField] protected ConditionsForLevel Conditions { get; private set; }

    [field:Header("Characteristics")]
    [field:SerializeField] public int Health { get; set; }
    [field: SerializeField] public int Damage { get; set; }
    [field:SerializeField] public Rank Tier { get; set; }
    [SerializeField] private float _rotationSpeed;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _freezing;

    public Animator Animator { get; private set; }
    public NavMeshAgent Agent => _agent;
    public BehaviourType Behaviour { get; protected set; }
    [field: SerializeField] public int IsMoveBlock { get; set; }
    public Transform Target { get; private set; }

    private List<Transform> _targets = new List<Transform>();
    private List<Type> _exceptions = new List<Type>();

    private Rigidbody2D _body;
    private NavMeshAgent _agent;
    private Component _exception;

    private int _defaultHealth;
    private float _timer = 0f;

    protected virtual void Start()
    {
        Animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody2D>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _defaultHealth = Health;
    }

    public void Init(EnemyReferences references)
    {
        _snakeHealth = references.SnakeHealth;
        _snakeHead = references.SnakeHead;
        _area = references.Area;
        Spawner = references.Spawner;
        Conditions = references.Conditions;
    }

    protected virtual void Update()
    {
        if (Convert.ToBoolean(IsMoveBlock))
        {
            _agent.SetDestination(transform.position);
            return;
        }

        if (Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationController.Move) ||
            Animator.GetCurrentAnimatorStateInfo(0).IsName(AnimationController.Rotten))
        {
            FindClosest();
            LookAtTarget(Time.deltaTime * _rotationSpeed);
            _agent.SetDestination(Target.position);
            _targets.Clear();

            if (_body.IsSleeping())
                _body.WakeUp();
        }
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
        
        Vector2 direction = _agent.steeringTarget - transform.position;
        float signedAngle = Vector2.SignedAngle(Vector2.up, direction);
        Quaternion rotation = Quaternion.Euler(0, 0, signedAngle);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, time);
    }

    private void GetTargets()
    {
        var cheese = _area.GetComponentInChildren<CheeseEffect>();
        List<Apple> apples = _area.GetApples().ToList();

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
        _targets.Add(_snakeHead.transform);
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
            Animator.SetTrigger(AnimationController.Death);
        else
            Animator.SetTrigger(AnimationController.Move);
    }

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

    public Vector2 GetSnakePosition() => _snakeHead.transform.position;

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
    public virtual void Poisoning() => Animator.SetTrigger(AnimationController.Rotten);

    
    //event in "Rotten" Animation
    public void RemoveHealth(int damage = 1)
    {
        Health -= damage;
        CheckDeath();
    }

    //event in "Death" animation
    public void Death() =>  Spawner.OnDeath(this);

    //event in "Attack" animation
    public void Attack() => _snakeHealth.Lost(Damage);

    public void SkillAttack(int damage) => _snakeHealth.Lost(damage);

    // events in "Freeze" animation
    public void FreezingEnable()
    {
        _freezing.gameObject.SetActive(true);
        _freezing.Play();
    }
    public void FreezingDisable()
    {
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
    [field: SerializeField] public ConditionsForLevel Conditions { get; private set; }
    [field: SerializeField] public EnemySpawner Spawner { get; private set; }
}
