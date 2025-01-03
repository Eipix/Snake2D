

public class RedApple : Apple
{
    public override void Add(int count = 1)
    {
        (int redApple, int goldApple) = SaveSerial.LoadApple();
        redApple += count;
        SaveSerial.SaveApple(redApple, goldApple);
    }
}
