using UnityEngine;

public class TabController : MonoBehaviour
{
    protected Tab Enabled { get; set; }

    protected virtual void Awake() => Enabled = GetComponentInChildren<Tab>();

    protected virtual void Start() => Enabled.On();

    public virtual void SetEnabled(Tab tab)
    {
        if (Enabled == tab)
            return;

        Enabled.Off();
        Enabled = tab;
    }
}
