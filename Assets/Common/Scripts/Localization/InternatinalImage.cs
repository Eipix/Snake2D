using UnityEngine;
using UnityEngine.UI;

public class InternatinalImage : International<Image, Sprite>
{
    public override Sprite Value
    {
        get => TypeToTranslate.sprite;
        set => TypeToTranslate.sprite = value;
    }
}
