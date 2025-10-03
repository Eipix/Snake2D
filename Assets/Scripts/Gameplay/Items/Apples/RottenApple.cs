using DG.Tweening;
using System.Collections;
using UnityEngine;

public class RottenApple : Apple
{
    private Sequence _countdown;

    private void OnDisable()
    {
        _countdown.Kill();
        StopAllCoroutines();
    }

    public override void Activate()
    {
        base.Activate();
        StartCoroutine(DeactivateCountdown());
    }

    private IEnumerator DeactivateCountdown()
    {
        yield return new WaitUntil(() => Countdown.LevelStarted);

        _countdown = DOTween.Sequence()
           .AppendInterval(20f)
           .AppendCallback(() => Deactivate());
    }

    public override void Add(int count = 1, bool updateBalance = true)
    {
        throw new System.NotImplementedException();
    }
}
