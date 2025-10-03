using UnityEngine;

public class SkipButton : MonoBehaviour
{
    [SerializeField] private Summoner _summoner;
    [SerializeField] private BlackoutAnimation _skipBlackout;

    public void OnButtonClick()
    {
        _skipBlackout.PlayOneShot(
            () => _summoner.Skip(),
            () => _skipBlackout.gameObject.SetActive(false),
            true
            );
    }
}
