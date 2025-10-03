using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Counter : MonoBehaviour
{
    [field:SerializeField] public Image Circle { get; protected set; }
    [field:SerializeField] public TextMeshProUGUI Count { get; protected set; }

    protected virtual void Awake() => UpdateCount();

    protected abstract void OnEnable();
    protected abstract void OnDisable();

    public abstract void UpdateCount();
}
