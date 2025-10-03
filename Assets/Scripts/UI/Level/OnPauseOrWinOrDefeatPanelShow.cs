using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using System.Linq;

public class OnPauseOrWinOrDefeatPanelShow : MonoBehaviour
{
    [SerializeField] private AdditionApples _additionApples;
    [SerializeField] private Loader _loader;
    [SerializeField] private ConditionsForLevel _conditions;
    [SerializeField] private Translatable<string> _translateLevel;

    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _defeatPanel;

    [SerializeField] private GameObject _areaUnlocked;
    [SerializeField] private GameObject _dropsFromAvatar;
    [SerializeField] private GameObject _dublicates;
    [SerializeField] private GameObject _blackout;
    [SerializeField] private GameObject _starsObject;
    [SerializeField] private Image _avatar;

    [SerializeField] private TextMeshProUGUI _currentLevel;
    [SerializeField] private TMP_Text _appleCounter;
    [SerializeField] private TMP_Text _goldAppleCounter;

    [SerializeField] private Button _nextLevel;

    public UnityEvent LevelCompleted;
    public UnityEvent Failed;

    private void Awake()
    {
        var datas = SaveSerial.Instance.Datas;
        int lastLevel = datas[datas.Length - 1].LevelIndex;

        if (SaveSerial.Instance.Data.LevelIndex == lastLevel)
        {
            _nextLevel.gameObject.SetActive(false);
            _areaUnlocked.SetActive(true);
            return;
        }
    }

    private void Start()
    {
        _currentLevel.text = $"{_translateLevel.Translate} <color=#BBFF00>{SaveSerial.Instance.Data.LevelIndex + 1}</color>";

        SetActiveDublicatePanelObjects(false);
        _dropsFromAvatar.SetActive(false);
        _starsObject.SetActive(false);

        _pausePanel.gameObject.SetActive(false);
        _defeatPanel.gameObject.SetActive(false);
        _winPanel.gameObject.SetActive(false);
    }

    public void Pause()
    {
        Loader.LastTimeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = Loader.LastTimeScale;
    }

    public void OnPauseClick()
    {
        SetActiveDublicatePanelObjects(true);
        _pausePanel.SetActive(true);
        UpdateApplePerLevel();
    }

    public void OnResumeClick()
    {
        SetActiveDublicatePanelObjects(false);
        _pausePanel.SetActive(false);
    }

    public void DefeatShow()
    {
        EndLevel();
        _dropsFromAvatar.SetActive(true);
        _defeatPanel.SetActive(true);
        Failed?.Invoke();
        Debug.Log("end level was saved in sdk");
#if UNITY_WEBGL && !UNITY_EDITOR
        SaveSerial.Instance.SaveGame();
#endif
    }

    public void WinShow()
    {
        EndLevel();
        _winPanel.SetActive(true);

        if (SaveSerial.Instance.Mode == LevelMode.Level)
        {
            (int completedLevels, int collectedStars) = CalculatePlayerProgress();
            SaveSerial.Instance.Save(collectedStars, SaveSerial.JsonPaths.CollectedStars);
            SaveSerial.Instance.Save(completedLevels, SaveSerial.JsonPaths.CompletedLevels);
            LevelCompleted?.Invoke();
        }

        Debug.Log("end level was saved in sdk");
        SaveSerial.Instance.SaveGame();
    }

    public (int,int) CalculatePlayerProgress()
    {
        int completedLevels = 0;
        int collectedStars = 0;
        int length = SaveSerial.Instance.Levels.Length;
        for (int i = 0; i < length; i++)
        {
            bool[] stars = SaveSerial.Instance.Load(i, SaveSerial.JsonPaths.LevelStars, new bool[3]);
            collectedStars += stars.Count(star => star == true);

            if (stars[0] == true)
                completedLevels++;
        }
        return (completedLevels, collectedStars);
    }

    public void OnInventoryClick()
    {
        PlayerPrefs.SetInt("EnterToInventory", 1);
        ExitTo(Scenes.Menu);
    }

    public void OnHomeClick()
    {
        PlayerPrefs.SetInt("Back", 1);
        ExitTo(Scenes.Menu);
    }

    public void OnTryAgainClick() => ExitTo(Scenes.Level);

    public void OnNextLevelClick()
    {
        var data = SaveSerial.Instance.Data;
        var datas = SaveSerial.Instance.Datas;
        int nextLevel = data.LevelIndex + 1;
        SaveSerial.Instance.Data = datas[nextLevel];
        Debug.LogWarning("Next Level");
        OnTryAgainClick();
    }

    public void EndLevel()
    {
        _conditions.CalculateStars();
        UpdateApplePerLevel();
        SetActiveDublicatePanelObjects(true);
        _starsObject.SetActive(true);
    }

    public void ExitTo(Scenes nextScene)
    {
        DOTween.CompleteAll();
        NextScene.Scene = nextScene;
        SceneManager.LoadSceneAsync((int)Scenes.Loader);
    }

    public void SetActiveDublicatePanelObjects(bool setActive)
    {
        if (setActive)
            Loader.LastTimeScale = Time.timeScale;

        Time.timeScale = setActive ? 0f : Loader.LastTimeScale;
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
