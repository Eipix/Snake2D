
public class Timer : Bonus
{
    public override void Effect()
    {
        effect.SlowDownEnemy();
        Spend();
    }
}
