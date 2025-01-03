using DG.Tweening;

public class RottenApple : Apple
{
    private void OnEnable()
    {
        DOTween.Sequence()
            .AppendInterval(20f)
            .AppendCallback(() => Deactivate());
    }

    public override void Add(int count = 1)
    {
        throw new System.NotImplementedException();
    }
}
