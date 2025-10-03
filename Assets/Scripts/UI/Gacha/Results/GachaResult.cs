using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GachaResult : MonoBehaviour
{
    [SerializeField] private SlotResult[] _slots;
    [SerializeField] private Button[] _buttons;
    [SerializeField] private Summoner _summoner;
    [SerializeField] private MainMenuButtons _menu;

    private List<IGachaReward> _rewards;
    private List<GachaResults> _statuses;

    private void OnEnable() => StartCoroutine(Show());

    private IEnumerator Show()
    {
        _buttons.ForEach(button => button.interactable = false);
        _slots.DisableAll();

        _rewards = (List<IGachaReward>)_summoner.Rewards;
        _statuses = (List<GachaResults>)_summoner.Statuses;

        for (int i = 0; i < _rewards.Count; i++)
        {
            DefineState(_rewards[i], _statuses[i], _slots[i]);
            yield return _slots[i].Punch();
        }

        Wallet.Instance.UpdateBalance();
        _buttons.ForEach(button => button.interactable = true);
    }

    private void DefineState(IGachaReward reward, GachaResults status, SlotResult slot)
    {
        Skin skin = reward is Skin Skin ? Skin : null;
        switch (status)
        {
            case GachaResults.Bonus:
                slot.Show(reward);
                break;
            case GachaResults.New:
                slot.New(reward);
                break;
            case GachaResults.Upgrade:
                    slot.DublicateUpgrade(reward, skin);
                break;
            case GachaResults.Compensation:
                    slot.AppleCompensation(reward, skin);
                break;
        }
        slot.gameObject.SetActive(true);
    }
}
