using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public abstract class Bonus : Item, IArtChanger, IProduct
{
    [SerializeField] private Sprite _smallIcon;

    [field:Header("IArtChanger")]
    [field: SerializeField] public Sprite FullScreenIcon { get; set; }
    [field: SerializeField] public Sprite Light { get; set; }
    [field: SerializeField] public TranslatableString LangDescription { get; set; }
    [field: SerializeField] public AssetText LangHeadline { get; set; }


    [field:Header("Characteristics")]
    [field:SerializeField] public int MaxToAdd { get; protected set; }
    [field:SerializeField] public int CoolDown { get; protected set; }

    public UnityEvent Used;

    public Sprite SmallIcon => _smallIcon;
    protected Sprite icon;
    protected Slots slots { get; set; }
    protected Slot slot { get; set; }
    protected Effects effect { get; set; }

    public int Amount => SaveSerial.Instance.LoadBonusAmount(this);

    protected virtual void Start()
    {
        icon = GetComponent<Image>().sprite;

        if (SceneManager.GetActiveScene().name == "Level")
        {
            effect = GetComponentInParent<Effects>();
            slots = GetComponentInParent<Slots>();
            slot = GetComponentInParent<Slot>();
        }
    }

    public virtual void Effect()
    {
        SaveSerial.Instance.Increment(1, SaveSerial.JsonPaths.SpentBonuses);
        Used?.Invoke();
    }

    public void Receive(int count)
    {
        var result = Amount + count;
        SaveSerial.Instance.SaveBonusAmount(this, result);
    }

    public override void Add(int count = 1, bool updateBalance = true)
    {
        var result = Amount + count;
        SaveSerial.Instance.SaveBonusAmount(this, result);
    }

    public int Decrese(int count = 1)
    {
        var result = Amount >= count ? Amount - count : Amount;
        SaveSerial.Instance.SaveBonusAmount(this, result);
        return result;
    }

    public bool TrySpend(int count = 1)
    {
        SaveSerial.Instance.SaveBonusInSlots(this, -1);
        slots.UpdateBonuses();
        slot.DisableSelected();

        if (SaveSerial.Instance.LoadBonusInSlots().ContainsKey(GetType().ToString()))
            slot.ReloadActive(true);

        Debug.Log("Bonus spend save in sdk");
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
        return true;
    }

    public void ChangeArt(Image art, Image light, TMP_Text description, TextMeshProUGUI headline)
    {
        art.sprite = FullScreenIcon;
        light.sprite = Light;
        description.text = LangDescription.Translate;
        light.gameObject.SetActive(true);
        headline.SetAssetText(LangHeadline);
        art.SetNativeSize();
        light.SetNativeSize();
    }
}
