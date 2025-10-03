using System;
using UnityEngine;
using TMPro;

[Serializable]
public struct AssetText
{
    [field: SerializeField] public TranslatableString Text { get; set; }
    [field:SerializeField] public Material Preset { get; set; }
    [field:SerializeField] public Color Color { get; set; }
}
