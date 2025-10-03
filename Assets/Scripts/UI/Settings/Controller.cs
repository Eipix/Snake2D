using UnityEngine;
using UnityEngine.Events;

public class Controller : Singleton<Controller>
{
    private ControllerTypes _type;

    public UnityAction TypeChanged;
    public ControllerTypes Type
    {
        get => _type;
        set
        {
            _type = value;
            TypeChanged?.Invoke();
        }
    }

    private void Start() => Type = (ControllerTypes)SaveSerial.Instance.Load<int>(SaveSerial.JsonPaths.ControllerType, 0);
}
