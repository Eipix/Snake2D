using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public abstract class Bonus : Item,IArtChanger
{
    [Header("References")]
    [SerializeField] protected SaveSerial saveSerial;

    [field:Header("IArtChanger")]
    [field: SerializeField] public Sprite IconArt { get; set; }
    [field: SerializeField] public Sprite Light { get; set; }
    [field: SerializeField] public Sprite Headline { get; set; }
    [field: TextArea]
    [field: SerializeField] public string Description { get; set; }

    [field:Header("Characteristics")]
    [field:SerializeField] public int MaxToAdd { get; protected set; }
    [field:SerializeField] public int CoolDown { get; protected set; }

    protected Sprite icon;
    protected Slots slots;
    protected Slot slot;
    protected Effects effect;

    protected int amount;

    protected virtual void Start()
    {
        amount = saveSerial.LoadBonusAmount(this);
        icon = GetComponent<Image>().sprite;

        if (SceneManager.GetActiveScene().name == "Level")
        {
            effect = GetComponentInParent<Effects>();
            slots = GetComponentInParent<Slots>();
            slot = GetComponentInParent<Slot>();
        }
    }

    public abstract void Effect();

    public int GetAmount() => amount = saveSerial.LoadBonusAmount(this);

    public override void Add(int count = 1)
    {
        GetAmount();
        amount += count;
        saveSerial.SaveBonusAmount(this, amount);
    }

    public int Decrese(int count = 1)
    {
        amount = amount >= count ? saveSerial.LoadBonusAmount(this) - count : saveSerial.LoadBonusAmount(this);
        saveSerial.SaveBonusAmount(this, amount);
        return amount;
    }

    public void Spend()
    {
        saveSerial.SaveBonusInSlots(this, -1);
        slots.UpdateBonuses();
        slot.DisableSelected();

        if (saveSerial.LoadBonusInSlots().ContainsKey(GetType().ToString()))
            slot.ReloadActive(true);
    }

    public void ChangeArt(ref Image art, ref Image name, ref Image light, ref TMP_Text description)
    {
        art.sprite = IconArt;
        name.sprite = Headline;
        light.sprite = Light;
        description.text = Description;
        light.gameObject.SetActive(true);
        art.SetNativeSize();
        name.SetNativeSize();
        light.SetNativeSize();
    }
}
