using UnityEngine;

public class Mark : UIMonoBehaviour
{
    [SerializeField] private RectTransform[] _langFlags;

    public readonly Vector2 _downRightCorner = new Vector2(75, -46);

    private void Start() => MoveToNewFlag();

    public void MoveToNewFlag()
    {
        gameObject.transform.SetParent(_langFlags[(int)Language.Instance.Current]);
        rectTransform.anchoredPosition = _downRightCorner;
    }
}
