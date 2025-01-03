
public class Bomb : Bonus
{
    public override void Effect()
    {
        slot.EnableSelected();
        StartCoroutine(effect.DropBomb(this));
    }
}
