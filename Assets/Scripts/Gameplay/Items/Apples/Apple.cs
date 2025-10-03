using System;

public abstract class Apple : Item
{
    private GenerationArea _generator;
    public CountdownToStart Countdown { get; protected set; }

    public void Init(GenerationArea generator, CountdownToStart countdown)
    {
        _generator = generator;
        Countdown = countdown;
    }

    public virtual void Activate() => gameObject.SetActive(true);

    public void Deactivate()
    {
        if (_generator == null)
            throw new NullReferenceException("Apple is not init");

        if(gameObject.activeSelf == false)
            return;

        _generator.RemoveApple(this);
        _generator.AppleGeneration();
    }
}
