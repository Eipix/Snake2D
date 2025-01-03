using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Zippo : Skin
{
    [SerializeField] private Animator _discharge;

    private bool _isActive;
    private float _duration = 10f;
    private float _timer = 0f;
    private float _accelerationMultiplier = 4f;
    private float _acceleration = 0f;
    private int _damage = 1;

    protected override void Start()
    {
        base.Start();

        if (SceneManager.GetActiveScene().name != "Level")
            return;

        DecreaseCooldown(SkillLevel);
        _discharge.transform.position = Vector2.zero;
        _discharge.transform.SetParent(_discharge.transform.root);
        MatchSpriteSize(References.Borders, _discharge.GetComponent<SpriteRenderer>());
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Level")
            return;

        if(References.Effects.Timer(ref _isActive, ref _timer, _duration))
        {
            References.Effects.DisableSpeedAnimation(ref _acceleration);
        }
    }

    public override void SkillActivation()
    {
        References.Effects.EnableSpeedAnimation(ref _isActive, ref _acceleration, _accelerationMultiplier);
        ElectricDischarge();
    }

    public void ElectricDischarge()
    {
        TriggerAnimationIn(_discharge, Vector2.zero, AnimationController.Discharge);
        var hits = Physics2D.BoxCastAll(Vector2.zero, References.Borders.bounds.size, 0f, Vector2.zero, 0.1f);

        if (hits == null) return;

        foreach (var hit in hits)
        {
            if (hit.collider.isTrigger)
                continue;

            if(hit.collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damage);
            }
        }
    }

    private void MatchSpriteSize(SpriteRenderer source, SpriteRenderer target)
    {
        Vector2 sourceSize = source.bounds.size;
        Vector2 targetSize = target.bounds.size;
        var scale = sourceSize / targetSize;
        target.transform.localScale = scale * target.transform.localScale;
    }

    public static class AnimationController
    {
        public const string Discharge = "Discharge";
    }
}
