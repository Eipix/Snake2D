
public class Peach : Bonus
{
    public override void Effect()
    {
        effect.Healing();
        Spend();
    }
}
