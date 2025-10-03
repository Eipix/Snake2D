using UnityEngine;

public interface IGachaReward
{
    public Sprite FullScreenIcon { get; set; }
    public Sprite Icon { get; set; }

    public AssetText LangHeadline { get; set; }
    public Rarity Rarity { get; set; }

    public GachaResults GetReward();
}
