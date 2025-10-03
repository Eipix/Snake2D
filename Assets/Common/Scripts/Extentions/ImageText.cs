using UnityEngine.UI;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Image))]
public class ImageText : MonoBehaviour
{
    public Image Image { get; private set; }
    public TextMeshProUGUI Text { get; private set; }

    private void Awake()
    {
        Image = GetComponent<Image>();
        Text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
