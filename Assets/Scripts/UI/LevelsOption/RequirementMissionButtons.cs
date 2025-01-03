using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RequirementMissionButtons : MonoBehaviour
{
    [SerializeField] private MissionRequirements _levelClick;
    [SerializeField] private Loader _loader;
    [SerializeField] private RectTransform _indicator;

    private SaveSerial _saveSerial;

    private void Start()
    {       
        _saveSerial = _loader.GetComponent<SaveSerial>();
        IndicatorMoveToLastLevel();      
    }

    public void OnPlayButtonClick()
    {
        DOTween.CompleteAll();
        _loader._sceneIndex = 2;
        AsyncOperation _loadAsync = SceneManager.LoadSceneAsync(0);
    }

    public void IndicatorMoveToLastLevel()
    {
        if (_saveSerial.LoadIndicatorPosition() != Vector2.zero)
            _indicator.anchoredPosition = _saveSerial.LoadIndicatorPosition();

        DOTween.Sequence()
               .Append(_indicator.DOAnchorPos(_levelClick.GetLastUnlockPosition(), 0.7f))
               .OnComplete(() => _saveSerial.SaveIndicatorPositions(_indicator.anchoredPosition));
    }

    public void OnCrossClick()
    {
        _levelClick.RequirementMissionPopupDisable();
    }

}
