using TMPro;
using UnityEngine;

public class BalanceInitializer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _redAppleBar;
    [SerializeField] private TextMeshProUGUI _goldAppleBar;

    private void Start()
    {
        Wallet.Instance.Init(_redAppleBar, _goldAppleBar);
    }
}
