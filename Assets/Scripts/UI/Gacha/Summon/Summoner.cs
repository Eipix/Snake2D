using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.IO;
using System;

[RequireComponent(typeof(SummonAnimations))]
public class Summoner : SerializedMonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private GachaResult _results;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private BlackoutAnimation _blackout;
    [SerializeField] private BannerTab _defaultTab;
    [SerializeField] private VideoPlayer _videoPlayer;

    [SerializeField, FoldoutGroup("Sounds")] private AudioClip _summon;
    [SerializeField, FoldoutGroup("Sounds")] private AudioClip _ray;
    [SerializeField, FoldoutGroup("Sounds")] private AudioClip _commonReward;
    [SerializeField, FoldoutGroup("Sounds")] private AudioClip _premiumReward;

    public UnityEvent RewardsReceived;

    private List<IGachaReward> _rewards;
    private List<GachaResults> _statuses;

    private Dictionary<Rarity, SummonVideos> _clipUrls = new Dictionary<Rarity, SummonVideos>()
    {
        {Rarity.Bonus, new SummonVideos(MP4Paths.CommonStart, MP4Paths.CommonRay) },
        {Rarity.Common, new SummonVideos(MP4Paths.CommonStart, MP4Paths.CommonRay) },
        {Rarity.Epic, new SummonVideos(MP4Paths.EpicStart, MP4Paths.EpicRay) },
        {Rarity.Legendary, new SummonVideos(MP4Paths.LegendaryStart, MP4Paths.LegendaryRay) }
    };

    private System.Random _rand = new System.Random();
    private SummonAnimations _animation;
    public Coroutine Summoning { get; private set; }
    public BannerData Data { get; set; }

    public IEnumerable Rewards => _rewards;
    public IEnumerable Statuses => _statuses;

    private bool _isTap;

    private void Awake() => _animation = GetComponent<SummonAnimations>();

    public void OnPointerDown(PointerEventData eventData) => _isTap = true;

    public void OnPointerUp(PointerEventData eventData) => _isTap = false;

    [Button]
    private void DebugSummon(int count)
    {
        var rewards = GetRewards(count, out List<GachaResults> statuses);
        int common = rewards.Where(reward => reward.Rarity == Rarity.Common).Count();
        int epic = rewards.Where(reward => reward.Rarity == Rarity.Epic).Count();
        int legendary = rewards.Where(reward => reward.Rarity == Rarity.Legendary).Count();
        Debug.Log($"Common: {common}\nEpic: {epic}\nLegendary: {legendary}");
    }

    public void Skip()
    {
        if(Summoning == null)
        {
            Debug.Log("Summon coroutine is null");
            return;
        }

        SoundsPlayer.Instance.PlayMusic(SoundsPlayer.Instance.MenuTheme);
        StopCoroutine(Summoning);
        _videoPlayer.Stop();
        _animation.ElementsDisable();
        _isTap = false;
        _results.gameObject.SetActive(true);
         gameObject.SetActive(false);
    }

    public void Summon(int count, (List<IGachaReward> rewards, List<GachaResults> statuses) data = default)
    {
        if (Data == null)
            Data = _defaultTab.Data;

        if(data.rewards != null)
        {
            _rewards = data.rewards;
            _statuses = data.statuses;
        }
        else
        {
            _rewards = GetRewards(count, out _statuses);
        }

        Wallet.Instance.UpdateBalance();
        gameObject.SetActive(true);

        Summoning = StartCoroutine(SummonVideo(count));
    }

    public List<IGachaReward> GetRewards(int count, out List<GachaResults> statuses)
    {
        int currencyBefore = SaveSerial.Instance.LoadApple().Item1 + SaveSerial.Instance.LoadApple().Item2;
        List<IGachaReward> results = new List<IGachaReward>();
        statuses = new List<GachaResults>();

        for (int i = 0; i < count; i++)
        {
            var rarity = _rand.Element(Data.Chances);
            var rewards = Data.Rewards.Where(reward => reward.Rarity == rarity).ToArray();
            var reward = rewards[UnityEngine.Random.Range(0, rewards.Length)];
            var status = reward.GetReward();
            statuses.Add(status);
            results.Add(reward);
        }
        int currencyAfter = SaveSerial.Instance.LoadApple().Item1 + SaveSerial.Instance.LoadApple().Item2;
        int delta = currencyAfter - currencyBefore;

        SaveAchievementProgress();
        RewardsReceived?.Invoke();

        SaveSerial.Instance.SaveGame();
        if (delta > 0)
            LeaderBoard.Instance.Add(delta);

        return results;
    }

    private IEnumerator SummonVideo(int count)
    {
        SoundsPlayer.Instance.Pause();
        _rawImage.gameObject.SetActive(false);

        yield return _blackout.PlayOneShot(onFade: () => 
        {
            var rendertexture = _rawImage.texture as RenderTexture;
            rendertexture.Release();
            _rawImage.gameObject.SetActive(true);
        });

        var maxRarity = _rewards.Max(reward => reward.Rarity);
        yield return new WaitUntil(() => CachedVideos.IsCached);
        yield return PlayAndWaitFinish(_videoPlayer, _clipUrls[maxRarity].Start, _summon);

        foreach (var reward in _rewards)
        {
            yield return CurrentView(reward);
        }
        Skip();
    }

    private IEnumerator CurrentView(IGachaReward reward)
    {
        yield return PlayAndWaitFinish(_videoPlayer, _clipUrls[reward.Rarity].Ray, _ray);

        SoundsPlayer.Instance.PlayOneShotMusic(reward.Rarity > Rarity.Common ? _premiumReward : _commonReward);

        yield return _animation.PlayOneShot(reward);

        yield return new WaitUntil(() => _isTap);

        _isTap = false;
        _animation.ElementsDisable();
    }

    private IEnumerator PlayAndWaitFinish(VideoPlayer player, SummonVideos.VideoData video, AudioClip clip)
    {
        player.Stop();
        player.url = video.Url;
        yield return player.WaitPreparation();
        yield return player.WaitStarted();
        SoundsPlayer.Instance.Resume();
        SoundsPlayer.Instance.PlayOneShotMusic(clip);

        yield return new WaitWhile(() => player.isPlaying);
        player.Stop();
    }


    private void SaveAchievementProgress()
    {
        var prefabs = SaveSerial.Instance.SkinPrefabs;
        int rare = prefabs.Count(skin => skin.Rarity == Rarity.Common && skin.UnlockState);
        int epic = prefabs.Count(skin => skin.Rarity == Rarity.Epic && skin.UnlockState);
        int legendary = prefabs.Count(skin => skin.Rarity == Rarity.Legendary && skin.UnlockState);
        SaveSerial.Instance.Save(rare, SaveSerial.JsonPaths.CollectRareSkins);
        SaveSerial.Instance.Save(epic, SaveSerial.JsonPaths.CollectEpicSkins);
        SaveSerial.Instance.Save(legendary, SaveSerial.JsonPaths.CollectLegendarySkins);
    }
}
