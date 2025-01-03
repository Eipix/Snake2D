using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using DG.Tweening;
using System.Linq;

public class Upgrade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private SpawnSkins _skins;
    [SerializeField] private Notification _notify;
    [SerializeField] private TMP_Text _appleBar;
    [SerializeField] private TMP_Text _goldAppleBar;

    [Header("Components")]
    [SerializeField] private Image _icon;
    [SerializeField] private Image _textCloud;
    [SerializeField] private TMP_Text _price;

    [Space]
    [SerializeField] private Sprite _filledCell;
    [SerializeField] private Sprite _emptyCell;
    [SerializeField] private string _description;

    private Skin _currentSkin;
    private Image[] _cells;
    private Button _upgrade;
    private TMP_Text _text;

    public UnityAction<string> Clicked; 

    private int[] _pricesForLevel = new int[9]
    { 50, 150, 350, 600, 1000, 1500, 2500, 3500, 4500 };
    private int _currentLevel = 0;
    private bool _isInit;
    private bool _isSkinSkill;

    private void OnEnable()
    {
        Clicked += _notify.Notify;

        var defaultParametr = GetComponent<Parametr>();
        if (defaultParametr == null)
            ChangeToSkinSkill();
    }

    private void OnDisable() => Clicked -= _notify.Notify;

    private void Start()
    {
        var parentCell = GetComponentInChildren<CellsElement>();
        _cells = parentCell.GetComponentsInChildren<Image>();
        _upgrade = _price.GetComponentInParent<Button>();
        _text = _textCloud.GetComponentInChildren<TMP_Text>();
        _isInit = true;
        UpdateParametrLevels();
    }

    private void SetEmptyAllCells()
    {
        for (int i = 0; i < _currentLevel; i++)
            _cells[i].sprite = _emptyCell;
    }

    private void UpdateParametrLevels()
    {
        _currentLevel = _saveSerial.LoadParametr(GetComponent<Parametr>(), _currentSkin);
        for (int i = 0; i < _currentLevel; i++)
            _cells[i].sprite = _filledCell;
        CheckMaxLevel();
    }

    private void CheckMaxLevel()
    {
        bool isLevelNotMax = _currentLevel < _pricesForLevel.Length;
        _upgrade.interactable = isLevelNotMax;
        _price.text = isLevelNotMax ? _pricesForLevel[_currentLevel].ToString() : "MAX";
    }

    public void OnUpgradeClick()
    {
        if (IsAppleSufficient(_isSkinSkill))
        {
            _saveSerial.SaveParametr(++_currentLevel, GetComponent<Parametr>(), _currentSkin);
            UpdateParametrLevels();
        }
        else
        {
            Clicked?.Invoke("Недостаточно ресурсов для улучшения");
        }
    }

    private bool IsAppleSufficient(bool spendGoldApple = false)
    {
        (int redAppleAmount, int goldAppleAmount) = _saveSerial.LoadApple();

        if (spendGoldApple)
        {
            if (goldAppleAmount >= _pricesForLevel[_currentLevel])
                SpendGoldApple(redAppleAmount, goldAppleAmount);
            return goldAppleAmount >= _pricesForLevel[_currentLevel];
        }
        else
        {
            if (redAppleAmount >= _pricesForLevel[_currentLevel])
                SpendApple(redAppleAmount, goldAppleAmount);
            return redAppleAmount >= _pricesForLevel[_currentLevel];
        }
    }

    private void SpendApple(int redAppleAmount, int goldAppleAmount)
    {
        _saveSerial.SaveApple(redAppleAmount - _pricesForLevel[_currentLevel], goldAppleAmount);
        _appleBar.text = (redAppleAmount - _pricesForLevel[_currentLevel]).ToString("D8");
        _goldAppleBar.text = goldAppleAmount.ToString("D8");
    }

    private void SpendGoldApple(int redAppleAmount, int goldAppleAmount)
    {
        _saveSerial.SaveApple(redAppleAmount, goldAppleAmount - _pricesForLevel[_currentLevel]);
        _appleBar.text = redAppleAmount.ToString("D8");
        _goldAppleBar.text = (goldAppleAmount - _pricesForLevel[_currentLevel]).ToString("D8");
    }

    private void ChangeToSkinSkill()
    {
        SetEmptyAllCells();
        _isSkinSkill = true;
        _currentSkin = _skins.PrefabSkins.Where(skin => skin.GetType().ToString() == _saveSerial.LoadCurrentSkinType()).First();

        if (_currentSkin.Rareness == Rarity.Common)
            gameObject.SetActive(false);

        _icon.sprite = _currentSkin.SkillIcon;
        _pricesForLevel = new int[9] { 5, 15, 35, 60, 100, 150, 250, 350, 450 };
        _description = _currentSkin.UpgradeDescription;
        _icon.SetNativeSize();

        if (_isInit)
            UpdateParametrLevels();
    }

    public void OnIconClick()
    {
        if (_textCloud.isActiveAndEnabled)
            return;

        _text.text = _description;
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
