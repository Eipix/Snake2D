using Sirenix.OdinInspector;
using UnityEngine;

public class SingletonListener : MonoBehaviour
{
    public void ShowBannerAd() => YandexMarkups.Instance.ShowBannerAd();

    public void OpenSettings() => Settings.Instance.Popup.Open();

    public void OpenDonatShop() => DonatShop.Instance.Window.Open();

    [Button]
    public void TimeScale(float time) => Time.timeScale = time;

    public void Resume() => Time.timeScale = Loader.LastTimeScale;

    public void PlayMusic(AudioClip clip) => SoundsPlayer.Instance.PlayMusic(clip);

    public void PlayOneShotSound(AudioClip clip) => SoundsPlayer.Instance.PlayOneShotSound(clip);
}
