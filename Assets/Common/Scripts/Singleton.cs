using UnityEngine;
using UnityEngine.Events;

public abstract class Singleton<Tthis> : MonoBehaviour where Tthis: class
{
    public static Tthis Instance { get; protected set; }
    public static event UnityAction Inited;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = gameObject.GetComponent<Tthis>();
            transform.SetParent(null, true);
            Inited?.Invoke();
            OnInit();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnInit() { }
}
