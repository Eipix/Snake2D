using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Coin : Bonus, IGachaReward
{
    [field: Header("IGachaReward")]
    [field: SerializeField] public Rarity Rarity { get; set; }
    [field: SerializeField] public Sprite Icon { get; set; }

    private Image _image;

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "Level")
        {
            SaveSerial.Instance.SaveBonusInSlots(this, -1);
        }
    }

    protected override void Start()
    {
        base.Start();
        _image = GetComponent<Image>();

        if (SceneManager.GetActiveScene().name == "Level")
        {
            Effect();
            effect.IncreaseGoldAppleChance();
            _image.color = new Color(_image.color.r * 0.5f, _image.color.g * 0.5f, _image.color.b * 0.5f);
            transform.parent.GetComponent<Button>().interactable = false;
        }
    }

    public override void Effect() { base.Effect(); }

    public GachaResults GetReward()
    {
        Add(1);
        return GachaResults.Bonus;
    }
}
