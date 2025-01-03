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

    public int _sceneIndex = 1;

    public void Start() => StartCoroutine(LoadAsync());

    public IEnumerator LoadAsync()
    {
        AsyncOperation _loadAsync = SceneManager.LoadSceneAsync(_sceneIndex);
        _loadAsync.allowSceneActivation = false;
        _loadingSlider.fillAmount = 0;

        DOTween.Sequence()
            .Append(_loadingText.DOFade(0, 1))
            .Append(_loadingText.DOFade(1, 1))
            .SetLoops(-1);

        while (!_loadAsync.isDone)
        {
            if (_loadAsync.progress >= 0.99f && !_loadAsync.allowSceneActivation)
            {
                yield return _loadingSlider.DOFillAmount(_loadAsync.progress, 0.5f).WaitForCompletion();
                DOTween.CompleteAll();
                _loadAsync.allowSceneActivation = true;
            }
            yield return _loadingSlider.DOFillAmount(_loadAsync.progress, 1f).WaitForCompletion();
        }
    }
}