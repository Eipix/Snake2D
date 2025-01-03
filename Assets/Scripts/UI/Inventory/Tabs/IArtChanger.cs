using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IArtChanger
{
    public Sprite IconArt { get; set; }
    public Sprite Light { get; set; }
    public Sprite Headline { get; set; }
    public string Description { get; set; }

    public void ChangeArt(ref Image art, ref Image name, ref Image light, ref TMP_Text description);
}
