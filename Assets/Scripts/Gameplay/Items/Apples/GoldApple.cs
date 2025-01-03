

public class GoldApple : Apple
{
    public override void Add(int count = 1)
    {
       (int redApple, int goldApple) = SaveSerial.LoadApple();
        goldApple += count;
        SaveSerial.SaveApple(redApple, goldApple);
    }
}
