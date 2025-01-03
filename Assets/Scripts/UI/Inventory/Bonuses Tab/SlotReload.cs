using UnityEngine;
using UnityEngine.UI;

public class SlotReload : MonoBehaviour
{
    private Bonus _bonus;
    private SkinSkillButton _skinSkill;
    private Image _reload;
    private Button _button;

    private int _cooldown;
    private float _timer = 0f;

    private void OnEnable()
    {
        if (_button != null)
            _button.interactable = false;
    }

    private void Start()
    {
        _button = transform.parent.GetComponent<Button>();
        _button.interactable = false;
        _bonus = transform.parent.GetComponentInChildren<Bonus>();
        _reload = GetComponent<Image>();

        if (_bonus == null)
            _skinSkill = GetComponentInParent<SkinSkillButton>();

        CheckTypeOfSlot(_bonus, _skinSkill);
    }

    private void Update()
    {
        if(_timer > _cooldown)
        {
            _timer = 0f;
            _reload.fillAmount = 1f;
            _button.interactable = true;
            gameObject.SetActive(false);
        }
        _timer += Time.deltaTime;
        _reload.fillAmount -= Time.deltaTime / (_cooldown == 0? 1: _cooldown);
    }

#nullable enable
    private void CheckTypeOfSlot(Bonus? bonus, SkinSkillButton? skin)
    {
        _cooldown = skin == null ? bonus!.CoolDown : skin.CurrentSkin.Cooldown;
    }
#nullable disable
}
