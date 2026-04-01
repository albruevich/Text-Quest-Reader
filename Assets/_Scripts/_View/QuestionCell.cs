using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCell : MonoBehaviour
{
    [SerializeField] private AliveText questionText;
    [SerializeField] private GameObject selectedImage;
    [SerializeField] private GameObject circle;
    [SerializeField] private Button button;

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
            gamePanel.Player.locationID = passage.to;
            gamePanel.Player.passageID = passage.id;
            gamePanel.ShowPassage(passage);
        }
        else
        {
            gamePanel.RestartQuest();           
        }
    }

    public void DisableButton()
    {
        button.enabled = false;
        questionText.Text.color = Color.gray;
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
