using UnityEngine;
using UnityEngine.UI;
using TMPro;

public interface IArtChanger
{
    public Sprite FullScreenIcon { get; set; }
    public Sprite Light { get; set; }
    public TranslatableString LangDescription { get; set; }
    public AssetText LangHeadline { get; set; }

    public void ChangeArt(Image art, Image light, TMP_Text description, TextMeshProUGUI headline);
}
