using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventoryArt : MonoBehaviour
{
    [Header("Art")]
    [SerializeField] private Image _art;
    [SerializeField] private Image _light;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TextMeshProUGUI _headline;
    [SerializeField] private TranslatableString _langDescription;

    [Space]
    [SerializeField] private Sprite _question;
    [SerializeField] private AssetText _assetText;

    [Header("Skill")]
    [SerializeField] private Sprite[] _skillIcons;
    [SerializeField] private Image _skillIcon;
    [SerializeField] private Image _cloud;
    [SerializeField] private TMP_Text _skillDescription;

    private Sequence _sequence;

    public void OnSkillClick()
    {
        if (_cloud.color.a == 0)
        {
            _cloud.DOFade(1f, 0f);
            _skillDescription.DOFade(1f, 0f);

            _sequence = DOTween.Sequence();
            _sequence.AppendInterval(2f);
            _sequence.Append(_cloud.DOFade(0f, 1f));
            _sequence.Join(_skillDescription.DOFade(0f, 1f));
            _sequence.AppendCallback(() => _sequence.Complete());
        }
    }

    public void ArtEmpty()
    {
        _art.sprite = _question;
        _light.gameObject.SetActive(false);
        _skillIcon.gameObject.SetActive(false);
        _description.text = _langDescription.Translate;
        _headline.SetAssetText(_assetText);
        _art.SetNativeSize();
    }

    public void ChangeArt(IArtChanger item)
    {
        item.ChangeArt(_art, _light, _description, _headline);

        if (item is Skin skin)       
            IfItemIsSkin(skin);
        else
            _skillIcon.gameObject.SetActive(false);
    }

    private void IfItemIsSkin(Skin skin)
    {
        _sequence.Complete();
        _cloud.DOFade(0f, 0f);
        _skillDescription.DOFade(0f, 0f);

        _skillIcon.sprite = _skillIcons[(int)skin.Skill];
        _skillDescription.text = skin.SkillTranslate.Translate;

        _skillIcon.gameObject.SetActive(true);
    }
}
