using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIApple : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private Wallet _wallet;
    private MainMenuButtons _menu;
    private UIAppleSpawner _spawner;
    private Sequence _bounceEffect;

    private void OnEnable()
    {
        transform.localScale = Vector2.one;

        _bounceEffect = DOTween.Sequence()
            .OnKill(() => DOTween.Sequence()
                                 .Append(transform.DOScale(0f, 0.3f))
                                 .AppendCallback(() => {
                                     _spawner.RespawnCoroutine();
                                     gameObject.SetActive(false);
                                 }))
            .AppendInterval(1f)
            .OnStart(() => transform.DOPunchScale(Vector2.one * 0.5f, 0.3f, vibrato: 1))
            .Append(transform.DOPunchScale(Vector2.one * 0.3f, 0.3f, vibrato: 1).SetLoops(2))
            .AppendInterval(3f)
            .SetLoops(-1);

        StartCoroutine(Disabling());
    }

    public void Init(Wallet wallet, MainMenuButtons menu, UIAppleSpawner spawner)
    {
        _wallet = wallet;
        _menu = menu;
        _spawner = spawner;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _wallet.TryGetRedApple(1, _menu);
        _bounceEffect.Kill();
    }

    public void OnPointerDown(PointerEventData eventData) { }
    
    private IEnumerator Disabling()
    {
        yield return new WaitForSeconds(10f);
        _bounceEffect.Kill();
    }
}
