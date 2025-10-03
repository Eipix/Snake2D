using static SaveSerial;

public class DestroyHedgehogs : Achievement
{
    protected override string SaveFile => JsonPaths.DestroyedHedgehogs;
}
