
public class Timer : Bonus
{
    public override void Effect()
    {
        base.Effect();
        effect.SlowDownEnemy();
        TrySpend();
    }
}
