using UnityEngine;

public class StoneSpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite _new;

    private System.Random rand = new System.Random();
    private SpriteRenderer _renderer;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();

        if (rand.withProbability(50))
            _renderer.sprite = _new;
    }
}
