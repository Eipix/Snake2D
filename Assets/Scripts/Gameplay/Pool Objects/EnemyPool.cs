using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private EnemyReferences _enemyReferences;
    [SerializeField] private EnemyData[] _allEnemyPrefabs;

    private List<Enemy> _allEnemys = new List<Enemy>();

    private void Awake()
    {
        if (_saveSerial.ArenaMode)
        {
            for (int i = 0; i < _saveSerial.Waves[_saveSerial.Waves.Length - 1].MaxEnemyCount; i++)
            {
                InstantiateEnemies(_allEnemyPrefabs);
            }
            return;
        }

        for (int i = 0; i < _saveSerial.Data.MaxEnemyCount; i++)
        {
            InstantiateEnemies(_saveSerial.Data.EnemyData);
        }
        InstantiateEnemies(_saveSerial.Data.Bosses);
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
        enemy.gameObject.SetActive(false);
        _allEnemys.Add(enemy);
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
            var enemyPrefab = Instantiate(data.EnemyPrefab.gameObject, transform.position, Quaternion.identity, transform);
            enemyPrefab.SetActive(false);

            var enemy = enemyPrefab.GetComponent<Enemy>();
            enemy.Init(_enemyReferences);

            _allEnemys.Add(enemy);
        }
    }
}