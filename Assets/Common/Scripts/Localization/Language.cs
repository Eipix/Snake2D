using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

public class Language : Singleton<Language>
{
    public Languages Current { get; private set; }

    public enum Languages
    {
        English, Russian, Turkey
    }

    private Formats[] _formats =
    {
        new Formats(Languages.English, "en", "English"),
        new Formats(Languages.Russian, "ru", "Русский"),
        new Formats(Languages.Turkey, "tr", "Türkçe")
    };

    public Formats[] Format => _formats;

    public UnityAction LanguageChanged;

    [DllImport("__Internal")]
    public static extern string GetLang();

    public static IEnumerator WaitInitCoroutine()
    {
        yield return new WaitWhile(() => Instance == null);
        Instance.ChangeLanguage(GetLang());
    }

    public void ChangeLanguage(string lang)
    {
        for (int i = 0; i < _formats.Length; i++)
        {
            if (lang == _formats[i].ISO_639_1)
            {
                Current = (Languages)i;
            }
        }
        LanguageChanged?.Invoke();
    }

    public struct Formats
    {
        public readonly Languages Enumerator;
        public readonly string ISO_639_1;
        public readonly string Translated;

        public Formats(Languages enumerator, string iso_639_1, string translated)
        {
            Enumerator = enumerator;
            ISO_639_1 = iso_639_1;
            Translated = translated;
        }
    }
}
