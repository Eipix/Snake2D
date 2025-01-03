using System;
using UnityEngine;

[Serializable]
public class SpinItems
{
    [field: SerializeField] public Item Item { get; private set; }
    [field: SerializeField] public int Count { get; private set; }
    [field: SerializeField] public int Probability { get; private set; }
    [field: SerializeField] public Vector2 RotationZBoundaries { get; private set; }
    [field: SerializeField] public int EquivalentInRedApples { get; private set; }

    public bool InBoundaries(float rotationZ)
    {
        return rotationZ > RotationZBoundaries.x && rotationZ < RotationZBoundaries.y;
    }
}
