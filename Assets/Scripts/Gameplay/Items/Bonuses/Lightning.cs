
public class Lightning : Bonus
{
    public override void Effect()
    {
        effect.Acceleration();
        Spend();
    }
}
