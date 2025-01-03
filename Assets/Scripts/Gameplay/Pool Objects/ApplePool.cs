using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ApplePool : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private GenerationArea _generator;

    private List<Apple> _allApples = new List<Apple>();

    private void Awake()
    {
        FillThePool();
    }

    public Apple Take(Type type, Transform parent, Vector2 position, Quaternion rotation)
    {
        if(TryGetByType(type, out Apple apple))
        {
            apple.transform.SetParent(parent);
            apple.transform.position = position;
            apple.transform.rotation = rotation;
            apple.gameObject.SetActive(true);
            _allApples.Remove(apple);
            return apple;
        }
        else
        {
            throw new InvalidOperationException($"В пуле не осталось яблок типа: {type}");
        }
    }

    public void Put(Apple apple)
    {
        apple.transform.SetParent(transform);
        apple.transform.position = transform.position;
        apple.transform.rotation = Quaternion.identity;
        apple.gameObject.SetActive(false);
        _allApples.Add(apple);
    }

    public void AddGoldApple()
    {
        var gold = _generator.ApplePrefabs[1];
        var instance = Instantiate(gold.gameObject, transform);
        instance.SetActive(false);
        var apple = instance.GetComponent<Apple>();
        apple.Init(_generator);
        _allApples.Add(apple);
    }

    private bool TryGetByType(Type appleType, out Apple apple)
    {
        apple = _allApples.Where(apple => apple.GetType() == appleType).FirstOrDefault();
        return apple != null;
    }

    private void FillThePool()
    {
        foreach (var typeOfApple in _generator.ApplePrefabs)
        {
            for (int i = 0; i < _saveSerial.Data.MaxAppleCount; i++)
            {
                var instance = Instantiate(typeOfApple.gameObject, transform);
                instance.SetActive(false);

                var apple = instance.GetComponent<Apple>();
                apple.Init(_generator);
                _allApples.Add(apple);
            }
        }
    }
}
