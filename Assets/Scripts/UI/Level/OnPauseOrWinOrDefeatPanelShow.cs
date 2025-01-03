using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OnPauseOrWinOrDefeatPanelShow : MonoBehaviour
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Loader _loader;
    [SerializeField] private ConditionsForLevel _conditions;

    [SerializeField] private GameObject _dropsFromAvatar;
    [SerializeField] private GameObject _dublicates;
    [SerializeField] private GameObject _blackout;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _defeatPanel;
    [SerializeField] private GameObject _starsObject;
    [SerializeField] private Image _avatar;

    [SerializeField] private TMP_Text _appleCounter;
    [SerializeField] private TMP_Text _goldAppleCounter;

    [SerializeField] private Button _nextLevel;

    private SaveSerial _saveSerial;

    private float _lastTimeScale = 1f;

    private void Start()
    {
        _saveSerial = _loader.GetComponent<SaveSerial>();

        SetActiveDublicatePanelObjects(false);
        _dropsFromAvatar.SetActive(false);
        _starsObject.SetActive(false);

        _pausePanel.SetActive(false);
        _defeatPanel.SetActive(false);
        _winPanel.SetActive(false);
    }

    public void OnPauseClick()
    {
        SetActiveDublicatePanelObjects(true);
        _pausePanel.SetActive(true);
        UpdateApplePerLevel();
    }

    public void OnPauseCrossClick()
    {
        SetActiveDublicatePanelObjects(false);
        _pausePanel.SetActive(false);
    }

    public void DefeatShow()
    {
        _conditions.StarsCheck();
        UpdateApplePerLevel();
        SetActiveDublicatePanelObjects(true);
        _starsObject.SetActive(true);
        _dropsFromAvatar.SetActive(true);
        _defeatPanel.SetActive(true);
    }

    public void WinShow()
    {
        _conditions.StarsCheck();
        UpdateApplePerLevel();
        SetActiveDublicatePanelObjects(true);
        _starsObject.SetActive(true);
        _winPanel.SetActive(true);       
    }

    public void OnHomeClick()
    {
        Time.timeScale = 1;
        _loader._sceneIndex = 1;
        DOTween.CompleteAll();
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(0); 
    }

    public void OnBackClick()
    {
        PlayerPrefs.SetInt("Back", 1);
        Time.timeScale = 1;
        _loader._sceneIndex = 1;
        DOTween.CompleteAll();
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(0);
    }

    public void OnTryAgainClick()
    {
        Time.timeScale = 1;
        _loader._sceneIndex = 2;
        DOTween.CompleteAll();
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(0);
    }

    public void OnNextLevelClick()
    {
        int lastLevel = _saveSerial.Datas[_saveSerial.Datas.Length - 1].LevelIndex;
        if (_saveSerial.Data.LevelIndex == lastLevel)
        {
            _nextLevel.gameObject.SetActive(false);
            return;
        }

        int nextLevel = _saveSerial.Data.LevelIndex + 1;
        _saveSerial.Data = _saveSerial.Datas[nextLevel];
        Debug.LogWarning("Next Level");
        OnTryAgainClick();
    }

    public void SetActiveDublicatePanelObjects(bool setActive)
    {
        if (setActive)
            _lastTimeScale = Time.timeScale;

        Time.timeScale = setActive ? 0f : _lastTimeScale;
        _blackout.SetActive(setActive);
        _dublicates.SetActive(setActive);
        _avatar.SetNativeSize();
    }

    public void UpdateApplePerLevel()
    {
        _appleCounter.text = _additionApples.NominallyRedAppleCollected.ToString();
        _goldAppleCounter.text = _additionApples.ActuallyGoldAppleCollected.ToString();
    }
}
