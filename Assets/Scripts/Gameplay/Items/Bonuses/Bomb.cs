
public class Bomb : Bonus
{
    public override void Effect()
    {
        base.Effect();
        slot.EnableSelected();
        StartCoroutine(effect.DropBomb(this));
    }
}
