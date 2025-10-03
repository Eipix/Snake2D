using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public abstract class TreeProduct : MonoBehaviour
{
    public Effects Effects { get; private set; }
    public GenerationArea Area { get; private set; }
    public EnemySpawner EnemySpawner { get; private set; }
    public King King { get; private set; }

    public virtual void Init(SkinReferences references, King king)
    {
        Effects = references.Effects;
        Area = references.Generator;
        EnemySpawner = references.EnemySpawner;
    }

    public abstract Vector2 GetTargetPosition(Vector3 offset = default);
}
