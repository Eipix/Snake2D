using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [field: Tooltip("~380x380")]
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }

    public abstract void Add(int count = 1);
}
