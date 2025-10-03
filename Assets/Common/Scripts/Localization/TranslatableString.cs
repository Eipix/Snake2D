using System;
using UnityEngine;

[Serializable]
public class TranslatableString
{
    [field:SerializeField] public string[] TranslateTexts { get; set; }

    public string Translate => Language.Instance == null
                               ? TranslateTexts[0]
                               : TranslateTexts[(int)Language.Instance.Current];
}
