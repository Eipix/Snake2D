using UnityEngine;

public class PanelController : MonoBehaviour
{
    public void OnCloseButtonClick(GameObject panel) => panel.SetActive(false);

    public void OnOpenButtonClick(GameObject panel) => panel.SetActive(true);
}
