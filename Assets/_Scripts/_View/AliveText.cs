using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    private TMP_Text text;
    public TMP_Text Text => text;

    private string startText;

    private const float CharDelay = 0.013f;

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

        while (length < value.Length)
        {
            yield return new WaitForSeconds(CharDelay);

            length++;

            // skip rich text tags <...>
            if (value.Substring(length - 1, 1) == "<")
            {
                length++;

                while (value.Substring(length - 1, 1) != ">")
                    length++;
            }

            string sub = value.Substring(0, length);
            text.text = sub;
        }
    }
}