using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.Video;

public class CachedVideos : Singleton<CachedVideos>
{
    [SerializeField] private VideoPlayer[] _videoPlayers;

    public static bool IsCached { get; private set; }

    private IEnumerator Start()
    {
        var fileNames = MP4Paths.GetAllFileNames().Where(name => name.StartsWith("Start")).ToArray();

        for (int i = 0; i < _videoPlayers.Length; i++)
        {
            _videoPlayers[i].url = $"{Application.streamingAssetsPath}/{fileNames[i]}";
            _videoPlayers[i].Play();
            yield return new WaitUntil(() => _videoPlayers[i].isPlaying);
        }

        foreach (var player in _videoPlayers)
        {
            yield return new WaitWhile(() => player.isPlaying);
        }

        _videoPlayers.ForEach(player => player.Stop());
        IsCached = true;
        Debug.Log("Video downloaded");
    }

    public static IEnumerator DownloadAndCache(string fileName)
    {
        string streamingPath = $"{Application.streamingAssetsPath}/{fileName}";
        string cachedPath = GetCachedVideoPath(fileName);

        if (File.Exists(cachedPath))
        {
            Debug.Log("File exist");
            yield break;
        }

        using (UnityWebRequest request = UnityWebRequest.Get(streamingPath))
        {
            request.downloadHandler = new DownloadHandlerFile(cachedPath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Download error");
                yield break;
            }
            Debug.Log($"Succes download {cachedPath}");
        }
        yield return null;
    }

    public static string GetCachedVideoPath(string fileName)
    {
        return $"{Application.streamingAssetsPath}/{fileName}";
    }
}
