
public class Lightning : Bonus
{
    public override void Effect()
    {
        base.Effect();
        effect.Acceleration();
        TrySpend();
    }
}
