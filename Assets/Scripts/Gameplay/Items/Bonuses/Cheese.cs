
public class Cheese : Bonus
{
    public override void Effect()
    {
        base.Effect();
        slot.EnableSelected();
        StartCoroutine(effect.Cheese(this));
    }
}
