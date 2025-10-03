using static SaveSerial;

public class CompleteLevels : Achievement
{
    protected override string SaveFile => JsonPaths.CompletedLevels;
}
