using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCell : MonoBehaviour
{
    [SerializeField] private AliveText questionText;
    [SerializeField] private GameObject selectedImage;
    [SerializeField] private GameObject circle;
    [SerializeField] private Button button;

    [SerializeField] private GamePanel gamePanel; // can be assigned in inspector or via StartWith()

    private Passage passage;

    private string text;
    private float delay;

    public void StartWith(GamePanel gamePanel, Passage passage, float delay = 0f)
    {
        this.gamePanel = gamePanel;
        this.passage = passage;
        this.delay = delay;

        text = gamePanel.TextParser.Parse(passage.question);

        selectedImage.SetActive(false);
    }

    private void OnEnable()
    {
        circle.SetActive(false);
        StartCoroutine(ShowWithDelay());
    }

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(delay);
       
        circle.SetActive(true);
        questionText.SetText(text);
    }

    public void ActionSelect()
    {
        if (passage != null)
        {
            var player = gamePanel.Player;

            player.locationID = passage.to;
            player.passageID = passage.id;

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

    public void OnPointerEnter()
    {
        if (button != null && button.enabled)
            selectedImage.SetActive(true);
    }

    public void OnPointerExit()
    {
        if (button != null && button.enabled)
            selectedImage.SetActive(false);
    }
}