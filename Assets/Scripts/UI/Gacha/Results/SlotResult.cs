using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SlotResult : UIMonoBehaviour
{
    [SerializeField] private Image _avatar;
    [SerializeField] private Image _outlight;
    [SerializeField] private Image _newIcon;
    [SerializeField] private Image _dublicateIcon;
    [SerializeField] private TextMeshProUGUI _compensationCount;
    [SerializeField] private Sprite[] _apples;

    public YieldInstruction Punch() => rectTransform.DOPunchScale(Vector2.one, 0.1f).WaitForCompletion();

    public void DisableElements()
    {
        _outlight.gameObject.SetActive(false);
        _newIcon.gameObject.SetActive(false);
        _dublicateIcon.gameObject.SetActive(false);
    }

    public void Show(IGachaReward reward)
    {
        DisableElements();
        _avatar.sprite = reward.Icon;
        _avatar.SetNativeSize();
    }

    public void New(IGachaReward reward)
    {
        Show(reward);
        _newIcon.gameObject.SetActive(true);

        if(reward is Skin skin && skin.Rarity != Rarity.Common)
        {
            OutlightEnable(skin.Outlight);
        }
    }

    public void DublicateUpgrade(IGachaReward reward, Skin skin)
    {
        Show(reward);
        OutlightEnable(skin.Outlight);
        DublicateEnable(skin.SkillIcon, "1");
        _dublicateIcon.rectTransform.sizeDelta /= 2;
    }

    public void AppleCompensation(IGachaReward reward, Skin skin)
    {
        Show(reward);

        if (skin.Rarity != Rarity.Common)
            OutlightEnable(skin.Outlight);

        Sprite compensationIcon = skin.Pair.Item is RedApple ? _apples[0] : _apples[1];
        DublicateEnable(compensationIcon, skin.Pair.Count.ToString());
    }

    private void DublicateEnable(Sprite icon, string compensationCount)
    {
        _dublicateIcon.sprite = icon;
        _dublicateIcon.gameObject.SetActive(true);
        _dublicateIcon.SetNativeSize();

        _compensationCount.text = $"+{compensationCount}";
    }

    private void OutlightEnable(Sprite outlight)
    {
        _outlight.sprite = outlight;
        _outlight.gameObject.SetActive(true);
    }
}
