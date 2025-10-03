using UnityEngine;

public class DonatShop : Singleton<DonatShop>
{
    [SerializeField] private PunchablePopup _window;
    [SerializeField] private MoverTab _packsTab;
    [SerializeField] private OneTimeProduct _characterPack;

    public OneTimeProduct SnakePack => _characterPack;
    public MoverTab PacksTab => _packsTab;
    public PunchablePopup Window => _window;
}
