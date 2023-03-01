using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCell : MonoBehaviour
{
    public Text questionText;
    public RawImage selectedImage;
    public Button button;

    GamePanel gamePanel;
    Passage passage; 

    public void StartWith(GamePanel gamePanel, Passage passage)
    {
        this.gamePanel = gamePanel;
        this.passage = passage;

        string str = gamePanel.ParseText(passage.question);        

        questionText.text = "- " + str;

        selectedImage.enabled = false;

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)questionText.transform);        

        RectTransform rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, questionText.GetComponent<RectTransform>().sizeDelta.y + 8);       

        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = questionText.GetGenerationSettings(questionText.rectTransform.rect.size);
        float width = textGen.GetPreferredWidth(questionText.text, generationSettings);

        //print("myRect: " + myRect.sizeDelta.y);
        rect.sizeDelta = new Vector2(width + 20, rect.sizeDelta.y);
    }

    public void ActionSelect()
    {                 
        gamePanel.player.locationID = passage.to;        
        gamePanel.player.passageID = passage.id;
        gamePanel.ShowPassage(passage);
    }

    public void DisableButton()
    {
        button.enabled = false;
        questionText.color = Color.gray;
    }

    public void OnPointerEnter ()
    {       
        if(button && button.enabled)
            selectedImage.enabled = true;
    }

    public void OnPointerExit()
    {
        if (button && button.enabled)
            selectedImage.enabled = false;
    }
}
