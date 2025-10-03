using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[Serializable]
public class BannerData
{
    [ShowInInspector, OdinSerialize] public Dictionary<Rarity, float> Chances { get; private set; }
    [ShowInInspector, OdinSerialize] public List<IGachaReward> Rewards { get; private set; }
}
