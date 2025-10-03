using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private EnemyReferences _enemyReferences;
    [SerializeField] private EnemyData[] _allEnemyPrefabs;

    private List<Enemy> _allEnemys = new List<Enemy>();

    private void Awake()
    {
        var waves = SaveSerial.Instance.Waves;
        var data = SaveSerial.Instance.Data;

        if (SaveSerial.Instance.Mode == LevelMode.Arena)
        {
            for (int i = 0; i < waves.Max(wave => wave.MaxEnemyCount); i++)
            {
                InstantiateEnemies(_allEnemyPrefabs);
            }
            return;
        }

        for (int i = 0; i < data.MaxEnemyCount; i++)
        {
            InstantiateEnemies(data.EnemyData);
        }
        InstantiateEnemies(data.Bosses);
    }

    public Enemy Take(Type enemyType, Transform parent, Vector2 position, Quaternion rotation)
    {
        if (TryGetEnemyByType(enemyType, out Enemy enemy))
        {
            enemy.transform.SetParent(parent);
            enemy.transform.position = position;
            enemy.transform.rotation = rotation;
            
            enemy.gameObject.SetActive(true);
            enemy.Revive();
            _allEnemys.Remove(enemy);
            return enemy;
        }
        else
        {
            throw new InvalidOperationException($"В пуле не осталось врагов типа: {enemyType}");
        }
    }

    public void Put(Enemy enemy)
    {
        enemy.transform.SetParent(transform);
        enemy.transform.position = transform.position;
        enemy.transform.rotation = Quaternion.identity;
        enemy.Renderer.color = Color.white;
        enemy.gameObject.SetActive(false);
        _allEnemys.Add(enemy);
    }

    public void AddEnemies(Enemy prefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddEnemy(prefab);
        }
    }

    private bool TryGetEnemyByType(Type type, out Enemy enemy)
    {
        enemy = _allEnemys.Where(enemy => enemy.GetType() == type).FirstOrDefault();
        return enemy != null;
    }

    private void InstantiateEnemies(EnemyData[] datas)
    {
        foreach (var data in datas)
        {
            AddEnemy(data.EnemyPrefab);
        }
    }

    private void AddEnemy(Enemy prefab)
    {
        var enemyPrefab = Instantiate(prefab.gameObject, transform.position, Quaternion.identity, transform);
        enemyPrefab.SetActive(false);
        var enemy = enemyPrefab.GetComponent<Enemy>();
        enemy.Init(_enemyReferences);

        _allEnemys.Add(enemy);
    }
}