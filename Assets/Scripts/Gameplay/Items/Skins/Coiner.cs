using System.Linq;
using UnityEngine;

public class Coiner : Skin
{
    [SerializeField] private Animator _turningIntoACoin;
    private Enemy[] _enemies;
    private readonly int _defaultChance = 5;

    protected override void OnInit()
    {
        base.OnInit();
        References.Generator.AddGoldApple();
        References.Effects.IncreaseGoldAppleChance(_defaultChance + SkillLevel);
    }

    public override void SkillActivation()
    {
        _enemies = CollectSimpleEnemies();

        if (_enemies.Length < 1)
            return;

        TurnEnemyIntoAGoldApple();
    }

    private void TurnEnemyIntoAGoldApple()
    {
        var randomEnemy = _enemies[Random.Range(0, _enemies.Length)];

        TriggerAnimationIn(_turningIntoACoin, randomEnemy.transform.position, AnimationController.TurningIntoACoin);

        randomEnemy.Death();
        References.Generator.SpawnGoldApple(randomEnemy.transform.position);
    }

    private Enemy[] CollectSimpleEnemies()
    {
        var allEnemies = References.Effects.GetActiveEnemies();
        var simpleEnemies = allEnemies.Where(enemy => enemy.Tier == Enemy.Rank.Simple).ToArray();
        return simpleEnemies;
    }

    public static class AnimationController
    {
        public const string TurningIntoACoin = "TurningIntoACoin";
    }
}
