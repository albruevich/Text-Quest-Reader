using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    private TMP_Text text;
    public TMP_Text Text => text;

    private string startText;

    private const float minCharsPerSecond = 80f;
    private const float maxCharsPerSecond = 700f;
    private const int shortTextLength = 40;
    private const int longTextLength = 500;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startText = text.text;
    }

    private void OnEnable()
    {
        text.text = startText;
    }

    public void SetText(string value)
    {
        StopAllCoroutines();
        StartCoroutine(ShowText(value));
    }

    private IEnumerator ShowText(string value)
    {
        if (string.IsNullOrEmpty(value))                    
            yield break;        

        text.text = value;
        text.maxVisibleCharacters = 0;

        float charsPerSecond = GetCharsPerSecond(value.Length);

        float timer = 0f;
        int charIndex = 0;

        while (charIndex < value.Length)
        {
            timer += Time.deltaTime;

            int charsToShow = Mathf.FloorToInt(timer * charsPerSecond);

            if (charsToShow > 0)
            {
                timer -= charsToShow / charsPerSecond;

                charIndex += charsToShow;
                charIndex = Mathf.Min(charIndex, value.Length);

                text.maxVisibleCharacters = charIndex;
            }

            yield return null;
        }
    }

    private float GetCharsPerSecond(int textLength)
    {
        float t = Mathf.InverseLerp(shortTextLength, longTextLength, textLength);
        return Mathf.Lerp(minCharsPerSecond, maxCharsPerSecond, t);
    }
}