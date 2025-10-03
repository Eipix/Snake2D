using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class King : Skin
{
    [Header("Components For Skill")]
    [SerializeField] private GameObject _cherry;
    [SerializeField] private GameObject _pineCone;

    private System.Random rand = new System.Random();
    private List<Item> _items = new List<Item>();
    private GameObject[] _cherries;
    private GameObject[] _pineCones;

    private Vector2 _targetPosition = Vector2.zero;

    private readonly int _cherriesCount = 2;
    private readonly int _pineConesCount = 5;
    private readonly float _offset = 0.2f;

    public override string CurrentLevelText => $"{20 + SkillLevel * 3}%";
    public override string NextLevelText => $"<color=green> +3%</color>";

    protected override void Start()
    {
        base.Start();

        if (SceneManager.GetActiveScene().name != "Level")
            return;

        _cherry.GetComponent<Collider2D>().enabled = false;

        _cherries = GetCherries();
        _pineCones = GetPineCones();
        var union = _cherries.Union(_pineCones).ToArray();

        foreach (var item in union)
        {
            _items.Add(new Item(item, References, this));
        }
    }

    public override void SkillActivation()
    {
        if (References.EnemySpawner.Count < 1)
            return;

        int quantity = rand.withProbability(20 + SkillLevel)
            ? _items.Count
            : 1;
        StartCoroutine(DropItems(quantity));     
    }

    private IEnumerator DropItems(int quantity)
    {
        DisableItems();
        _items =_items.Shuffled();

        for (int i = 0; i < quantity; i++)
        {
            yield return DOTween.Sequence()
            .Append(MoveAtPosition(i))
            .AppendInterval(0.5f)
            .WaitForCompletion();
        }
    }

    private Tween MoveAtPosition(int index)
    {
        SetPositions(index);
        return _items[index].transform.DOMove(_targetPosition, 1f);
    }

    private void SetPositions(int index)
    {
        _targetPosition = _items[index].TreeProduct.GetTargetPosition(new Vector2(0, _offset));
        _items[index].transform.position = GetStartingPosition(_targetPosition);
        _items[index].gameObject.SetActive(true);
    }

    private Vector2 GetStartingPosition(Vector2 targetPosition)
    {
        float up = 8f;
        return new Vector2(targetPosition.x, up);
    }

    private void DisableItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].gameObject.SetActive(false);
        }
    }

    private GameObject[] GetPineCones()
    {
        GameObject[] pineCones = new GameObject[_pineConesCount];
        for (int i = 0; i < _pineConesCount; i++)
        {
            pineCones[i] = Instantiate(_pineCone.gameObject, References.Generator.transform);
            pineCones[i].SetActive(false);
        }
        return pineCones;
    }

    private GameObject[] GetCherries()
    {
        GameObject[] cherries = new GameObject[_cherriesCount];
        for (int i = 0; i < _cherriesCount; i++)
        {
            cherries[i] = Instantiate(_cherry.gameObject, References.Generator.transform);
            cherries[i].SetActive(false);
        }
        return cherries;
    }
    
    public class Item
    {
        public TreeProduct TreeProduct { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public Animator Animator { get; private set; }
        public GameObject gameObject { get; private set; }
        public Transform transform { get; private set; }

        public Item(GameObject item, SkinReferences references, King king)
        {
            TreeProduct = item.GetComponent<TreeProduct>();
            TreeProduct.Init(references, king);
            Renderer = item.GetComponent<SpriteRenderer>();
            Animator = item.GetComponent<Animator>();
            gameObject = item;
            transform = item.transform;
        }
    }
}
