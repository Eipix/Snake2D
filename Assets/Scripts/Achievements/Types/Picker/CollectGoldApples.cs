using static SaveSerial;

public class CollectGoldApples : Achievement
{
    protected override string SaveFile => JsonPaths.CollectedGoldApples;
}
