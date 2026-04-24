using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private const string BaseUri = "http://localhost:5205/";
    private const string GetAllQuestsUri = BaseUri + "quests";
    private const string GetQuestUri = BaseUri + "quest";

    public static ApiManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    #region GET

    public void GetAllQuests(Action<string> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetRequest(GetAllQuestsUri, onSuccess, onError));
    }

    public void GetQuest(int id, Action<string> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetRequest($"{GetQuestUri}/{id}", onSuccess, onError));
    }

    public void DownloadQuestPackage(int id, Action<byte[]> onSuccess, Action<string> onError = null)
    {
        StartCoroutine(GetBytesRequest($"{GetQuestUri}/{id}/package", onSuccess, onError));
    }

    private IEnumerator GetRequest(string requestUri, Action<string> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMessage = $"HTTP error. Code: {webRequest.responseCode}, Error: {webRequest.error}, Uri: {requestUri}";
                onError?.Invoke(errorMessage);
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    private IEnumerator GetBytesRequest(string requestUri, Action<byte[]> onSuccess, Action<string> onError)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                string errorMessage = $"HTTP error. Code: {webRequest.responseCode}, Error: {webRequest.error}, Uri: {requestUri}";
                Debug.LogError(errorMessage);
                onError?.Invoke(errorMessage);
            }
            else
            {
                onSuccess?.Invoke(webRequest.downloadHandler.data);
            }
        }
    }

    #endregion
}