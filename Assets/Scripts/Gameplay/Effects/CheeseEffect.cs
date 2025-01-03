using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CheeseEffect : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;

    private SpriteRenderer _renderer;
    private RottenApple _rotApple;

    public bool IsRotten { get; set; }

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.TryGetComponent(out RottenApple rotApple))
        {
            _rotApple = rotApple;
        }
    }

    public void Eating(Enemy enemy)
    {
        if (_rotApple != null)
        {
            enemy.Poisoning();
            _rotApple.gameObject.SetActive(false);
        }
        Destroy(gameObject);
    }

    public void TurnOn()
    {
        _renderer.sortingOrder = -5;
        _collider.enabled = true; 
    }
}
