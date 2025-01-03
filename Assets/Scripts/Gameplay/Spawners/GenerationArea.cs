using UnityEngine;
using NavMeshPlus.Components;
using System.Collections.Generic;
using static SnakeMovement;

public class GenerationArea : MonoBehaviour
{
    [SerializeField] private ApplePool _applePool;
    [SerializeField] private EnemySpawner _enemySpawner;
    [SerializeField] private NavMeshSurface[] _surfaces;
    [SerializeField] private CountdownToStart _countdown;
    [SerializeField] private SaveSerial _saveSerial;

    [SerializeField] private RectTransform _redApplePointBar;
    [SerializeField] private RectTransform _goldApplepointBar;

    [SerializeField] private Apple[] _typeOfApple;
    [SerializeField] private Stone[] _typeOfStone;

    public Apple[] ApplePrefabs => _typeOfApple;

    private List<Apple> _activeApples = new List<Apple>();
    private List<int> _rotations = new List<int> { 0, 90, 180, 270 };

    public int GoldAppleChance => _goldAppleChance;
    public int RottenAppleChance { get; private set; }
    public int BigStoneChance { get; private set; }
    public int MiddleStoneChance { get; private set; }
    public int StoneCount { get; private set; }

    public Vector2 RandomSpawnPoint => new Vector2(-7.7f + (MoveStep * Random.Range(0, 22)), -3.5f + (MoveStep * Random.Range(0, 11)));

    private int _goldAppleChance = 2;
    private int[] _appleChances = new int[100];
    private int[] _stoneChances = new int[10];

    private void Start()
    {
        SetChancesPerLevel();

        SetAppleSpawnChances();
        SetStoneSpawnChances();

        StoneGeneration();
        AppleGeneration();

        foreach (var surface in _surfaces)
        {
            surface.BuildNavMeshAsync();
        }

        StartCoroutine(_countdown.CountdownStart());
    }

    public void AppleGeneration()
    {
        if (_activeApples.Count >= _saveSerial.Data.MaxAppleCount)
        {
            Debug.LogWarning("Apples Count is Max");
            return;
        }

        int l = 0;
        for (int i = _activeApples.Count; i < _saveSerial.Data.MaxAppleCount;)
        {
            var spawnPoint = new Vector2(-7.7f + (MoveStep * Random.Range(0, 22)), -3.5f + (MoveStep * Random.Range(0, 11)));
            var hit = Physics2D.Raycast(spawnPoint, spawnPoint, 0.05f);

            if (hit.collider == null ||
               (!hit.collider.TryGetComponent(out Apple apple) &&
                !hit.collider.TryGetComponent(out Stone stone) &&
                !hit.collider.TryGetComponent(out Snake snake) &&
                !hit.collider.TryGetComponent(out Enemy enemy)))
            {
                //if (hit.collider != null) Debug.LogWarning(_typeOfApple[randomIndex].name + ":" + hit.collider.name);
                //Debug.DrawRay(spawnPoint, spawnPoint, Color.red, 5f, true);
                int randomIndex = _appleChances[Random.Range(0, 100)];
                var newApple = _applePool.Take(_typeOfApple[randomIndex].GetType(), transform, spawnPoint, Quaternion.identity);
                _activeApples.Add(newApple);
                i++;
            }

            if (l > 300)
                break;
            l++;
        }
    }

