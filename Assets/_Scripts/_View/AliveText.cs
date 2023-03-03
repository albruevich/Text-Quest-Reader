using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    [HideInInspector]
    public TMP_Text aliveText;

    string startText;

    private void Awake()
    {
        aliveText = GetComponent<TMP_Text>();
        startText = aliveText.text;
    } 

    private void OnEnable()
    {       
        aliveText.text = startText;
    }

    public void SetText(string str)
    {       
        StartCoroutine(AnimateText(str));       
    }

    IEnumerator AnimateText(string str)
    {
        if (string.IsNullOrEmpty(str))
            yield break;

        int length = 0;        

        while(length < str.Length)
        {
            yield return new WaitForSeconds(0.015f); 

            length++;

            if(str.Substring(length-1, 1) == "<")
            {
                length++;
                while(str.Substring(length - 1, 1) != ">")                
                    length++;                               
            }

            string sub = str.Substring(0, length);
            aliveText.text = sub;
        }        
    }
}
