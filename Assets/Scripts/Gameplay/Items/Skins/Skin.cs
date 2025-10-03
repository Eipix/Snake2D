using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public abstract class Skin : MonoBehaviour, IArtChanger, IGachaReward, IGachaCompensation, IProduct
{
    [field: Header("IGachaReward")]
    [field: SerializeField] public Sprite Icon { get; set; }


    [field: Header("IGachaCompensation")]
    [field: SerializeField] public ItemCountPair Pair { get; set; }

    [field:Header("IArtChanger")]
    [field:SerializeField] public Sprite FullScreenIcon { get; set; }
    [field: SerializeField] public Sprite Light { get; set; }
    [field: SerializeField] public TranslatableString LangDescription { get; set; }
    [field: SerializeField] public AssetText LangHeadline { get; set; }

    [Header("Components")]
    [SerializeField] protected Image ironLock;
    [SerializeField] protected Image outline;
    [SerializeField] protected Image outlight;
    [SerializeField] protected Image avatar;
    [SerializeField] private Button _button;
    [field:SerializeField, Required] public ClickBehaviour ClickBehaviour { get; private set; }

    [Header("Sprites")]
    [SerializeField] private Sprite[] _partsToLoadAtLevel;
    [SerializeField] private Sprite _avatarUnlock;
    [SerializeField] private Sprite _avatarLock;
    [SerializeField] private Sprite _skillIcon;
    [SerializeField] private Sprite _activationSkillIcon;

    [field:Header("Characteristis")]
    [field: SerializeField] public Rarity Rarity { get; set; }
    [SerializeField] protected CombatSkill skill;
    [SerializeField] private int _cooldown;
    [field:SerializeField] public TranslatableString SkillTranslate { get; private set; }
    [field: SerializeField] public TranslatableString UpgradeTranslate { get; private set; }

    public Sprite SkillIcon => _skillIcon;
    public Sprite ActivationSkillIcon => _activationSkillIcon;
    public Sprite Outlight => outlight.sprite;
    public SkinReferences References { get; private set; }

    public Rarity Rareness => Rarity;
    public CombatSkill Skill => skill;

    public bool UnlockState
    {
        get => SaveSerial.Instance.LoadLockState(GetType());
        private set
        {
            SaveSerial.Instance.SaveDataSkin(this, value);
            if (value) SetUnlockState();
            else SetLockState();
        }
    }

    public virtual string CurrentLevelText => "";
    public virtual string NextLevelText => "";

    public int Cooldown => _cooldown;
    protected int SkillLevel => SaveSerial.Instance.LoadSkinSkill(this);
    protected bool IsSkillMaxLevel => SkillLevel >= SkillMaxLevel;

    private const int SkillMaxLevel = 9;

    protected virtual void OnEnable() => UnlockState = SaveSerial.Instance.LoadLockState(GetType());

    protected virtual void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu")
        {
            avatar.SetNativeSize();
        }
    }

    protected virtual void OnInit() { }

    public void Init(SkinReferences references)
    {
        References = references;
        OnInit();
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

    public void Receive(int count) => GetReward();

    public GachaResults GetReward()
    {
        if (UnlockState)
        {
            return Compensate();
        }
        else
        {
            Unlock();
            return GachaResults.New;
        }
    }

    public GachaResults Compensate()
    {
        if (IsSkillMaxLevel || Rarity == Rarity.Common)
        {
            Pair.Item.Add(Pair.Count, false);
            return GachaResults.Compensation;
        }
        else
        {
            SaveSerial.Instance.SaveParametr(SkillLevel + 1, equippedSkin: this);
            return GachaResults.Upgrade;
        }
    }

    public abstract void SkillActivation();

    public void Block()
    {
        UnlockState = false;
        SaveSerial.Instance.SaveSkinSkill(0, this);
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

    private void SetLockState()
    {
        avatar.sprite = _avatarLock;
        ironLock.enabled = true;
        outlight.enabled = false;
        outline.enabled = false;
        _button.interactable = false;
    }

    private void SetUnlockState()
    {
        avatar.sprite = _avatarUnlock;
        avatar.SetNativeSize();
        ironLock.enabled = false;
        outlight.enabled = true;
        outline.enabled = true;
        _button.interactable = true;
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
    Bonus,
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

