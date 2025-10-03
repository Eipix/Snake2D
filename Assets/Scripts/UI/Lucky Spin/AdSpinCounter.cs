using UnityEngine;

public class AdSpinCounter : Counter
{
    [SerializeField] private AdSpin _adSpin;

    public override void UpdateCount()
    {
        Circle.gameObject.SetActive(_adSpin.IsCooldownEnd() && _adSpin.SpinAvailable());
        Count.text = (_adSpin.MaxSpin - _adSpin.CompletedSpin).ToString();
    }

    protected override void OnEnable()
    {
        _adSpin.SpinSpent += UpdateCount;
        _adSpin.Restarted += UpdateCount;
        UpdateCount();
    }

    protected override void OnDisable()
    {
        _adSpin.SpinSpent -= UpdateCount;
        _adSpin.Restarted -= UpdateCount;
    }

}
