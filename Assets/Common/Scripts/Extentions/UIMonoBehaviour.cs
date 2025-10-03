using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIMonoBehaviour : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }

    protected virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}
