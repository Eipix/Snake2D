
public class SettingsSwiper : Swiper
{
    protected override void OnEnable()
    {
        base.OnEnable();
        if (Index != (int)Controller.Instance.Type)
        {
            Index = (int)Controller.Instance.Type;
            Move(BeforeMovePosition, 0f);
        }
    }

    protected override void BeforeMove()
    {
        base.BeforeMove();

        Controller.Instance.Type = (ControllerTypes)Index;
        SaveSerial.Instance.Save((int)Controller.Instance.Type, SaveSerial.JsonPaths.ControllerType);
    }
}
