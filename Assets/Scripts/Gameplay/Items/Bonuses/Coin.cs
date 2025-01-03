using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Coin : Bonus
{
    private Image _image;

    private void OnDestroy()
    {
        if (SceneManager.GetActiveScene().name == "Level")
        {
            saveSerial.SaveBonusInSlots(this, -1);
        }
    }

    protected override void Start()
    {
        base.Start();
        _image = GetComponent<Image>();

        if (SceneManager.GetActiveScene().name == "Level")
        {
            effect.IncreaseGoldAppleChance();
            _image.color = new Color(_image.color.r * 0.5f, _image.color.g * 0.5f, _image.color.b * 0.5f);
            transform.parent.GetComponent<Button>().interactable = false;
        }
    }

    public override void Effect() { }
}
