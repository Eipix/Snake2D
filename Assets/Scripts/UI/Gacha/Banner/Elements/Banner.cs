using UnityEngine;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    [SerializeField] private Image _image;
    public RectTransform RectTransform => transform as RectTransform;
    public Image Image => _image;

    public float SizeX => Image.rectTransform.sizeDelta.x;
    public float PosX => Image.rectTransform.anchoredPosition.x;
}
