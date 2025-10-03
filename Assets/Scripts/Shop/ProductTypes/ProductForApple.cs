using UnityEngine;

public class ProductForApple : Product
{
    [SerializeField] private int _price;

    public override void BuyProduct()
    {
        if (Wallet.Instance.IsEnoughGoldApples(_price))
        {
            ConfirmationNotification.Instance.Show(() =>
            {
                Wallet.Instance.TrySpentGoldApple(_price);
                Data.Add();
            });
        }
        else
        {
            Notification.Instance.Notify(Notification.Instance.LangNotEnough.Translate);
        }
    }
}
