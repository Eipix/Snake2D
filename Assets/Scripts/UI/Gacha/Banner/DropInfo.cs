using UnityEngine;
using UnityEngine.UI;

public class DropInfo : MonoBehaviour
{
    private ScrollRect[] _dropTables;
    public ScrollRect Current { get; set; }

    private void Awake() => _dropTables = GetComponentsInChildren<ScrollRect>(true);

    private void OnEnable()
    {
        if (Current == null)
            Current = _dropTables[0];

        Current.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if(Current != null)
        {
            Current.gameObject.SetActive(false);
        }
    }

    public void SetDropTable(ScrollRect dropTable)
    {
        Current = dropTable;
    }
}
