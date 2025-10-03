
public class Bronic : Skin
{
    private readonly int _defaultDuration = 4;

    public Translatable<string> _langSecond = new Translatable<string>
    {
        Translated = new string[3] { "sec.", "сек.", "sn." }
    };

    public override string CurrentLevelText => $"{_defaultDuration + SkillLevel} {_langSecond.Translate}";
    public override string NextLevelText => $"<color=green> +1 {_langSecond.Translate}</color>";

    public override void SkillActivation()
    {
        References.Effects.Shield(_defaultDuration + SkillLevel);
    }
}
