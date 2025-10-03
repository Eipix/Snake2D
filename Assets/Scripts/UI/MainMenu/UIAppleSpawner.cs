using System.Collections;
using UnityEngine;

public class UIAppleSpawner : MonoBehaviour
{
    [SerializeField] private UIApple[] _apples;

    private void Awake() => _apples.ForEach(apple => apple.Init(this));

    private void Start() => StartCoroutine(Respawn());

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

    public void RespawnCoroutine() => StartCoroutine(Respawn());
}
