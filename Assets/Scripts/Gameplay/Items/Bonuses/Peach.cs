
public class Peach : Bonus
{
    public override void Effect()
    {
        base.Effect();
        effect.Healing();
        TrySpend();
    }
}
