using UnityEngine;
using TMPro;

public class Wallet : MonoBehaviour
{
    [SerializeField] private SaveSerial _saveSerial;

    public int RedAppleCount => _saveSerial.LoadApple().Item1;
    public int GoldAppleCount => _saveSerial.LoadApple().Item2;

    public bool TryGetRedApple(int count, MainMenuButtons menu = null)
    {
        if (count < 0)
            return false;

        int redApple = RedAppleCount + count;
        _saveSerial.SaveApple(redApple, GoldAppleCount);
        menu?.UpdateAppleBarsInMenu();
        return true;
    }

    public bool TryGetGoldApple(int count, TextMeshProUGUI balance = null)
    {
        if (count < 0)
            return false;

        int goldApple = GoldAppleCount + count;
        _saveSerial.SaveApple(RedAppleCount, goldApple);

        if(balance != null) balance.text = goldApple.ToString("D8");
        return true;
    }

    public bool TrySpentRedApple(int price)
    {
        if (price < 0 || RedAppleCount < price)
            return false;

        int redApple = RedAppleCount - price;
        _saveSerial.SaveApple(redApple, GoldAppleCount);
        return true;
    }

    public bool TrySpentGoldApple(int price)
    {
        if (price < 0 || GoldAppleCount < price)
            return false;

        int goldApple = GoldAppleCount - price;
        _saveSerial.SaveApple(RedAppleCount, goldApple);
        return true;
    }

    public void ResetRedApple()
    {
        _saveSerial.SaveApple(0, GoldAppleCount);
    }
}
