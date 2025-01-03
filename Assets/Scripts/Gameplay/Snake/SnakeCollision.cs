using UnityEngine;

public class SnakeCollision : MonoBehaviour
{
    [SerializeField] private SnakeMovement _snakeMovement;
    [SerializeField] private Health _health;
    [SerializeField] private SaveSerial _saveSerial;

    public int StoneCollisionCount { get; private set; }

    private float _time = 0f;

    private void Update() => _time += Time.deltaTime;

    public void OnCollision(Collider2D collision)
    {
        if (TryGetCollision(collision))
        {
            if (_time > 1.5f)
            {
                if (collision.TryGetComponent(out Stone stones))
                    StoneCollisionCount++;

                if (collision.TryGetComponent(out Enemy enemys))
                    return;

                _health.Lost();
                _time = 0;
            }
        }
    }

    public bool TryGetCollision(Collider2D collision)
    {
        return collision.TryGetComponent(out MapBorders fence) || collision.TryGetComponent(out Stone stone) ||
               collision.TryGetComponent(out Body body) || collision.TryGetComponent(out Tail tail) ||
               collision.TryGetComponent(out Enemy enemy);
    }
}
