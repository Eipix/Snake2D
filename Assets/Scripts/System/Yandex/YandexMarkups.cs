using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class YandexMarkups : Singleton<YandexMarkups>
{
    [SerializeField] private DonateProductData[] _products;

    public static bool IsAdOpen;

    private Texture2D _currency;
    public bool IsInit { get; private set; }

    public Texture2D Currency => _currency;
    public DateTime ServerTime => DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(GetServerTimeExtern()))
                                              .LocalDateTime;

    [DllImport("__Internal")]
    public static extern void InitSDK();

    [DllImport("__Internal")]
    public static extern void GetCurrencyIcon();

    [DllImport("__Internal")]
    public static extern void GetPurchasePrice(string id, string objectPath, string methodName);

    [DllImport("__Internal")]
    public static extern void BuyProduct(string id, string objectPath, string methodName);

    [DllImport("__Internal")]
    public static extern void GetPurchase(string id, string objectPath, string methodName);

    [DllImport("__Internal")]
    public static extern void CheckPurchase(string id);

    [DllImport("__Internal")]
    public static extern void ShowFullScreenAd();

    [DllImport("__Internal")]
    public static extern void RewardedAds(string pathToObject, string onRewardedMethodName, string onCloseMethodName);

    [DllImport("__Internal")]
    public static extern string GetServerTimeExtern();

    [DllImport("__Internal")]
    private static extern void GameReady();

    public override void OnInit()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            InitSDK();
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if (IsInit == false)
            return;

        if (focus)
            SoundsPlayer.Instance.Resume();
        else
            SoundsPlayer.Instance.Pause();
    }

    private void OnApplicationPause(bool pause)
    {
        if (IsInit == false)
            return;

        if (pause)
            SoundsPlayer.Instance.Pause();
        else
            SoundsPlayer.Instance.Resume();
    }

    [Button]
    private void AdOpenClose(bool active) => IsAdOpen = active;

    public void OnFullScreenAdClose() => IsAdOpen = false;

    //invoke extern
    public void OnSDKInit()
    {
        StartCoroutine(SaveSerial.WaitInitCoroutine());
        StartCoroutine(Language.WaitInitCoroutine());

        GameReady();
        GetCurrencyIcon();
        foreach (var product in _products)
        {
            string id = ((int)product.Type).ToString();
            CheckPurchase(id);
        }

        IsInit = true;
    }

    public void ShowRewardedAd(string pathToObject, string onRewardMethodName, string onCloseMethodName, Action onEditor)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        RewardedAds(pathToObject, onRewardMethodName, onCloseMethodName);
#elif UNITY_EDITOR
        onEditor.Invoke();
#endif
    }

    public void ShowBannerAd()
    {
        Debug.Log("Ad Open");
#if UNITY_WEBGL && !UNITY_EDITOR
        IsAdOpen = true;
        Time.timeScale = 0;
        ShowFullScreenAd();
#endif
    }

    //invoke extern by CheckPurchase
    public void ConsumePurchase(string id)
    {
        int intID = int.Parse(id);
        var product = _products.Where(product => (int)product.Type == intID).FirstOrDefault();

        if (product == null)
            Debug.LogError($"product is null, type {product.Type}, index {(int)product.Type}, intID {intID}, enum {(Goods)intID}");

        product.Add();
    }

    public void SetIcon(string url)
    {
        StartCoroutine(DownloadImage(url));
    }

    private IEnumerator DownloadImage(string mediaUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(mediaUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            Debug.LogWarning(request.error);
        else
            _currency = ((DownloadHandlerTexture)request.downloadHandler).texture;
    }
}
