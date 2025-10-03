using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Linq;
using Sirenix.Utilities;

public class Upgrade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text _appleBar;
    [SerializeField] private TMP_Text _goldAppleBar;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _currentProgress;
    [SerializeField] private Image _icon;
    [SerializeField] private Image _textCloud;
    [SerializeField] private TMP_Text _price;
    [SerializeField] private Parametr _parametr;

    [Space]
    [SerializeField] private Sprite _filledCell;
    [SerializeField] private Sprite _emptyCell;
    [SerializeField] private TranslatableString _langDescription;
    [SerializeField] private int _multiplier;
    [SerializeField] private string _symbol;

    private Skin _currentSkin;
    private Image[] _cells;
    private Button _upgrade;
    private TMP_Text _text;
    private TranslatableString _langNotEnough = new TranslatableString
    {
        TranslateTexts = new string[]
        {
            "Insufficient resources for improvement",
            "Недостаточно ресурсов для улучшения",
            "İyileştirme için yetersiz kaynak"
        }
    };

    private int[] _pricesForLevel = new int[9]
    { 50, 150, 350, 600, 1000, 1500, 2500, 3500, 4500 };
    private int _currentLevel = 0;
    private bool _isInit;
    private bool _isSkinSkill;

    public bool MaxLevel => _currentLevel >= _pricesForLevel.Length;

    private void OnEnable()
    {
        _parametr = GetComponent<Parametr>();
        if (_parametr == null)
            ChangeToSkinSkill();

        if (_isInit)
            UpdateParametrLevels();

        UpdateValues();
    }

    private void Awake()
    {
        var parentCell = GetComponentInChildren<CellsElement>();
        _cells = parentCell.GetComponentsInChildren<Image>();
        _upgrade = _price.GetComponentInParent<Button>();
        _text = _textCloud.GetComponentInChildren<TMP_Text>();
        _isInit = true;
        UpdateParametrLevels();
    }

    private void UpdateParametrLevels()
    {
        _currentLevel = SaveSerial.Instance.LoadParametr(_parametr, _currentSkin);
        for (int i = 0; i < _currentLevel; i++)
            _cells[i].sprite = _filledCell;
        CheckMaxLevel();
    }

    private void CheckMaxLevel()
    {
        _upgrade.interactable = MaxLevel == false;
        _price.text = MaxLevel ? "MAX" : _pricesForLevel[_currentLevel].ToString();
    }

    public void OnUpgradeClick()
    {
        if (IsAppleSufficient(_isSkinSkill))
        {
            SaveSerial.Instance.SaveParametr(++_currentLevel, _parametr, _currentSkin);
            UpdateParametrLevels();
            UpdateValues();

            Debug.Log("Upgrade was saved in sdk");
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
        }
        else
        {
            Notification.Instance.Notify(_langNotEnough.Translate);
        }
    }

    private bool IsAppleSufficient(bool spendGoldApple = false)
    {
        return spendGoldApple
            ? Wallet.Instance.TrySpentGoldApple(_pricesForLevel[_currentLevel])
            : Wallet.Instance.TrySpentRedApple(_pricesForLevel[_currentLevel]);
    }

    private void ChangeToSkinSkill()
    {
        foreach (var cell in _cells)
            cell.sprite = _emptyCell;

        _isSkinSkill = true;
        _currentSkin = SaveSerial.Instance.SkinPrefabs
            .Where(skin => skin.GetType().ToString() == SaveSerial.Instance.LoadCurrentSkinType())
            .First();

        if (_currentSkin.Rareness == Rarity.Common)
            gameObject.SetActive(false);

        _icon.sprite = _currentSkin.SkillIcon;
        _pricesForLevel = new int[9] { 5, 15, 35, 60, 100, 150, 250, 350, 450 };

        var skillTranslations = _currentSkin.UpgradeTranslate.TranslateTexts;
        _langDescription.TranslateTexts = skillTranslations;

        _icon.SetNativeSize();
    }

    private void UpdateValues()
    {
        if (_isSkinSkill)
        {
            _currentProgress.text = _currentSkin.CurrentLevelText;
            if (MaxLevel == false)
                _currentProgress.text += _currentSkin.NextLevelText;
        }
        else
        {
            string nextLevelParametrs = $"<color=green> +{_multiplier}{_symbol}</color>";
            _currentProgress.text = $"{_currentLevel * _multiplier}{_symbol}";
            if (MaxLevel == false)
                _currentProgress.text += nextLevelParametrs;
        }
    }

    public void OnIconClick()
    {
        if (_textCloud.isActiveAndEnabled)
            return;

        _text.text = _langDescription.Translate;
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => _textCloud.gameObject.SetActive(true));
        sequence.Append(_textCloud.DOFade(1, 1f));
        sequence.Join(_text.DOFade(1, 1f));
        sequence.AppendInterval(1f);
        sequence.Append(_textCloud.DOFade(0, 1f));
        sequence.Join(_text.DOFade(0, 1f));
        sequence.AppendCallback(() => _textCloud.gameObject.SetActive(false));
    }
}
