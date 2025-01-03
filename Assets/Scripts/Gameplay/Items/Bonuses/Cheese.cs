
public class Cheese : Bonus
{
    public override void Effect()
    {
        slot.EnableSelected();
        StartCoroutine(effect.Cheese(this));
    }
}
