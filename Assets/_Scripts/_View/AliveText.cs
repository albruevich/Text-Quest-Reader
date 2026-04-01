using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    TMP_Text text;
    public TMP_Text Text => text;

    string startText;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
        startText = text.text;
    } 

    private void OnEnable()
    {
        text.text = startText;
    }

    public void SetText(string str)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateText(str));       
    }

    IEnumerator AnimateText(string str)
    {
        if (string.IsNullOrEmpty(str))
            yield break;

        int length = 0;        

        while(length < str.Length)
        {
            yield return new WaitForSeconds(0.013f); 

            length++;

            if(str.Substring(length-1, 1) == "<")
            {
                length++;
                while(str.Substring(length - 1, 1) != ">")                
                    length++;                               
            }

            string sub = str.Substring(0, length);
            text.text = sub;
        }        
    }
}
