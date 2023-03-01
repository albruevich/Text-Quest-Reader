using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(InputField))]
[RequireComponent(typeof(ScrollRect))]

public class AutoInputHeight : UIBehaviour
{
    public float VerticalOffset;
    public AutoCanvasScaler autoCanvasScaler;

    InputField inputField;
    InputField InputField { get { if (!inputField) inputField = GetComponent<InputField>(); return inputField; } }

    ScrollRect scrollRect;
    ScrollRect ScrollRect { get { if (!scrollRect) scrollRect = GetComponent<ScrollRect>(); return scrollRect; } }

    TextGenerationSettings settings;
    RectTransform tectRect;
    float rowHeight;
    float preferredHeight;    

    protected override void Start()
    {
        settings = InputField.textComponent.GetGenerationSettings(InputField.textComponent.rectTransform.rect.size);
        settings.generateOutOfBounds = false;

        tectRect = InputField.textComponent.GetComponent<RectTransform>();      
        rowHeight = InputField.textComponent.preferredHeight;

        SetTextHeight();
        InputField.onValueChanged.AddListener(OnValueChange);       
    }

    private void Update()
    {          
        if (Input.GetKey("up") || Input.GetKey("down"))
        {
            CorrectScrollPos();            
        }
        else if(Input.GetKey(KeyCode.Return))
        {
            int curRow, allRows;
            GetRows(out curRow, out allRows);

           if (curRow != 0 && curRow + 1 != allRows)            
                CorrectScrollPos();            
        }
    }   

    private void SetTextHeight()
    {
        preferredHeight = new TextGenerator().GetPreferredHeight(InputField.text, settings) + VerticalOffset;
        tectRect.sizeDelta = new Vector2(tectRect.sizeDelta.x, preferredHeight / autoCanvasScaler.scaleFactor);
    }

    private void CorrectScrollPos()
    {
        int curRow, allRows;
        GetRows(out curRow, out allRows);

        float y = 1 - curRow / (float)allRows;
        ScrollRect.verticalNormalizedPosition = y;
    }

    private void GetRows(out int curRow, out int allRows)
    {
        curRow = (int)(-GetLocalCaretY() / rowHeight);
        allRows = (int)(preferredHeight / rowHeight);

        if (curRow / (float)allRows > 0.5f)                  
            curRow++;                   
    }

    private void OnValueChange(string arg0)
    {
        SetTextHeight();

        int curRow, allRows;
        GetRows(out curRow, out allRows);

        if (curRow == 0 || curRow + 1 == allRows)        
            StartCoroutine(ScrollMax());             
    }

    IEnumerator ScrollMax()
    {
        yield return new WaitForEndOfFrame();
        ScrollRect.verticalNormalizedPosition = 0;
    }
 
    public float GetLocalCaretY()
    {
        TextGenerator gen = InputField.textComponent.cachedTextGenerator;      

        if(InputField.caretPosition < gen.characters.Count)
        {           
            UICharInfo charInfo = gen.characters[InputField.caretPosition];
            //float x = (charInfo.cursorPos.x + charInfo.charWidth) / InputField.textComponent.pixelsPerUnit;
            //float y = charInfo.cursorPos.y / InputField.textComponent.pixelsPerUnit;
            return charInfo.cursorPos.y / InputField.textComponent.pixelsPerUnit;
        }

        return 0;
    }
}
