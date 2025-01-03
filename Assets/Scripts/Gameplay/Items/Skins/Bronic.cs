
public class Bronic : Skin
{
    private readonly int _defaultDuration = 4;

    public override void SkillActivation()
    {
        References.Effects.Shield(_defaultDuration + SkillLevel);
    }
}
