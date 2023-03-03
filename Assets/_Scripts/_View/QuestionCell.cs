using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionCell : MonoBehaviour
{
    public AliveText questionText;
    public GameObject selectedImage, circle;
    public Button button;

    GamePanel gamePanel;
    Passage passage;

    string text;
    float delay;

    public void StartWith(GamePanel gamePanel, Passage passage, float delay = 0)
    {
        this.gamePanel = gamePanel;
        this.passage = passage;
        this.delay = delay;

        text = gamePanel.ParseText(passage.question);
        
        selectedImage.SetActive(false);     
    }

    private void OnEnable()
    {
        circle.SetActive(false);
        StartCoroutine(Delay(delay));
    }

    IEnumerator Delay(float delay)
    {
        yield return new WaitForSeconds(delay);
        circle.SetActive(true);
        questionText.SetText(text);
    }   

    public void ActionSelect()
    {
        if(passage != null)
        {
            gamePanel.player.locationID = passage.to;
            gamePanel.player.passageID = passage.id;
            gamePanel.ShowPassage(passage);
        }
        else
        {
            //todo - Сыграть заново
        }
    }

    public void DisableButton()
    {
        button.enabled = false;
        questionText.aliveText.color = Color.gray;
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
