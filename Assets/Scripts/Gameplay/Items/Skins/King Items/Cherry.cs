using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Cherry : TreeProduct
{
    private SpriteRenderer _renderer;
    private Collider2D _collider;

    public override void Init(SkinReferences references, King king)
    {
        base.Init(references, king);
        _renderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _collider.enabled = false;
    }

    public void OnTheGround()
    {
        _renderer.sortingOrder = -5;
        _collider.enabled = true;
    }

    public void OnTheSky()
    {
        _renderer.sortingOrder = -2;
        _collider.enabled = false;
    }

    public override Vector2 GetTargetPosition(Vector3 offset = default) => Area.GetTargetPosition(offset);
}
