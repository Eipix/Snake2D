using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/*
 Preferable settings
 speed: 1000,
 turnCount: 9,
 turn step in degrees: 10,
 stopPower: 1.8
*/

public class LuckySpin : MonoBehaviour
{
    [SerializeField] private SpinItems[] _items;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private RectTransform _wheelContent;
    [SerializeField] private MainMenuButtons _menu;
    [SerializeField] private RewardPopup _rewardPopup;
    [SerializeField] private Notification _popup;
    [SerializeField] private Wallet _wallet;

    [Space()]
    [SerializeField] private int _price;

    [Space()][Range(1, 10)]
    [SerializeField] private float _stopPower;
    [SerializeField] private int _turnCount;
    [SerializeField] private float _speed;

    private float _startSpeed;
    private float _stopRatio;

    private readonly int _turnStepDegrees = 10;
    private readonly string _notEnoughApple = "Не достаточно яблок";

    private void Start()
    {
        _stopRatio = 1 - _stopPower / 100;
        _startSpeed = _speed;

        //CalculateMathExpectation();
    }

    private void CalculateMathExpectation()
    {
        SpinItems item;
        float Profit = 0f;
        int iterations = 1000;
        for (int i = 0; i < iterations; i++)
        {
            item = GetReward();
            Profit += item.EquivalentInRedApples;
            
        }
        Debug.LogWarning($"Gain: {Profit}");
        Debug.LogWarning($"Spent: {_price * iterations}");
        Debug.LogWarning($"MathExpectation: {Profit/(_price * iterations)}");
    }

    public void OnButtonClick()
    {  
        StartCoroutine(RotateWheel());
    }

    private IEnumerator RotateWheel()
    {
        if (_wallet.TrySpentRedApple(_price))
        {
            _menu.UpdateAppleBarsInMenu();
            DisableButtonsInteractable();

            _speed = _startSpeed;
            _wheelContent.rotation = Quaternion.identity;

            var item = GetReward();

            yield return MainSpins();
            yield return FinishSpin(item);
            yield return ShowReward(item);

            _menu.UpdateAppleBarsInMenu();
            EnableButtonsInteractable();
        }
        else
        {
            _popup.Notify(_notEnoughApple);
        }

    }

    private IEnumerator ShowReward(SpinItems item)
    {
        string description = $"{item.Item.name} x{item.Count}";
        _rewardPopup.ChangeValues(item.Item.Sprite, description, item.Item.Color);
        yield return _rewardPopup.Show(1f);
    }

    private void EnableButtonsInteractable()
    {
        foreach (var button in _buttons)
        {
            button.interactable = true;
        }
    }

    private void DisableButtonsInteractable()
    {
        foreach (var button in _buttons)
        {
            button.interactable = false;
        }
    }

    private IEnumerator MainSpins()
    {
        var oneTurnMultiplier = 360 / _turnStepDegrees;
        var partialTurns = _turnCount * oneTurnMultiplier;
        for (int i = 0; i < partialTurns; i++)
        {
            yield return TurnWheel(_turnStepDegrees);

            if (i > partialTurns * 0.6f)
                _speed *= _stopRatio;
        }
    }

    private IEnumerator FinishSpin(SpinItems item)
    {
        if (item.InBoundaries(_wheelContent.rotation.eulerAngles.z) == false)
        {
            float finishScrewing = Random.Range(item.RotationZBoundaries.x, item.RotationZBoundaries.y);
            float absScrewing = Mathf.Abs(finishScrewing);
            yield return TurnWheel(absScrewing);
        }
        yield return _wheelContent.DOPunchRotation(Vector3.forward * 5, 0.3f, 10, 3f);
    }

    private YieldInstruction TurnWheel(float targetZ)
    {
        return _wheelContent.DORotate(Vector3.forward * targetZ, _speed)
                       .SetEase(Ease.Linear)
                       .SetRelative()
                       .SetSpeedBased()
                       .WaitForCompletion();
    }

    private SpinItems GetReward()
    {
        System.Random rand = new System.Random();
        Dictionary<SpinItems, int> rotations = new Dictionary<SpinItems, int>();
        foreach (var item in _items)
        {
            rotations.Add(item, item.Probability);
        }
        var reward = rand.Element(rotations);
        reward.Item.Add(reward.Count);
        return reward;
    }
}
