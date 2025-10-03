using UnityEngine;
using UnityEngine.UI;

public class SwapCircle : MonoBehaviour
{
    [SerializeField] private Sprite _on;
    [SerializeField] private Sprite _off;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public SwapCircle Init()
    {
        _image = GetComponent<Image>();
        return this;
    }

    public SwapCircle On()
    {
        _image.sprite = _on;
        return this;
    }

    public SwapCircle Off()
    {
        _image.sprite = _off;
        return this;
    }
}
