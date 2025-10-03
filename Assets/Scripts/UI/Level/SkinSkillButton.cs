using UnityEngine;
using UnityEngine.UI;

public class SkinSkillButton : MonoBehaviour
{
    [SerializeField] private SkinLoader _loader;
    [SerializeField] private SlotReload _reload;

    public Skin CurrentSkin { get; private set; }

    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        CurrentSkin = _loader.CurrentSkin;

        IfSkinIsCommon(_loader.CurrentSkin);
        SetSprite(_image);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(_reload.gameObject.activeSelf == false)
            {
                OnButtonClick();
            }
        }
    }

    public void OnButtonClick()
    {
        _reload.gameObject.SetActive(true);
        _loader.CurrentSkin.SkillActivation();
    }

    private void SetSprite(Image image) => image.sprite = _loader.CurrentSkin.ActivationSkillIcon;

    private void IfSkinIsCommon(Skin skin)
    {
        if (skin.Rareness == Rarity.Common)
            gameObject.SetActive(false);
    }
}
