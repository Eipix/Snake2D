using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Iceik : Skin
{
    private SpriteRenderer _freezingArea;
    private ParticleSystem _freezingSmoke;

    private readonly int _defaultDuration = 2;
    private int _totalDuration;

    protected override void Start()
    {
        base.Start();

        if (SceneManager.GetActiveScene().name != "Level")
            return;

        _freezingArea = GetComponentInChildren<SpriteRenderer>();
        _freezingSmoke = _freezingArea.GetComponentInChildren<ParticleSystem>();

        _freezingArea.transform.position = Vector2.zero;

        _totalDuration = _defaultDuration + SkillLevel;
    }

    public override void SkillActivation()
    {
        References.Effects.Freezing(_totalDuration);
        StartCoroutine(FreezeArea());
    }

    private IEnumerator FreezeArea()
    {
        AreaEnabled();
        yield return new WaitUntil(() => References.Effects.IsFreezing == false);
        AreaDisabled();
    }

    private void AreaEnabled()
    {
        _freezingArea.enabled = true;
        _freezingSmoke.gameObject.SetActive(true);
        _freezingSmoke.Play();
    }

    private void AreaDisabled()
    {
        _freezingSmoke.Stop();
        _freezingArea.enabled = false;
        _freezingSmoke.gameObject.SetActive(false);
    }
}
