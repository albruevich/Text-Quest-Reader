using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    private TMP_Text text;
    public TMP_Text Text => text;

    private string startText;

    private const float CharDelay = 0.004f;

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
        StartCoroutine(AnimateText(value));
    }

    private IEnumerator AnimateText(string value)
    {
        if (string.IsNullOrEmpty(value))
            yield break;

        int length = 0;
        float timer = 0f;

        while (length < value.Length)
        {
            timer += Time.deltaTime;

            if (timer >= CharDelay)
            {
                timer = 0f;

                length++;

                // skip rich text tags <...>
                if (value[length - 1] == '<')
                {
                    length++;

                    while (value[length - 1] != '>')
                        length++;
                }

                text.text = value.Substring(0, length);
            }

            yield return null;
        }
    }
}