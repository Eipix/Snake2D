

public class GoldApple : Apple, IProduct
{
    public override void Add(int count = 1, bool updateBalance = true)
    {
        Wallet.Instance.TryGetGoldApple(count, updateBalance);
    }

    public void Receive(int count) => Wallet.Instance.TryGetGoldApple(count);
}
