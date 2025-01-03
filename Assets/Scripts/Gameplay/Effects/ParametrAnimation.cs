using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ParametrAnimation : MonoBehaviour
{
    [SerializeField] private Head _snakehead;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Sprite[] _parametrIcons;

    private Dictionary<Type, Sprite> _spriteOfType;

    private Vector3 _startingOffset;
    private Vector3 _targetOffset;

    private void Start()
    {
        _renderer.gameObject.SetActive(false);

        _startingOffset = new Vector3(0.25f, 0.7f);
        _targetOffset = new Vector3(0.6f, 1.2f);

        _spriteOfType = new Dictionary<Type, Sprite>()
        {
            {typeof(AppleDouble), _parametrIcons[0] },
            {typeof(LuckParametr), _parametrIcons[1] }
        };
    }

    public void IconFlyAway(Type parametr)
    {
        if (parametr.BaseType != typeof(Parametr))
            return;

        _renderer.sprite = _spriteOfType[parametr];
        _renderer.transform.position = _snakehead.transform.position + _startingOffset;
        _renderer.color = Color.white;
        _renderer.gameObject.SetActive(true);

        Sequence sequence = FlyAway();
    }

    private Sequence FlyAway()
    {
        Sequence sequence = DOTween.Sequence()
           .Append(_renderer.transform.DOMove(_snakehead.transform.position + _targetOffset, 1f))
           .Join(_renderer.DOFade(0, 1f))
           .AppendCallback(() => _renderer.gameObject.SetActive(false));
        return sequence;
    }
}
