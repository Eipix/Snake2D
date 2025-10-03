using UnityEngine;

public class YandexData : MonoBehaviour
{
    //Data is loading in SaveSerial.cs
    private void OnDisable()
    {
        Debug.Log("Data is Save!");
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
    }
}
