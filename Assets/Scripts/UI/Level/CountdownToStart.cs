using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownToStart : MonoBehaviour
{
    [SerializeField] private SnakeMovement _movement;
    [SerializeField] private GameObject _countdown;
    private TMP_Text _countdownText;

    private void Start()
    {
        _countdownText = _countdown.GetComponentInChildren<TMP_Text>();
        _countdown.SetActive(true);
    }

    public IEnumerator CountdownStart()
    {
        float speed = _movement.Speed;
        _movement.ChangeSpeedMovement(-speed);
        _countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        _countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        _countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        _countdown.SetActive(false);
        _movement.ChangeSpeedMovement(speed);
    }
}
