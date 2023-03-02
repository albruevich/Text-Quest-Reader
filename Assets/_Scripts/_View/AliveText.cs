using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class AliveText : MonoBehaviour
{
    private TMP_Text aliveText;

    private void Start()
    {
        aliveText = GetComponent<TMP_Text>();
    }

    public void SetText(string str)
    {
        StartCoroutine(AnimateText(str));       
    }

    IEnumerator AnimateText(string str)
    {
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
