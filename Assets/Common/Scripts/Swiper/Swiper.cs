using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class Swiper : MonoBehaviour
{
    [SerializeField] private RectTransform _content;
    [SerializeField] private Button _backArrow;
    [SerializeField] private Button _forwardArrow;

    [SerializeField] private Vector2 _targetPosition;
    [SerializeField] private Vector2 _beforeMovePosition;

    public event UnityAction SwipeStarted;

    private RectTransform[] _childs;
    private RectTransform _current;
    private Sequence _swap;

    public Vector2 BeforeMovePosition => _beforeMovePosition;

    public int Count => _childs.Length;
    public int Index { get; protected set; } = 0;

    protected virtual void Awake()
    {
        _childs = new RectTransform[_content.childCount];
        int k = 0;
        foreach (var child in _content.transform)
        {
            var transformChild = child as RectTransform;
            _childs[k] = transformChild;
            k++;
        }

        _current = _childs[0];
        for (int i = 1; i < _childs.Length; i++)
        {
            _childs[i].anchoredPosition = _beforeMovePosition;
        }
    }
    
    protected virtual void OnEnable() { }

    protected virtual void Start()
    {
        _backArrow.onClick.AddListener(MoveBack);
        _forwardArrow.onClick.AddListener(MoveForward);
    }

    public void MoveForward() => StartCoroutine(MoveForwardCoroutine());

    public void MoveBack() => StartCoroutine(MoveBackCoroutine());

    protected virtual void BeforeMove() { }

    private IEnumerator MoveForwardCoroutine(float time = 0.3f)
    {
        _swap.CompleteIfActive(true);

        Index++;
        if (Index >= _childs.Length)
            Index = 0;

        yield return Move(_beforeMovePosition, time);
    }

    private IEnumerator MoveBackCoroutine(float time = 0.3f)
    {
        _swap.CompleteIfActive(true);

        Index--;
        if (Index < 0)
            Index = _childs.Length - 1;

        yield return Move(new Vector2(-_beforeMovePosition.x, _beforeMovePosition.y), time);
    }

    protected YieldInstruction Move(Vector2 beforeMovePosition, float time = 0.3f)
    {
        SwipeStarted?.Invoke();
        BeforeMove();

        _childs[Index].anchoredPosition = beforeMovePosition;

        _swap = DOTween.Sequence().SetUpdate(true)
            .Append(_current.DOAnchorPos(new Vector2(-beforeMovePosition.x, beforeMovePosition.y), time))
            .Join(_childs[Index].DOAnchorPos(_targetPosition, time))
            .AppendCallback(() => _current = _childs[Index]);
        
        return _swap.WaitForCompletion();
    }
}
