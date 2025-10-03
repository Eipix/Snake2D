using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class Arena : MonoBehaviour
{
    [SerializeField] private LevelData[] _waves;
    [SerializeField] private MissionRequirements _requirements;
    [SerializeField] private TranslatableString _description;

    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Sprite _unlock;
    [SerializeField] private Image _icon;
    [SerializeField] private float _offset;

    private Button _button;

    public LevelData[] Waves => _waves;
    public Vector2 Position => _rectTransform.anchoredPosition;

    public string Description => _description.Translate;
    public float Offset => _offset;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.interactable = false;
    }

    private void OnEnable()
    {
        if (SaveSerial.Instance.Levels[^1].IsCompleted)
        {
            _button.interactable = true;
            _icon.sprite = _unlock;
            _icon.SetNativeSize();
        }
    }

    public void OnButtonClick()
    {
        _requirements.OnMissionClick(_waves[0].LevelIndex);
    }
}
