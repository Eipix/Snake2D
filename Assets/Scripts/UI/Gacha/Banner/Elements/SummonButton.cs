using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SummonButton : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private Summoner _summoner;
    //[SerializeField] private Notification _popup;
    [field:SerializeField] public SpinData SpinData { get; private set; }
    [SerializeField] private int _summonCount;

    public Image Image => _image;
    public Button Button => _button;

    public void OnButtonClick()
    {
        var price = SpinData.Price;
        var apple = SpinData.Apple;

        if (Wallet.Instance.TrySpentApples(apple as Apple, price))
        {
            _summoner.Summon(_summonCount);
        }
        else
        {
            Notification.Instance.Notify(Notification.Instance.LangNotEnough.Translate);
        }
    }
}
