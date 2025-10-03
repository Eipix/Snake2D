using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class RequirementMissionButtons : MonoBehaviour
{
    [SerializeField] private MissionRequirements _levelClick;
    [SerializeField] private Loader _loader;
    [SerializeField] private RectTransform _indicator;

    private void OnEnable() => IndicatorMoveToLastLevel();

    public void OnPlayButtonClick()
    {
        DOTween.CompleteAll();
        NextScene.Scene = Scenes.Level;
        SceneManager.LoadSceneAsync((int)Scenes.Loader);
    }

    public void IndicatorMoveToLastLevel()
    {
        float[] defPosition = new float[] { _indicator.anchoredPosition.x, _indicator.anchoredPosition.y };
        float[] vector2 = SaveSerial.Instance.Load(SaveSerial.JsonPaths.IndicatorPosition, defPosition);
        _indicator.anchoredPosition = new Vector2(vector2[0], vector2[1]);

        var lastLevel = _levelClick.LastUnlock;

        var nextPosition = lastLevel == null
            ? _levelClick.Arena.Position + GetOffset(lastLevel)
            : lastLevel.Position + GetOffset(lastLevel);

        DOTween.Sequence().SetUpdate(true)
            .Append(_indicator.DOAnchorPos(nextPosition, 0.7f))
            .AppendCallback(() =>
            {
                float[] vector2 = new float[] { nextPosition.x, nextPosition.y };
                SaveSerial.Instance.Save(vector2, SaveSerial.JsonPaths.IndicatorPosition);
            });
    }

    public Vector2 GetOffset(Level level)
    {
        if(level == null)
        {
            return new Vector2(0, _levelClick.Arena.Offset);
        }
        return new Vector2(0, level.Offset);
    }
}
