using UnityEngine;
using System.Collections.Generic;

public class PineCone : TreeProduct
{
    public void Explosion() => Effects.Explosion(gameObject, 1);
    public void Disabled() => gameObject.SetActive(false);

    public override Vector2 GetTargetPosition(Vector3 offset = default)
    {
        List<Enemy> enemies = new List<Enemy>();
        foreach (var enemy in EnemySpawner.GetActiveEnemies())
        {
            enemies.Add(enemy);
        }

        if(enemies.Count > 0)
        {
            int enemyIndex = Random.Range(0, EnemySpawner.Count);
            return enemies[enemyIndex].transform.position + offset;
        }
        else
        {
            return Area.RandomSpawnPoint;
        }
    }
}
