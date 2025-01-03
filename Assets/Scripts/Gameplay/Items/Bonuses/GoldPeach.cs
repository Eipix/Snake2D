
public class GoldPeach : Bonus
{
    public override void Effect()
    {
        effect.Shield(isPeach: true);
        Spend();
    }
}
