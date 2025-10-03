using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Effects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SnakeInputController _controller;
    [SerializeField] private SnakeMovement _snakeMovement;
    [SerializeField] private Health _health;
    [SerializeField] private GenerationArea _generationArea;
    [SerializeField] private Head _head;
    [SerializeField] private EnemySpawner _enemySpawner;

    [Header("Sounds")]
    [SerializeField] private AudioClip _explosion;
    [SerializeField] private AudioClip _energyShield;
    [SerializeField] private AudioClip _freeze;
    [SerializeField] private AudioClip _speed;

    [Header("Animations")]
    [SerializeField] private ParticleSystem _healing;
    [SerializeField] private Transform _accelerationAnimation;
    [SerializeField] private Transform _shieldAnimation;
    [SerializeField] private Transform _goldShieldAnimation;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bombPrefab;
    [SerializeField] private GameObject _cheesePrefab;

    [Space]
    [SerializeField] private TextMeshProUGUI _drop;

    private List<Enemy> _enemies = new List<Enemy>();
    private TranslatableString _langDropText = new TranslatableString();

    public int BombUsedCount { get; private set; }
    public bool IsInvincible => _invincible;
    public bool IsFreezing => _freezing;

    private int _poisonousNumber = 0;
    private float _acceleration = 0f;
    private float _invincibleDuration = 0f;
    private float _freezeDuration = 0f;
    private float _accelerationDuration = 0f;

    private bool _invincible;
    private bool _lightning;
    private bool _slowMotion;
    private bool _heal;
    private bool _freezing;

    private float _timerInvincible = 0f;
    private float _timerLightning = 0f;
    private float _timerSlowMotion = 0f;
    private float _timerHealth = 0f;
    private float _timerFreezing = 0f;

    private void Start()
    {
        _langDropText.TranslateTexts = new string[]
        {
            "Specify the reset point",
            "Укажите точку сброса",
            "Sıfırlama noktasını belirleyin"
        };
        _drop.color = _drop.color.Fade();
    }

    private void Update()
    {
        if (_poisonousNumber == 1)
            _snakeMovement.ChangeColor(0.475f, 0.855f, 0.075f);

        if (Timer(ref _heal, ref _timerHealth, 2f))
        {
            _healing.Stop();
            _healing.gameObject.SetActive(false);
        }

        if (Timer(ref _invincible, ref _timerInvincible, _invincibleDuration))
        {
            _invincibleDuration = 0f;
            _shieldAnimation.gameObject.SetActive(false);
            _goldShieldAnimation.gameObject.SetActive(false);
        }

        if(Timer(ref _slowMotion, ref _timerSlowMotion, 10f))
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Agent.speed *= 2;
                _enemies[i].Animator.speed *= 2;
            }
        }

        if(Timer(ref _lightning, ref _timerLightning, _accelerationDuration))
        {
            _snakeMovement.Speed -= _acceleration;
            _acceleration = 0f;

            if (_snakeMovement.Speed < 8)
                _accelerationAnimation.gameObject.SetActive(false);
        }

        if(Timer(ref _freezing, ref _timerFreezing, _freezeDuration))
        {
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i].Behaviour == Enemy.BehaviourType.Aggressive)
                    _enemies[i].ResetAttackTriggers();

                _enemies[i].FreezingDisable();
            }
        }
    }

    public List<Enemy> GetActiveEnemies()
    {
        List<Enemy> activeEnemies = new List<Enemy>();
        foreach (var enemy in _enemySpawner.GetActiveEnemies())
        {
            activeEnemies.Add(enemy);
        }
        return activeEnemies;
    }

    public IEnumerator RottenApple()
    {
        if (IsInvincible == false)
        {
            _poisonousNumber += 1;
            _snakeMovement.ChangeColor(0f, 1f, 0f);

            yield return new WaitForSeconds(5f);
            _health.Lost();

            yield return new WaitForSeconds(5f);
            _health.Lost();

            _poisonousNumber -= 1;
            if (_poisonousNumber == 0)
                _snakeMovement.ChangeColor(1f, 1f, 1f);
        }
    }

    public void Healing()
    {
        _heal = true;

        _health.Recovery();
        _head.StopPoisoning();
        _snakeMovement.ChangeColor(1f, 1f, 1f);
        _poisonousNumber = 0;

        _healing.gameObject.SetActive(true);
        _healing.Play();
    }

    public void IncreaseGoldAppleChance(int increase = 5) => _generationArea.ChangeGoldAppleChance(_generationArea.GoldAppleChance + increase);

    public void Shield(float duration = 7f, bool isPeach = false)
    {
        SoundsPlayer.Instance.PlayOneShotSound(_energyShield);
        _invincible = true;
        _invincibleDuration += duration;

        _head.StopPoisoning();
        _snakeMovement.ChangeColor(1f, 1f, 1f);
        _poisonousNumber = 0;

        if (isPeach) _goldShieldAnimation.gameObject.SetActive(true);
        else _shieldAnimation.gameObject.SetActive(true);
    }

    public void SlowDownEnemy()
    {
        _slowMotion = true;
        _enemies = GetActiveEnemies();

        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].Agent.speed *= 0.5f;
            _enemies[i].Animator.speed *= 0.5f;
        }
    }

    public void Acceleration(float duration = 10f, float acceleration = 2f)
    {
        SoundsPlayer.Instance.PlayOneShotSound(_speed);

        _lightning = true;
        _accelerationDuration = duration;
        float speed = SnakeMovement.DefaultSpeed;

        _acceleration = speed * acceleration - speed;
        _snakeMovement.Speed += _acceleration;
        _accelerationAnimation.gameObject.SetActive(true);
    }

    public void Freezing(float duration = 5f)
    {
        _freezing = true;
        var audioSource = gameObject.AddComponent<AudioSource>();
        SoundsPlayer.Instance.PlayWhile(_freeze, () => _freezing, audioSource);
        _freezeDuration = duration;

        _enemies = GetActiveEnemies();
        _enemies.ForEach(enemy => enemy.FreezingEnable());
    }

    public IEnumerator DropBomb(Bomb bomb = null, int doubleDamageChance = 0)
    {
        System.Random rand = new System.Random();
        Sequence sequence = RunNotification(new Color(0.875f, 0.604f, 1f, 1f));

        yield return new WaitUntil(() => _controller.isTouch);

        BombUsedCount++;
        sequence.Kill();
        bomb?.TrySpend();

        int damage = rand.withProbability(doubleDamageChance) ? 2 : 1;
        var bombPrefab = Instantiate(_bombPrefab, new Vector2(0, 8.5f), Quaternion.identity);

        Sequence explosion = DOTween.Sequence();
        explosion.Append(bombPrefab.transform.DOLocalMove(_controller.TouchPoint, 1f));
        explosion.AppendCallback(() => Explosion(bombPrefab, damage));
        explosion.AppendInterval(0.5f);
        explosion.AppendCallback(() => Destroy(bombPrefab.gameObject));
    }

    public IEnumerator Cheese(Cheese cheese = null)
    {
        Sequence sequence = RunNotification(new Color(1f, 1f, 0.662f, 1f));

        yield return new WaitUntil(() => _controller.isTouch);

        sequence.Kill();
        cheese?.TrySpend();

        var cheesePrefab = Instantiate(_cheesePrefab, new Vector2(0, 8.5f), Quaternion.identity, _generationArea.transform);

        Sequence drop = DOTween.Sequence();
        drop.Append(cheesePrefab.transform.DOLocalMove(_controller.TouchPoint, 1f));
        drop.Join(cheesePrefab.transform.DOScale(Vector3.one * 0.5f, 1f));
        drop.AppendCallback(() => cheesePrefab.GetComponent<CheeseEffect>().TurnOn());
    }

    public void Explosion(GameObject prefab, int damage)
    {
        SoundsPlayer.Instance.PlayOneShotSound(_explosion);
        var hits = Physics2D.BoxCastAll(prefab.transform.position, Vector2.one * 3, 0, prefab.transform.position, 0.1f);

        if (hits == null) return;

        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if (hit.collider.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage(damage);
        }
    }

    public void EnableSpeedAnimation(ref bool isActive, ref float acceleration, float accelerationMultiplier)
    {
        isActive = true;
        float defSpeed = SnakeMovement.DefaultSpeed;
        acceleration = defSpeed * accelerationMultiplier - defSpeed;
        _snakeMovement.Speed += acceleration;
        _accelerationAnimation.gameObject.SetActive(true);
    }

    public void DisableSpeedAnimation(ref float acceleration)
    {
        _snakeMovement.Speed -= acceleration;
        acceleration = 0f;

        if (_snakeMovement.Speed < 8)
            _accelerationAnimation.gameObject.SetActive(false);
    }

    public bool Timer(ref bool effectActive, ref float timer, float duration)
    {
        if (effectActive)
        {
            timer += Time.deltaTime;
            if (timer > duration)
            {
                timer = 0f;
                effectActive = false;
                return true;
            }
        }
        return false;
    }
    
    private Sequence RunNotification(Color color)
    {
        _drop.text = _langDropText.Translate;
        _drop.color = color;
        Sequence sequence = DOTween.Sequence();    
            sequence.Append(_drop.DOFade(1f, 0.5f));
            sequence.Append(_drop.DOFade(0f, 0.5f));
            sequence.SetLoops(-1, LoopType.Restart);
        sequence.OnKill(() => _drop.color = _drop.color.Fade());
        return sequence;
    }
}
