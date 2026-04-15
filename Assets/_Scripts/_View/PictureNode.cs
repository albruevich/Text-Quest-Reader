using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PictureNode : MonoBehaviour
{
    [SerializeField] private Image outerPicture;
    [SerializeField] private Image innerPicture;

    private Animator animator;
    private string lastPictureName;
    private Coroutine loadPictureCoroutine;

    private readonly Dictionary<string, Sprite> spriteCache = new();

    private static readonly string[] imageExtensions =
    {
        ".png",
        ".jpg",
        ".jpeg"      
    };

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ClearPicturesColor();
    }

    public void InitImages(string pictureName, string questName)
    {
        if (loadPictureCoroutine != null)
            StopCoroutine(loadPictureCoroutine);

        loadPictureCoroutine = StartCoroutine(InitImagesRoutine(pictureName, questName));
    }

    private IEnumerator InitImagesRoutine(string pictureName, string questName)
    {
        Sprite sprite = null;

        if (!string.IsNullOrEmpty(pictureName))        
            yield return StartCoroutine(LoadSprite(pictureName, questName, loadedSprite => sprite = loadedSprite));        

        innerPicture.sprite = sprite;
        outerPicture.sprite = sprite;
        lastPictureName = pictureName;
        loadPictureCoroutine = null;
    }

    public void ClearPicturesColor()
    {
        outerPicture.color = Color.white;
        innerPicture.color = Color.white;
    }

    public void SetNewPicture(string pictureName, string questName, bool mayBeSame)
    {
        if (lastPictureName == pictureName && !mayBeSame)
            return;

        if (loadPictureCoroutine != null)
            StopCoroutine(loadPictureCoroutine);

        loadPictureCoroutine = StartCoroutine(SetNewPictureRoutine(pictureName, questName));
    }

    private IEnumerator SetNewPictureRoutine(string pictureName, string questName)
    {
        Sprite sprite = null;

        if (!string.IsNullOrEmpty(pictureName))        
            yield return StartCoroutine(LoadSprite(pictureName, questName, loadedSprite => sprite = loadedSprite));        

        innerPicture.sprite = sprite;

        if (animator != null)
            animator.Play("FadePictures");

        lastPictureName = pictureName;
        loadPictureCoroutine = null;
    }

    public void Callback()
    {
        outerPicture.sprite = innerPicture.sprite;
    }

    private IEnumerator LoadSprite(string pictureNameWithoutExtensionOrWithIt, string questName, Action<Sprite> onLoaded)
    {
        string resolvedPath = FindImagePath(pictureNameWithoutExtensionOrWithIt, questName);

        if (string.IsNullOrEmpty(resolvedPath))
        {
            Debug.LogWarning($"Image not found. Quest: {questName}, Name: {pictureNameWithoutExtensionOrWithIt}");
            onLoaded?.Invoke(null);
            yield break;
        }

        string cacheKey = resolvedPath;

        if (spriteCache.TryGetValue(cacheKey, out Sprite cachedSprite) && cachedSprite != null)
        {
            onLoaded?.Invoke(cachedSprite);
            yield break;
        }

        string url = ToFileUrl(resolvedPath);

        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning($"Failed to load image: {resolvedPath}\n{request.error}");
            onLoaded?.Invoke(null);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        if (texture == null)
        {
            Debug.LogWarning($"Failed to decode image: {resolvedPath}");
            onLoaded?.Invoke(null);
            yield break;
        }

        texture.name = Path.GetFileNameWithoutExtension(resolvedPath);

        Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        sprite.name = texture.name;
        spriteCache[cacheKey] = sprite;

        onLoaded?.Invoke(sprite);
    }

    private string FindImagePath(string pictureNameWithoutExtensionOrWithIt, string questName)
    {
        string baseFolder = Path.Combine(Application.streamingAssetsPath, "Quests", questName, "Images");

        if (!Directory.Exists(baseFolder))
            return null;

        if (Path.HasExtension(pictureNameWithoutExtensionOrWithIt))
        {
            string fullPath = Path.Combine(baseFolder, pictureNameWithoutExtensionOrWithIt);
            return File.Exists(fullPath) ? fullPath : null;
        }

        foreach (string ext in imageExtensions)
        {
            string fullPath = Path.Combine(baseFolder, pictureNameWithoutExtensionOrWithIt + ext);

            if (File.Exists(fullPath))
                return fullPath;
        }

        return null;
    }

    private static string ToFileUrl(string path)
    {
        string normalizedPath = path.Replace("\\", "/");

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        return "file:///" + normalizedPath;
#else
        return "file://" + normalizedPath;
#endif
    }

    public void ClearLoadedQuestPictures(string questName)
    {
        if (string.IsNullOrEmpty(questName))
            return;

        List<string> keysToRemove = new();

        foreach (var pair in spriteCache)
        {
            string normalized = pair.Key.Replace("\\", "/");
            string questPart = "/Quests/" + questName + "/";

            if (normalized.Contains(questPart, StringComparison.OrdinalIgnoreCase))
            {
                if (pair.Value != null)
                {
                    if (pair.Value.texture != null)
                        Destroy(pair.Value.texture);

                    Destroy(pair.Value);
                }

                keysToRemove.Add(pair.Key);
            }
        }

        foreach (string key in keysToRemove)
            spriteCache.Remove(key);
    }

    public void ClearAllLoadedPictures()
    {
        foreach (var pair in spriteCache)
        {
            if (pair.Value != null)
            {
                if (pair.Value.texture != null)
                    Destroy(pair.Value.texture);

                Destroy(pair.Value);
            }
        }

        spriteCache.Clear();
    }
}