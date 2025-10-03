using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SwitcherTab : Tab
{
    public override void On()
    {
        base.On();
        Content.gameObject.SetActive(true);
    }

    public override void Off()
    {
        base.Off();
        Content.gameObject.SetActive(false);
    }
}
