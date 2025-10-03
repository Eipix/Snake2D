using System;
using UnityEngine;

[Serializable]
public class Translatable<T>
{
    [field: SerializeField] public T[] Translated { get; set; }

    public T Translate => Language.Instance == null
                               ? Translated[0]
                               : Translated[(int)Language.Instance.Current];
}
