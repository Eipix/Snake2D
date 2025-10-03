using TMPro;
using UnityEngine;

public class InterText : MonoBehaviour
{
    [SerializeField] private ScriptableInterText _interTexts;

    private TextMeshProUGUI _text;

    private void Awake()
    {
        if (_interTexts.TranslatedTexts == null || _interTexts.TranslatedTexts.Count < 1)
            _interTexts.InitDictionary();

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

    public void Translate()
    {
        if (Language.Instance == null)
        {
            _text.text = _interTexts.TranslatedTexts[0];
            return;
        }
        _text.text = _interTexts.TranslatedTexts[Language.Instance.Current];
    }
}

