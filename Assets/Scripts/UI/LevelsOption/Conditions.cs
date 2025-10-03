using System;

[Serializable]
public class Condition
{
    public LevelConditions Name;
    public TranslatableString Text;
    public int Value;
    public bool IsLessThan;

    public bool IsComplete(int currentValue)
    {
        if (IsLessThan) return currentValue <= Value;
        else return currentValue >= Value;
    }
}

public enum LevelConditions
{
    CollectApples,
    CollectGoldApples,
    LoseHealthNoMoreThan,
    AvoidHittingRocks,
    PassDistance,
    NoBomb,
    DistractHedgehogs,
    KillAnts,
    FreezeHedgehogs,
    HoldOutSeconds,
    PassLevelByEpicSkin,

    PoisonMouses,
    KillMouses,
    PassByRareSkin,
    PassByLegendarySkin,
    PoisonHedgehogs
}

