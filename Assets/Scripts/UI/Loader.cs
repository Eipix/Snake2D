using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class Loader : MonoBehaviour
{
    [SerializeField] private Image _loadingSlider;
    [SerializeField] private TMP_Text _loadingText;

    public static float LastTimeScale = 1;

    public void OnEnable()
    {
        StartCoroutine(WaitAdClosing());

        if (NextScene.Scene == Scenes.Loader)
            NextScene.Scene = Scenes.Menu;

        StartCoroutine(LoadAsync());
    }

    private IEnumerator WaitAdClosing()
    {
        yield return new WaitWhile(() => YandexMarkups.IsAdOpen);
        Time.timeScale = 1;
        LastTimeScale = 1;
    }

    public IEnumerator LoadAsync()
    {
        SoundsPlayer.Instance.Stop();
        AsyncOperation _loadAsync = SceneManager.LoadSceneAsync((int)NextScene.Scene);
        NextScene.Scene = Scenes.Menu;

        _loadAsync.allowSceneActivation = false;
        _loadingSlider.fillAmount = 0;

        StartBlinkingText(-1);

        while (!_loadAsync.isDone)
        {
            if (_loadAsync.progress >= 0.9f && !_loadAsync.allowSceneActivation)
            {
                yield return _loadingSlider.DOFillAmount(_loadAsync.progress, 0.5f).WaitForCompletion();
                DOTween.CompleteAll();
                _loadAsync.allowSceneActivation = true;
                break;
            }
            yield return _loadingSlider.DOFillAmount(_loadAsync.progress, 1f).WaitForCompletion();
        }
    }

    private Sequence StartBlinkingText(int loops)
    {
        return DOTween.Sequence()
            .Append(_loadingText.DOFade(0, 1))
            .Append(_loadingText.DOFade(1, 1))
            .SetLoops(loops);
    }
}