using UnityEngine;
using UnityEngine.UI;

public class LanguageSelector : MonoBehaviour
{
    [SerializeField] private Language.Languages _language;
    [SerializeField] private Mark _mark;

    private Button _button;

    private void Awake() => _button = GetComponent<Button>();

    private void OnEnable() => _button.onClick.AddListener(() => SetLanguage());

    private void OnDisable() => _button.onClick.RemoveListener(() => SetLanguage());

    private void SetLanguage()
    {
        string language = Language.Instance.Format[(int)_language].ISO_639_1;
        Language.Instance.ChangeLanguage(language);
        _mark.MoveToNewFlag();
    }
}
