using System;
using UnityEngine;

public abstract class Apple : Item
{
    [field:SerializeField] public SaveSerial SaveSerial {get; private set;}

    private GenerationArea _generator;

    public void Init(GenerationArea generator)
    {
        _generator = generator;
    }

    public void Deactivate()
    {
        if (_generator == null)
            throw new NullReferenceException("Apple is not init");

        _generator.RemoveApple(this);
        _generator.AppleGeneration();
    }
}
