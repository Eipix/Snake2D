using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SoundsPlayer : Singleton<SoundsPlayer>
{
    [SerializeField] private AudioClip _menuTheme;
    [SerializeField] private AudioClip _button;

    [SerializeField] private VolumeSlider _music;
    [SerializeField] private VolumeSlider _sound;

    public AudioClip Button => _button;
    public AudioClip MenuTheme => _menuTheme;

    private void OnEnable() => SaveSerial.Instance.DataLoaded += LoadAll;
    private void OnDisable() => SaveSerial.Instance.DataLoaded -= LoadAll;

    private void Start()
    {
        LoadAll();
        _music.Slider.onValueChanged.AddListener(volume => ChangeVolume(_music, volume));
        _sound.Slider.onValueChanged.AddListener(volume => ChangeVolume(_sound, volume));
    }

    private void LoadAll()
    {
        Load(_music);
        Load(_sound);
    }

    private void Load(VolumeSlider volumeSlider)
    {
        volumeSlider.AudioSource.volume = SaveSerial.Instance.Load(volumeSlider.Key, SaveSerial.JsonPaths.Volumes, 0.5f);
        volumeSlider.Slider.value = volumeSlider.AudioSource.volume;
    }

    public void PlayMusic(AudioClip clip)
    {
        PlayOneShotMusic(clip);
        _music.AudioSource.loop = true;
    }

    public void PlayOneShotMusic(AudioClip clip)
    {
        _music.AudioSource.clip = clip;
        _music.AudioSource.Play();
        _music.AudioSource.loop = false;
    }

    public void Pause(int isMute = 0)
    {
        isMute = Math.Clamp(isMute, 0, 1);
        if(isMute == 1)
        {
            _music.AudioSource.mute = true;
            Debug.Log($"Paused {_music.AudioSource.mute}");
        }

        _music.AudioSource.Pause();
        _sound.AudioSource.Pause();
    }
    
    public void Resume(int isMute = 0)
    {
        isMute = Math.Clamp(isMute, 0, 1);
        if (isMute == 1)
        {
            _music.AudioSource.mute = false;
            Debug.Log($"Resumed {!_music.AudioSource.mute}");
        }

        if (_music.AudioSource.mute)
            return;

        _music.AudioSource.UnPause();
        _sound.AudioSource.UnPause();
    }

    public void Stop()
    {
        _music.AudioSource.Stop();
        _sound.AudioSource.Stop();
    }

    public void PlayOneShotSound(AudioClip clip) => _sound.AudioSource.PlayOneShot(clip);

    public Coroutine PlayWhile(AudioClip clip, Func<bool> condition, AudioSource audioSource)
    {
        return StartCoroutine(WaitEnd(clip, condition, audioSource));
    }

    private IEnumerator WaitEnd(AudioClip clip, Func<bool> condition, AudioSource audioSource)
    {
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.volume = _sound.AudioSource.volume;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
        yield return new WaitWhile(condition);
        audioSource.Stop();
        Destroy(audioSource);
    }

    private void ChangeVolume(VolumeSlider volumeSlider, float volume)
    {
        volumeSlider.AudioSource.volume = volume;
        SaveSerial.Instance.Save((volumeSlider.Key, volume), SaveSerial.JsonPaths.Volumes);
    }

    [Serializable]
    private struct VolumeSlider
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Slider _slider;
        [SerializeField] private string _key;

        public AudioSource AudioSource => _audioSource;
        public Slider Slider => _slider;
        public string Key => _key;
    }
}
