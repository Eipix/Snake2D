using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

public abstract class International<T, K> : SerializedMonoBehaviour
{
    [OdinSerialize, OnInspectorInit(nameof(InitDictionary))]
    public Dictionary<Language.Languages, K> Translated { get; protected set; }

    public T TypeToTranslate { get; protected set; }
    public abstract K Value { get; set; }

    private void Awake()
    {
        if (Translated == null || Translated.Count < 1)
            InitDictionary();

        TypeToTranslate = GetComponent<T>();
    }

    private void OnEnable()
    {
        if (Language.Instance != null)
            Language.Instance.LanguageChanged += Translate;
        Translate();
    }

    private void OnDisable()
    {
        if (Language.Instance != null)
            Language.Instance.LanguageChanged -= Translate;
    }

    public void InitDictionary()
    {
        if (Translated != null && Translated.Count > 0)
            return;

        Translated = new Dictionary<Language.Languages, K>();
        foreach (var language in Enum.GetValues(typeof(Language.Languages)))
        {
            Translated.Add((Language.Languages)language, default(K));
        }
    }

    public void Translate()
    {
        if (Language.Instance == null)
        {
            Value = Translated[0];
            return;
        }

        Value = Translated[Language.Instance.Current];
    }
}
