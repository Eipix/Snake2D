

public interface IGachaCompensation
{
    public ItemCountPair Pair { get; set; }

    public GachaResults Compensate();
}
