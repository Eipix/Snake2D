using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionRequirements : MonoBehaviour
{
    [SerializeField] private Arena _arena;
    [SerializeField] private TextMeshProUGUI _requirements;
    [SerializeField] private Image[] _missionStars;
    [SerializeField] private Image _skull;

    [SerializeField] private PunchablePopup _missionPanel;
    [SerializeField] private GameObject _blackout;

    [Header("Sprites")]
    [SerializeField] private Sprite _bigStar;
    [SerializeField] private Sprite _bigVoidStar;
    [SerializeField] private Sprite _smallStar;
    [SerializeField] private Sprite _bossStar;

    public Arena Arena => _arena;
    public Level LastUnlock => SaveSerial.Instance.Levels.FirstOrDefault(level => level.IsCompleted == false && level.IsUnlock);

    public void Start()
    {
        Level[] levels = SaveSerial.Instance.Levels;
        LevelData[] datas = new LevelData[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            datas[i] = levels[i].Data;
        }
        SaveSerial.Instance.Datas = datas;

        RequirementMissionPopupDisable();
    }

    public void OnMissionClick(int levelIndex)
    {
        RequirementMissionPopupEnable();

        if(levelIndex < 0) SetArenaData();
        else SetLevelData(levelIndex);
    }

    private void SetLevelData(int levelIndex)
    {
        SaveSerial.Instance.Mode = LevelMode.Level;
        _skull.gameObject.SetActive(false);
        for (int i = 0; i < _missionStars.Length; i++)
        {
            _missionStars[i].gameObject.SetActive(true);
            _missionStars[i].sprite = SaveSerial.Instance.Load(levelIndex, SaveSerial.JsonPaths.LevelStars, new bool[3])[i]
                ? _bigStar
                : _bigVoidStar;
        }
        _requirements.text = SaveSerial.Instance.Levels[levelIndex].GetConditionText();
        SaveSerial.Instance.Data = SaveSerial.Instance.Levels[levelIndex].Data;
    }

    private void SetArenaData()
    {
        SaveSerial.Instance.Mode = LevelMode.Arena;
        _skull.gameObject.SetActive(true);

        for (int i = 0; i < _missionStars.Length; i++)
            _missionStars[i].gameObject.SetActive(false);

        _requirements.text = _arena.Description;
        SaveSerial.Instance.Waves = _arena.Waves;
        SaveSerial.Instance.Data = _arena.Waves[0];
    }

    public void RequirementMissionPopupEnable()
    {
        _missionPanel.Open();
        _blackout.SetActive(true);
    }

    public void RequirementMissionPopupDisable()
    {
        _missionPanel.Close();
        _blackout.SetActive(false);
    }
}