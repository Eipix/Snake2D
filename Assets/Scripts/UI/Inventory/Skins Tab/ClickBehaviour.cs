using UnityEngine;
using UnityEngine.UI;

public class ClickBehaviour : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private Image _shadow;
    [SerializeField] private Image _selected;

    private SpawnSkins _skins;

    private string _skinType;
    private float _timer;
    private bool _isButtonClick;

    private void Start()
    {
        _skins = GetComponentInParent<SpawnSkins>();
        _skinType = GetComponent<Skin>().GetType().ToString();

        if (_skinType == _saveSerial.LoadCurrentSkinType())
            SetCurrent();
    }

    private void Update()
    {
        if (_isButtonClick)
            _timer += Time.deltaTime;
    }

    public void OnSkinClick()
    {
        _skins.SetUnclickableState();
        _shadow.enabled = true;
        _skins.InventoryArt.ChangeArt(GetComponent<Skin>());

        if (_timer < 0.3f && _isButtonClick)
        {
            SetCurrent();
        }
        else if (_timer > 0.3f)
        {
            _timer = 0f;
            _isButtonClick = false;
        }
        _isButtonClick = true;
    }

    public void SetCurrent()
    {
        _saveSerial.SaveCurrentSkin(GetComponent<Skin>());
        _skins.SetUnselectedState();
        _selected.enabled = true;
    }

    public void ShadowDisable() => _shadow.enabled = false;
    public void SelectedDisable() => _selected.enabled = false;

}
