using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCell : MonoBehaviour
{
    public Text questionText;
    public GameObject selectedImage;
    public Button button;

    GamePanel gamePanel;
    Passage passage; 

    public void StartWith(GamePanel gamePanel, Passage passage)
    {
        this.gamePanel = gamePanel;
        this.passage = passage;
        
        string str = gamePanel.ParseText(passage.question);        

        questionText.text = "- " + str;
        
        selectedImage.SetActive(false);             
        
        RectTransform rect = GetComponent<RectTransform>();      
        
        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = questionText.GetGenerationSettings(questionText.rectTransform.rect.size);
        float width = textGen.GetPreferredWidth(questionText.text, generationSettings);       
        rect.sizeDelta = new Vector2(Mathf.Min(width / gamePanel.autoCanvasScaler.scaleFactor + 50, Screen.width / gamePanel.autoCanvasScaler.scaleFactor - 120), rect.sizeDelta.y);
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
            selectedImage.SetActive(true); 
    }

    public void OnPointerExit()
    {
        if (button && button.enabled)
            selectedImage.SetActive(false);
    }
}