    private void StoneGeneration()
    {
        StoneCount = Random.Range(StoneCount - 2, StoneCount + 1);
        
        int l = 0;
        for (int i = 0; i < StoneCount;)
        {
            int rotation = Random.Range(0, _rotations.Count);
            int typeOfStone = _stoneChances[Random.Range(0, 10)];

            var smallStonePosition = new Vector2(-7f + (MoveStep * Random.Range(0, 21)), -2.8f + (MoveStep * Random.Range(0, 9)));                                                                                           
            var bigStonePosition = new Vector2(-5.9f + (MoveStep * Random.Range(0, 18)), -1.75f + (MoveStep * Random.Range(0, 6)));
            var middleStonePosition = new Vector2(-6.27f + (MoveStep * Random.Range(0, 18)), -2.1f + (MoveStep * Random.Range(0, 7)));

            Vector2 position;
            RaycastHit2D hit;

            if(typeOfStone == 1 || typeOfStone == 2)
            {
                position = typeOfStone == 1 ? bigStonePosition : middleStonePosition;
                if (typeOfStone == 1)
                {
                    hit = Physics2D.BoxCast(position, Vector2.one, 0f, position, 0.05f);
                }
                else
                {
                    Vector2 size = _typeOfStone[2].GetComponent<BoxCollider2D>().size;
                    float devX = 0;
                    float devY = 0f;

                    if (_rotations[rotation] == 90)
                        devY = 0.3f;
                    else if (_rotations[rotation] == 270)
                        devY = -0.3f;
                    else if (_rotations[rotation] == 0)
                        devX = 0.3f;
                    else if (_rotations[rotation] == 180)
                        devX = -0.3f;

                    var newPosition = new Vector2(position.x + devX, position.y + devY);
                    hit = Physics2D.BoxCast(newPosition, size, _rotations[rotation], position, 0.05f);
                }
            }
            else
            {
                position = smallStonePosition;
                hit = Physics2D.Raycast(position, position, Mathf.Infinity);
            }

            if (hit.collider == null || (!hit.collider.TryGetComponent(out Apple apple) && 
                !hit.collider.TryGetComponent(out Stone stone) && 
                !hit.collider.TryGetComponent(out Snake snake) && 
                !hit.collider.TryGetComponent(out SnakeCollision leadingTrigger)))
            {
                //if(hit.collider != null) Debug.LogWarning(_typeOfStone[typeOfStone].name +":"+hit.collider.name);

                Instantiate(_typeOfStone[typeOfStone], position, Quaternion.Euler(0, 0, _rotations[rotation]), transform);
                i++;               
            }

            if (l > 100)
                break;
            l++;
        }
    }

    public void SetStoneSpawnChances()
    {                
        for (int i = 0; i < _stoneChances.Length; i++)        
            _stoneChances[i] = 0;
        for (int i = 0; i < BigStoneChance;)
            SetSpawnChance(_stoneChances, 1, 10, ref i);        
        for (int i = 0; i < MiddleStoneChance;)
            SetSpawnChance(_stoneChances, 2, 10, ref i);
    }

    public void SetAppleSpawnChances()
    {
        for (int i = 0; i < _appleChances.Length; i++)
            _appleChances[i] = 0;
        for (int i = 0; i < GoldAppleChance;)
            SetSpawnChance(_appleChances, 1, 100, ref i);
        for (int i = 0; i < RottenAppleChance;)
            SetSpawnChance(_appleChances, 2, 100, ref i);
    }

    private void SetSpawnChance(int[] mainChances, int changeTo, int randMaxExclusive, ref int index)
    {
        int rand = Random.Range(0, randMaxExclusive);
        if (mainChances[rand] == 0)
        {
            mainChances[rand] = changeTo;
            index++;
        }
    }

    private void SetChancesPerLevel()
    {
        RottenAppleChance = _saveSerial.Data.RottenAppleChance;
        BigStoneChance = 2;
        MiddleStoneChance = 4;
        StoneCount = _saveSerial.Data.MaxStoneCount;
    }

    public void ChangeGoldAppleChance(int chance)
    {
        _goldAppleChance = chance;
        SetAppleSpawnChances();
    }

    public void SpawnGoldApple(Vector2 position)
    {  
        var apple = _applePool.Take(_typeOfApple[1].GetType(), transform, position, Quaternion.identity);
        _activeApples.Add(apple);
    }

    public void AddGoldApple() => _applePool.AddGoldApple();

    public Vector2 GetTargetPosition(Vector2 offset = default)
    {
        var clearPosition = Vector2.zero;
        for (int i = 0; i < 300; i++)
        {
            var position = new Vector2(-7.7f + (MoveStep * Random.Range(0, 22)), -3.5f + (MoveStep * Random.Range(0, 10)));
            var hit = Physics2D.Raycast(position, position, 0.1f);

            if (hit.collider == null || (!hit.collider.TryGetComponent(out Apple apple) &&
                !hit.collider.TryGetComponent(out Stone stone) &&
                !hit.collider.TryGetComponent(out Snake snake) &&
                !hit.collider.TryGetComponent(out Enemy enemy)))
            {
                clearPosition = position;
                break;
            }
        }
        return clearPosition + offset;
    }

    public void RemoveApple(Apple apple)
    {
        _applePool.Put(apple);
        _activeApples.Remove(apple);
    }

    public IEnumerable<Apple> GetApples() => _activeApples;
}
