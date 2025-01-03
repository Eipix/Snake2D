using UnityEngine;

public class Bomber : Skin
{
    [SerializeField] private Animator _animator;

    private readonly int _defaultChance = 0;

    protected override void OnInit()
    {
        base.OnInit();
        SparksEnable();
    }

    public override void SkillActivation()
    {
        StartCoroutine(References.Effects.DropBomb(doubleDamageChance: _defaultChance + SkillLevel));
    }

    private void SparksEnable()
    {
        var targetPosition = Vector3.zero + new Vector3(-0.36f, 0.045f);
        TriggerAnimationIn(_animator, targetPosition, AnimationController.Sparks, References.SnakeTail.transform, true);
    }

    public static class AnimationController
    {
        public const string Sparks = "Sparks";
    }
}
