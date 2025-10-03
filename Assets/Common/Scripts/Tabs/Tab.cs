using UnityEngine;
using UnityEngine.UI;

public abstract class Tab : MonoBehaviour
{
    [SerializeField] private Sprite _stateOn;
    [SerializeField] private Sprite _stateOff;
    [SerializeField] private RectTransform _content;

    public Sprite StateOn => _stateOn;
    public Sprite StateOff => _stateOff;
    public RectTransform Content => _content;

    public Button Button { get; private set; }
    public Image Image { get; private set; }
    public TabController Controller { get; private set; }

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        Image = GetComponent<Image>();
        Controller = gameObject.GetComponentInParent<TabController>();
    }

    protected virtual void OnEnable() => Button.onClick.AddListener(On);

    protected virtual void OnDisable() => Button.onClick.RemoveListener(On);

    public virtual void On()
    {
        Image.sprite = StateOn;
        Controller.SetEnabled(this);
    }

    public virtual void Off() => Image.sprite = StateOff;
}
