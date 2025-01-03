using System.Collections;
using UnityEngine;

public class UIAppleSpawner : MonoBehaviour
{
    [SerializeField] private UIApple[] _apples;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private MainMenuButtons _menu;

    private void Awake()
    {
        foreach (var apple in _apples)
        {
            apple.Init(_wallet, _menu, this);
        }
    }

    private void Start()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        _apples = _apples.Shuffled();
        int count = Random.Range(1, _apples.Length + 1);

        yield return new WaitForSeconds(60);

        for (int i = 0; i < count; i++)
        {
            _apples[i].gameObject.SetActive(true);
        }
    }

    public void RespawnCoroutine()
    {
        StartCoroutine(Respawn());
    }

    private void DisableAll()
    {
        foreach (var apple in _apples)
        {
            apple.gameObject.SetActive(false);
        }
    }
}
