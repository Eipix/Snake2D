using UnityEngine;

public class ResetData : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;

    private void OnEnable()
    {
        _saveSerial.ResetData();
    }
}
