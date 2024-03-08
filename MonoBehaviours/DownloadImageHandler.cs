using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class DownloadImageHandler : MonoBehaviour
{
    private Sprite result;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Download a single image
    /// </summary>
    /// <param name="url">Image url</param>
    /// <param name="callback"> Callback when the download is completed </param>
    public static void Download(string url, Action<Sprite> callback)
    {
        DownloadImageHandler handler = new GameObject("Image Downloader").AddComponent<DownloadImageHandler>();
        handler.StartCoroutine(handler.DownloadImage(url, callback));
    }

    /// <summary>
    /// Downlaod a single image
    /// </summary>
    /// <typeparam name="T"> Type of user data </typeparam>
    /// <param name="url"> Image url </param>
    /// <param name="data"> User data, can be anything </param>
    /// <param name="callback"> Callback when the download is completed </param>
    public static void Download<T>(string url, T data, Action<Sprite, T> callback)
    {
        DownloadImageHandler handler = new GameObject("Image Downloader").AddComponent<DownloadImageHandler>();
        handler.StartCoroutine(handler.DownloadImage(url, data, callback));
    }

    /// <summary>
    /// Download multiple images
    /// </summary>
    /// <param name="urlsWithIndividualCallbacks"> urls of all the images to download. Each entry in this enumerable should be a tuple indicating (string url, Action&lt;Sprite&gt; onImageDownloadedFromGivenUrl) </param>
    /// <param name="onAllDownloaded">Callback when all images are downloaded</param>
    public static void Download(IEnumerable<(string, Action<Sprite>)> urlsWithIndividualCallbacks, Action onAllDownloaded)
    {
        DownloadImageHandler handler = new GameObject("Image Downloader").AddComponent<DownloadImageHandler>();
        handler.StartCoroutine(handler.DownloadImages(urlsWithIndividualCallbacks, onAllDownloaded));
    }

    /// <summary>
    /// Download multiple images
    /// </summary>
    /// <param name="urlsWithIndividualCallbacks"> urls of all the images to download. Each entry in this enumerable should be a tuple indicating (string url, T userData, Action&lt;Sprite, T&gt; onImageDownloadedFromGivenUrl) </param>
    /// <param name="onAllDownloaded">Callback when all images are downloaded</param>
    public static void Download<T>(IEnumerable<(string, T, Action<Sprite, T>)> urlsWithIndividualCallbacks, Action onAllDownloaded)
    {
        DownloadImageHandler handler = new GameObject("Image Downloader").AddComponent<DownloadImageHandler>();
        handler.StartCoroutine(handler.DownloadImages(urlsWithIndividualCallbacks, onAllDownloaded));
    }


    private IEnumerable InternalDownloader(string url)
    {
        result = null;
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError(uwr.error);
                result = null;
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                result = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            }
        }
    }

    private IEnumerator DownloadImages(IEnumerable<(string, Action<Sprite>)> urlsWithIndividualCallbacks, Action onAllDownloaded)
    {
        foreach ((string, Action<Sprite>) tuple in urlsWithIndividualCallbacks)
        {
            foreach (var item in InternalDownloader(tuple.Item1)) yield return item;
            tuple.Item2?.Invoke(result);
        }

        onAllDownloaded?.Invoke();
        Destroy(this.gameObject);
    }

    private IEnumerator DownloadImages<T>(IEnumerable<(string, T, Action<Sprite, T>)> urlsWithIndividualCallbacks, Action onAllDownloaded)
    {
        foreach ((string, T, Action<Sprite, T>) tuple in urlsWithIndividualCallbacks)
        {
            foreach (var item in InternalDownloader(tuple.Item1)) yield return item;
            tuple.Item3?.Invoke(result, tuple.Item2);
        }

        onAllDownloaded?.Invoke();
        Destroy(this.gameObject);
    }

    private IEnumerator DownloadImage<T>(string url, T data, Action<Sprite, T> callback)
    {
        foreach (var item in InternalDownloader(url)) yield return item;
        callback?.Invoke(result, data);
        Destroy(this.gameObject);
    }

    private IEnumerator DownloadImage(string url, Action<Sprite> callback)
    {
        foreach (var item in InternalDownloader(url)) yield return item;
        callback?.Invoke(result);
        Destroy(this.gameObject);
    }
}
