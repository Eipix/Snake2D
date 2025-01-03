using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionRequirements : MonoBehaviour
{
    [SerializeField] private RectTransform _indicator;
    [SerializeField] private Arena _arena;
    [SerializeField] private TextMeshProUGUI _requirements;
    [SerializeField] private Image[] _missionStars;
    [SerializeField] private Image _skull;
    [SerializeField] private SaveSerial _saveSerial;
    [SerializeField] private List<Level> _levels;

    [SerializeField] private GameObject _missionPanel;
    [SerializeField] private GameObject _blackout;

    [Header("Sprites")]
    [SerializeField] private Sprite _bigStar;
    [SerializeField] private Sprite _bigVoidStar;
    [SerializeField] private Sprite _smallStar;
    [SerializeField] private Sprite _bossStar;

    public void Start()
    {
        Level[] levels = _levels.ToArray();
        LevelData[] datas = new LevelData[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            datas[i] = levels[i].Data;
        }
        _saveSerial.CacheAllLevelsData(datas);

        RequirementMissionPopupDisable();
    }

    public void OnMissionClick(int levelNumber)
    {
        RequirementMissionPopupEnable();

        if(levelNumber == -1) SetArenaData();
        else SetLevelData(levelNumber);
    }

    private void SetLevelData(int levelNumber)
    {
        _saveSerial.ArenaMode = false;
        _skull.gameObject.SetActive(false);
        for (int i = 0; i < _missionStars.Length; i++)
        {
            _missionStars[i].gameObject.SetActive(true);
            _missionStars[i].sprite = (bool)_saveSerial.LoadStars(levelNumber - 1).GetValue(i)
                ? _bigStar
                : _bigVoidStar;
        }
        _requirements.text = _levels[levelNumber - 1].GetConditionText();
        _saveSerial.Data = _levels[levelNumber - 1].Data;
    }

    private void SetArenaData()
    {
        _saveSerial.ArenaMode = true;
        _skull.gameObject.SetActive(true);

        for (int i = 0; i < _missionStars.Length; i++)
            _missionStars[i].gameObject.SetActive(false);

        _requirements.text = _arena.GetConditionText();
        _saveSerial.Waves = _arena.Waves;
        _saveSerial.Data = _arena.Waves[0];
    }

    public Vector2 GetLastUnlockPosition()
    {
        var lastUnlock = _levels.Where(level => level.IsComplete(level.Number) == false && level.IsUnlock).FirstOrDefault();

        if (lastUnlock == null)
            return new Vector2(_arena.RectTransform.anchoredPosition.x, _arena.RectTransform.anchoredPosition.y + 160f);

        Vector2 target = new Vector2(lastUnlock.AnchoredPosition.x, lastUnlock.AnchoredPosition.y + lastUnlock.Offset);
        return target;
    }
   
    public void RequirementMissionPopupEnable()
    {
        _missionPanel.SetActive(true);
        _blackout.SetActive(true);
    }

    public void RequirementMissionPopupDisable()
    {
        _missionPanel.SetActive(false);
        _blackout.SetActive(false);
    }

    public List<Level> GetLevelList() => _levels;
}