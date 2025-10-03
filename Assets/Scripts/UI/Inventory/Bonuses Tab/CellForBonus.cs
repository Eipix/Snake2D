using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public class CellForBonus : MonoBehaviour
{
    private SelectedCell _selectCell;
    private TMP_Text _amount;
    private BonusesTab _bonusesTab;
    public UnityAction CellSelected;
    public UnityAction<Bonus> SpriteChanged;
    public UnityAction<Bonus> CellDoubleClicked;

    private float _timer;
    private bool _isButtonClick;

    private void OnEnable()
    {
        _amount = GetComponentInChildren<TMP_Text>();
        _selectCell = GetComponentInChildren<SelectedCell>();
        _bonusesTab = GetComponentInParent<BonusesTab>();

        CellSelected += _bonusesTab.SetCellOnUnselectState;
        SpriteChanged += _bonusesTab.ChangeArtSprites;
        CellDoubleClicked += _bonusesTab.MoveFromCellToSlot;
    }
    
    private void OnDisable()
    {
        CellSelected -= _bonusesTab.SetCellOnUnselectState;
        SpriteChanged -= _bonusesTab.ChangeArtSprites;
        CellDoubleClicked -= _bonusesTab.MoveFromCellToSlot;
    }

    private void Start()
    {
        _amount.text = GetComponentInChildren<Bonus>()?.Amount.ToString();
    }

    private void Update()
    {
        if (_isButtonClick)
            _timer += Time.deltaTime;
    }

    public void OnCellClick()
    {
        Bonus bonus = GetComponentInChildren<Bonus>();
        CellSelected?.Invoke();
        SpriteChanged?.Invoke(bonus);
        _selectCell.GetComponent<Image>().enabled = true;

        if (_timer < 0.3f && _isButtonClick)
        {
            CellDoubleClicked(bonus);
        }
        else if (_timer > 0.3f)
        {
            _timer = 0f;
            _isButtonClick = false;
        }
        _isButtonClick = true;
    }
}
