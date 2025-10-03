using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClickBehaviour : MonoBehaviour
{
    [SerializeField] private Image _shadow;
    [SerializeField] private Image _selected;

    private Skin _skin;
    private SpawnSkins _skins;

    private float _timer;
    private bool _isButtonClick;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Menu")
            return;

        _skins = GetComponentInParent<SpawnSkins>();
        _skin = GetComponent<Skin>();

        var skinType = _skin.GetType().ToString();
        var savedSkinType = SaveSerial.Instance.LoadCurrentSkinType();

        if (skinType == savedSkinType)
        {
            if (_skin.UnlockState)
                Equip(_skin);
            else
                Equip(null, false);
        }
    }

    private void Update()
    {
        if (_isButtonClick)
            _timer += Time.deltaTime;
    }

    public void OnSkinClick()
    {
        _skins.SetUnclickableAll();
        _shadow.enabled = true;
        _skins.InventoryArt.ChangeArt(_skin);

        if (_timer < 0.3f && _isButtonClick)
        {
            Equip(_skin);
        }
        else if (_timer > 0.3f)
        {
            _timer = 0f;
            _isButtonClick = false;
        }
        _isButtonClick = true;
    }

    private void Equip(Skin skin, bool selectedEnable = true)
    {
        SaveSerial.Instance.SaveCurrentSkin(skin);
        _skins.SetUnselectedAll();

        _selected.enabled = selectedEnable;
    }

    public void ShadowDisable() => _shadow.enabled = false;
    public void SelectedDisable() => _selected.enabled = false;
}
