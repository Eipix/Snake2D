using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopArt : MonoBehaviour
{
    [Header("Art")]
    [SerializeField] private Image _art;
    [SerializeField] private Image _light;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _headline;
    [SerializeField] private float _sizeMiltiplier;

    public void Set(IArtChanger item)
    {
        item.ChangeArt(_art, _light, _description, _headline);
        _art.rectTransform.sizeDelta *= _sizeMiltiplier;
    }
}
