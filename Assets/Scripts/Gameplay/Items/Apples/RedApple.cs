


public class RedApple : Apple, IProduct
{
    public override void Add(int count = 1, bool updateBalance = true)
    {
        Wallet.Instance.TryGetRedApple(count, updateBalance);
    }

    public void Receive(int count) => Wallet.Instance.TryGetRedApple(count);

}
