using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshPlus.Components;

public class SnakeMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshSurface[] _surfaces;
    [SerializeField] private SnakeInputController _snakeInputController;
    [SerializeField] private SnakeCollision _snakeCollisions;
    [SerializeField] private PoolObjects _pool;

    [Header("Prefabs")]
    [SerializeField] private Body _prefabBody;
    [SerializeField] private RightBody _prefabRightCorner;
    [SerializeField] private LeftBody _prefabLeftCorner;

    [field:Space][field:Range(0, 10)]
    [field:SerializeField] public float Speed { get; set; }
    public Head Head { get; private set; }
    public Tail Tail { get; private set; }

    private List<Body> _bodys = new List<Body>();
    private List<SpriteRenderer> _sprites = new List<SpriteRenderer>();
    private SpriteRenderer _headImage;
    private Collider2D _collision;

    private Dictionary<float, Vector2> _headDirections = new Dictionary<float, Vector2>()
    {
        {0f, Vector2.right },
        {180f, Vector2.left },
        {90f, Vector2.up },
        {-90f, Vector2.down }
    };
    private Dictionary<Vector2, MoveRule> _moveRules = new Dictionary<Vector2, MoveRule>()
    {
        {Vector2.right, new MoveRule(new Vector3(MoveStep, 0), Vector2.up, 90f, -90f, 0) },
        {Vector2.left, new MoveRule(new Vector3(-MoveStep, 0), Vector2.down, -90f, 90f, 180) },
        {Vector2.up, new MoveRule(new Vector3(0, MoveStep), Vector2.left, 180f, 0f, 90) },
        {Vector2.down, new MoveRule(new Vector3(0, -MoveStep), Vector2.right, 0f, 180f, -90) }
    };

    private RaycastHit2D[] _hits = new RaycastHit2D[1];
    private ContactFilter2D _filter;
    private Vector2 _swipeDirection;
    private Vector2 _currentDirection;

    public int DistanceTraveled { get; private set; }
    private float _headRot;
    private float _timer = 0;

    public const float DefaultSpeed = 4f;
    public const float MoveStep = 0.7f;

    private void Start()
    {
        Head = GetComponentInChildren<Head>();  
        Tail = GetComponentInChildren<Tail>();
        _headImage = Head.GetComponent<SpriteRenderer>();
        _collision = Head.GetComponent<Collider2D>();

        var bodys = GetComponentsInChildren<Body>();
        var sprites = GetComponentsInChildren<SpriteRenderer>();

        _bodys.AddRange(bodys);
        _sprites.AddRange(sprites);

        _swipeDirection = Vector2.right;
        _currentDirection = Vector2.right;

        _filter.useTriggers = false;
    }

    private void Update()
    {
        if (_timer * Speed > 1)
        {
            SwipeDetection(_snakeInputController.GetDeltaX(), _snakeInputController.GetDeltaY());
            MoveBody(_moveRules[_swipeDirection]);

            foreach (var surface in _surfaces)
            {
                surface.UpdateNavMesh(surface.navMeshData);
            }

            _timer = 0;
            DistanceTraveled++;
        }
        _timer += Time.deltaTime;
    }
    
    public void SwipeDetection(float DeltaX, float DeltaY)
    {

        if (Mathf.Abs(DeltaX) > Mathf.Abs(DeltaY))
        {
            if (DeltaX > 0 && _headRot != 180)
            {
                _swipeDirection = Vector2.right;
            }
            else if (DeltaX < 0 && _headRot != 0)
            {
                _swipeDirection = Vector2.left;
            }
        }
        else
        {
            if (DeltaY > 0 && _headRot != -90)
            {
                _swipeDirection = Vector2.up;
            }
            else if (DeltaY < 0 && _headRot != 90 )
            {
                _swipeDirection = Vector2.down;
            }
        }
    }

    public void MoveBody(MoveRule rule)
    {
        Body newBody;
        _filter.SetLayerMask(~LayerMask.GetMask(LayerMask.LayerToName(8), LayerMask.LayerToName(7)));
        int collision = _collision.Cast(rule.Step, _filter, _hits, MoveStep);

        if (collision > 0)
        {
            _snakeCollisions.OnCollision(_hits[0].collider);
            foreach (var rotation in _headDirections)
            {
                if (rotation.Key == _headRot)
                    _snakeInputController.ChangeDirection(rotation.Value);
            }
            return;
        }

        if (_swipeDirection != _currentDirection)
        {
            if (_currentDirection == rule.Direction)
                newBody = _pool.Take(_prefabRightCorner, Head.transform.position, Quaternion.Euler(0, 0, rule.AngleRightBody), transform, _headImage.color);
            else
                newBody = _pool.Take(_prefabLeftCorner, Head.transform.position, Quaternion.Euler(0, 0, rule.AngleLeftBody), transform, _headImage.color);
        }
        else
        {
            newBody = _pool.Take(_prefabBody, Head.transform.position, Head.transform.rotation, transform, _headImage.color);
        }

        _sprites.Add(newBody.GetComponent<SpriteRenderer>());
        _bodys.Add(newBody);

        _headRot = rule.HeadRotation;
        Head.transform.rotation = Quaternion.Euler(0, 0, _headRot);
        Head.transform.position += rule.Step;
        _currentDirection = _swipeDirection;

        MoveTail(collision);
    }
    
    private void MoveTail(int collissionsCount)
    {
        if (!Head.IsAppleEaten && collissionsCount < 1)
        {
            Tail.transform.rotation = _bodys.Count > 1 ? _bodys[1].transform.rotation : Head.transform.rotation;
            Tail.transform.position = _bodys[0].transform.position;

            _pool.Put(_bodys[0]);
            _bodys.RemoveAt(0);
        }
        else
        {
            Head.AppleEatenComplete();
        }      
    }


    public IEnumerator HitAnimation()
    {
        for (int j = 0; j < 5; j++)
        {
            for (int i = 0; i < _sprites.Count; i++)
            {
                _sprites[i].color = new Color(_sprites[i].color.r, _sprites[i].color.g, _sprites[i].color.b, 0);
            }
            yield return new WaitForSeconds(0.05f);
            for (int i = 0; i < _sprites.Count; i++)
            {
                _sprites[i].color = new Color(_sprites[i].color.r, _sprites[i].color.g, _sprites[i].color.b, 255);
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ChangeColor(float r, float g, float b)
    {
        for (int i = 0; i < _sprites.Count; i++)
        {
            _sprites[i].color = new Color(r, g, b, _sprites[i].color.a);
        }
    }

    public void ChangeSpeedMovement(float increase) => Speed += increase;
}

public class MoveRule
{
    public Vector3 Step { get; private set; }
    public Vector2 Direction { get; private set; }
    public float AngleRightBody { get; private set; }
    public float AngleLeftBody { get; private set; }
    public float HeadRotation { get; private set; }

    public MoveRule(Vector3 step, Vector2 direction, float angleRight, float angleLeft, float headRotation)
    {
        (Step, Direction, AngleRightBody, AngleLeftBody, HeadRotation) =
        (step, direction, angleRight, angleLeft, headRotation);
    }
}