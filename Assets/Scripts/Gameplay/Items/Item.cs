using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [field: SerializeField] public TranslatableString LangName { get; private set; }
    [field: Tooltip("~380x380")]
    [field:SerializeField] public Sprite Sprite { get; private set; }
    [field: SerializeField] public Sprite Shadowed { get; private set; }
    [field: SerializeField] public Color Color { get; private set; }

    public abstract void Add(int count = 1, bool updateBalance = true);
}
