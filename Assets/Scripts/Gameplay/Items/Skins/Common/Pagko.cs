
public class Pagko : Skin
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Unlock();
    }

    protected override void Start()
    {
        CheckSelected();
    }

    public override void SkillActivation() { }
    
    private void CheckSelected()
    {
        if (SaveSerial.Instance.LoadCurrentSkinType() == "")
            SaveSerial.Instance.SaveCurrentSkin(this);
    }
}
