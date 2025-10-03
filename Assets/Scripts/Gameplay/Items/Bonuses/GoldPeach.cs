
using UnityEngine;

public class GoldPeach : Bonus, IGachaReward
{
    [field: Header("IGachaReward")]
    [field: SerializeField] public Rarity Rarity { get; set; }
    [field: SerializeField] public Sprite Icon { get; set; }

    public override void Effect()
    {
        base.Effect();
        effect.Shield(isPeach: true);
        TrySpend();
    }

    public GachaResults GetReward()
    {
        Add(1);
        return GachaResults.Bonus;
    }
}
