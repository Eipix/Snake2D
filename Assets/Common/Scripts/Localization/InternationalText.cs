using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using TMPro;

public class InternationalText : SerializedMonoBehaviour
{
    [OdinSerialize, OnInspectorInit(nameof(InitDictionary))]
    private Dictionary<Language.Languages, string> _translateTexts;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        if (_translateTexts == null || _translateTexts.Count < 1)
            InitDictionary();

        _text = GetComponent<TextMeshProUGUI>();
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
        if (_translateTexts != null && _translateTexts.Count > 0)
            return;

        _translateTexts = new Dictionary<Language.Languages, string>();
        foreach (var language in Enum.GetValues(typeof(Language.Languages)))
        {
            _translateTexts.Add((Language.Languages)language, "");
        }
    }

    public void Translate()
    {
        if(Language.Instance == null)
        {
            _text.text = _translateTexts[0];
            return;
        }
        _text.text = _translateTexts[Language.Instance.Current];
    }
}
