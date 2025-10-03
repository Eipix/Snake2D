using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private SpriteRenderer _borders;
    [SerializeField] private WaveChanger _arena;

    private System.Random _rand = new System.Random();
    private List<Enemy> _activeEnemys = new List<Enemy>();

    private LevelData _data;
    public int Count => _activeEnemys.Count;

    private void Awake() => _data = SaveSerial.Instance.Data;

    private void Start()
    {
        if (SaveSerial.Instance.Mode == LevelMode.Arena)
            _arena.gameObject.SetActive(true);
        else
            SpawnToTheLimit();
    }

    public void SpawnToTheLimit()
    {
        if (_data.Bosses.Length > 0)
            SpawnEnemy(_data.Bosses[0].EnemyPrefab);

        while (CanSpawn())
        {
            if (_data.EnemyData.Length < 1)
                break;

            SpawnEnemyByChances();
        }
    }

    public void OnDeath(Enemy thisEnemy)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(false);
        sequence.AppendCallback(() => { 
            _enemyPool.Put(thisEnemy);
            _activeEnemys.Remove(thisEnemy);
        });
        sequence.AppendInterval(_data.DelayBeforeEnemySpawn);
        sequence.AppendCallback(() => SpawnEnemyByChances());
    }

    public void SpawnEnemy(Enemy prefab)
    {
        if (CanSpawn() == false || prefab == null)
            return;

        var position = prefab.GetSpawnPosition(_borders);
        var enemy = _enemyPool.Take(prefab.GetType(), transform, position, Quaternion.identity);
        _activeEnemys.Add(enemy);
    }

    public bool CanSpawn() => _activeEnemys.Count < _data.MaxEnemyCount;

    public IEnumerable<Enemy> GetActiveEnemies() => _activeEnemys;

    private void SpawnEnemyByChances()
    {
        Dictionary<Enemy, int> enemyChances = new Dictionary<Enemy, int>();
        foreach (var data in _data.EnemyData)
            enemyChances.Add(data.EnemyPrefab, data.SpawnChances);

        var enemy = _rand.Element(enemyChances);
        SpawnEnemy(enemy);
    }
}
