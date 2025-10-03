using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "InterText", menuName = "Localization/International Text")]
public class ScriptableInterText : SerializedScriptableObject
{
    [OdinSerialize, OnInspectorInit(nameof(InitDictionary))]
    public Dictionary<Language.Languages, string> TranslatedTexts { get; private set; }

    public void InitDictionary()
    {
        if (TranslatedTexts != null && TranslatedTexts.Count > 0)
            return;

        TranslatedTexts = new Dictionary<Language.Languages, string>();
        foreach (var language in Enum.GetValues(typeof(Language.Languages)))
        {
            TranslatedTexts.Add((Language.Languages)language, "");
        }
    }
}
