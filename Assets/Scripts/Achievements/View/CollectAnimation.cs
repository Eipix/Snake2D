using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;

public class CollectAnimation : UIMonoBehaviour
{
    [SerializeField] private Vector2 _boundaries;
    [Range(1, 100)]
    [SerializeField] private int _poolLength = 10;

    private List<Image> _items = new List<Image>();
    private Sequence[] _sequences;
    private RectTransform _target;

    private Vector2 _targetPosition => _target.anchoredPosition - rectTransform.anchoredPosition;

    protected override void Awake()
    {
        base.Awake();
        _sequences = new Sequence[_poolLength];
        GameObject item = new GameObject();
        item.AddComponent<RectTransform>();
        item.AddComponent<Image>();
        item.name = "item";

        for (int i = 0; i < _poolLength; i++)
        {
            var instance = Instantiate(item, transform);
            var image = instance.GetComponent<Image>();
            image.raycastTarget = false;
            _items.Add(image);
        }
        _items.DisableAll();
    }

    private void OnDisable() => _sequences.CompleteIfActiveAll();

    [Button]
    private void PlayTest()
    {
        StartCoroutine(Play(10));
    }

    public IEnumerator Play(int maxCount)
    {
        int length = _items.Count;
        if (length > maxCount && maxCount > 0)
            length = maxCount;

        _sequences.CompleteIfActiveAll(true);
        for (int i = 0; i < length; i++)
        {
            _sequences[i] = PlayOne(i);
            yield return new WaitForSeconds(0.1f);
        }
    }

    [Button]
    public void SetParametrs(Sprite sprite, RectTransform target)
    {
        _target = target;
        foreach (var item in _items)
        {
            item.sprite = sprite;
            item.SetNativeSize();
        }
    }

    private Sequence PlayOne(int index)
    {
        _items[index].gameObject.SetActive(true);

        GeneratePosition(out Vector2 position);

        return DOTween.Sequence()
               .Append(_items[index].rectTransform.DOAnchorPos(position, 0.2f))
               .AppendInterval(1f)
               .Append(_items[index].rectTransform.DOAnchorPos(_targetPosition, 0.1f))
               .AppendCallback(() => _target.localScale = Vector3.one)
               .Append(_target.DOPunchScale(Vector3.one * 0.5f, 0.1f))
               .AppendCallback(() =>
               {
                   _items[index].gameObject.SetActive(false);
                   _items[index].rectTransform.anchoredPosition = Vector2.zero;
               });
    }

    private void GeneratePosition(out Vector2 position)
    {
        float x = Random.Range(-_boundaries.x, _boundaries.x);
        float y = Random.Range(-_boundaries.y, _boundaries.y);
        position = new Vector2(x, y);
    }
}
