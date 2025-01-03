using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(RectTransform))]
public class Arena : MonoBehaviour
{
    [SerializeField] private LevelData[] _waves;
    [SerializeField] private MissionRequirements _requirements;
    [SerializeField] private Level _lastLevel;
    [SerializeField] private SaveSerial _saveSerial;

    [SerializeField] private Sprite _unlock;
    [SerializeField] private Image _icon;

    private Button _button;

    public LevelData[] Waves => _waves;
    public RectTransform RectTransform { get; private set; }

    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
        _button.interactable = false;

        if (_lastLevel.IsComplete())
        {
            _button.interactable = true;
            _icon.sprite = _unlock;
            _icon.SetNativeSize();
        }
    }

    public string GetConditionText()
    {
        return "- Продержаться как можно дольше";
    }

    public void OnButtonClick()
    {
        _requirements.OnMissionClick(_waves[0].LevelIndex);
    }
}
