
public class IceCube : Bonus
{
    public override void Effect()
    {
        base.Effect();
        effect.Freezing();
        TrySpend();
    }
}
