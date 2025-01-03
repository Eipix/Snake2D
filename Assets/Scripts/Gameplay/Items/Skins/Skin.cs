using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class Skin : MonoBehaviour, IArtChanger
{
    [field:Header("IArtChanger")]
    [field:SerializeField] public Sprite IconArt { get; set; }
    [field: SerializeField] public Sprite Light { get; set; }
    [field: SerializeField] public Sprite Headline { get; set; }
    [field:TextArea]
    [field:SerializeField] public string Description { get; set; }

    [Header("References")]
    [SerializeField] protected SaveSerial saveSerial;

    [Header("Components")]
    [SerializeField] protected Image ironLock;
    [SerializeField] protected Image outline;
    [SerializeField] protected Image outlight;
    [SerializeField] protected Image avatar;
    [SerializeField] private Button _button;

    [Header("Sprites")]
    [SerializeField] private Sprite[] _partsToLoadAtLevel;
    [SerializeField] private Sprite _avatarUnlock;
    [SerializeField] private Sprite _avatarLock;
    [SerializeField] private Sprite _skillIcon;
    [SerializeField] private Sprite _activationSkillIcon;

    [Header("Characteristis")]
    [SerializeField] protected Rarity rareness;
    [SerializeField] protected CombatSkill skill;
    [SerializeField] private int _cooldown;

    public Sprite SkillIcon => _skillIcon;
    public Sprite ActivationSkillIcon => _activationSkillIcon;  
    public SkinReferences References { get; private set; }

    public Rarity Rareness => rareness;
    public CombatSkill Skill => skill;
    public bool UnlockState
    {
        get => saveSerial.LoadLockState(this.GetType());
        private set
        {
            saveSerial.SaveDataSkin(GetComponent<Skin>(), value);
            if (value) SetUnlockState();
            else SetLockState();
        }
    }
    public int Cooldown => _cooldown;

    public string SkillDescription => _skillDescriptions[(int)skill];
    public string UpgradeDescription => _upgradeDescription[(int)skill];
    protected int SkillLevel { get; private set; }

    private readonly string[] _skillDescriptions =
    {
        "Не имеет особых свойств",
        "Заморозка",
        "Ускорение/Электро-разряд",
        "Взрыв",
        "Помощь леса",
        "Неуязвимость",
        "Богатство"
    };
    private readonly string[] _upgradeDescription =
    {
        "",
        "Увеличивает длительность заморозки на 1 секунду",
        "Уменьшает перезарядку навыка на 1 секунду",
        "Увеличивает шанс нанести 2 ед. урона на 2%",
        "Увеличивает шанс сценария 2 на 3%",
        "Увеличивает длительность неуязвимости на 1 секунду",
        "Увеличивает шанс появления золотых яблок на 1%"
    };

    protected virtual void OnEnable() => UnlockState = saveSerial.LoadLockState(this.GetType());

    protected virtual void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
            avatar.SetNativeSize();

        SkillLevel = saveSerial.LoadSkinSkill(this);
    }

    protected virtual void OnInit() { }

    public void Init(SkinReferences references)
    {
        References = references;
        OnInit();
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

    public abstract void SkillActivation();

    protected void SetLockState()
    {
        avatar.sprite = _avatarLock;
        ironLock.enabled = true;
        outlight.enabled = false;
        outline.enabled = false;
        _button.interactable = false;
    }

    protected void SetUnlockState()
    {
        avatar.sprite = _avatarUnlock;
        avatar.SetNativeSize();
        ironLock.enabled = false;
        outlight.enabled = true;
        outline.enabled = true;
        _button.interactable = true;
    }

    public void Unlock() => UnlockState = true;

    public void LoadSkin(ref SpriteRenderer[] currentSkin, ref Image avatar, ref SpriteRenderer bodyDefaultInScene)
    {
        for (int i = 0; i < _partsToLoadAtLevel.Length; i++)
        {
            currentSkin[i].sprite = _partsToLoadAtLevel[i];
        }
        avatar.sprite = _avatarUnlock;
        bodyDefaultInScene.sprite = _partsToLoadAtLevel[1];
    }

    public void DecreaseCooldown(int inSeconds)
    {
        if (_cooldown > inSeconds) _cooldown -= inSeconds;
    }

    public void TriggerAnimationIn(Animator animator, Vector3 targetPosition, string trigger, Transform parent = null, bool localPosition = false)
    {
        if (parent != null)
            animator.gameObject.transform.SetParent(parent);

        if(localPosition)
            animator.gameObject.transform.localPosition = targetPosition;
        else
            animator.gameObject.transform.position = targetPosition;

        animator.SetTrigger(trigger);
    }
}

[Serializable]
public class SkinReferences
{
    [field:SerializeField] public Effects Effects { get; private set; }
    [field: SerializeField] public SpriteRenderer Borders { get; private set; }
    [field: SerializeField] public GenerationArea Generator { get; private set; }
    [field: SerializeField] public EnemySpawner EnemySpawner { get; private set; }
    [field: SerializeField] public Head SnakeHead { get; private set; }
    [field: SerializeField] public Tail SnakeTail { get; private set; }
}

public enum Rarity
{
    Common,
    Epic,
    Legendary
}

public enum CombatSkill
{
    None,
    Ice,
    Lightning,
    Bomb,
    King,
    Defence,
    Coin
}

