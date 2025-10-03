using UnityEngine;
using System;

[Serializable]
public class ItemCountPair
{
    [field: SerializeField] public Item Item { get; set; }
    [field: SerializeField] public int Count { get; set; }
}
