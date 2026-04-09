using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestCell : MonoBehaviour
{
    [SerializeField] private AliveText nameText;
    [SerializeField] private GameObject selectedImage;
    [SerializeField] private GameObject circle;
    [SerializeField] private Button button;

    GamePanel gamePanel;
    Quest quest;
    string nameString;

    public void StartWith(GamePanel gamePanel, Quest quest, bool selected)
    {
        this.gamePanel = gamePanel;
        this.quest = quest;

        nameString = quest.displayName ?? quest.questName;

        selectedImage.SetActive(selected);
    }

    private void OnEnable()
    {
        circle.SetActive(false);
        StartCoroutine(ShowWithDelay());
    }

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        circle.SetActive(true);
        nameText.SetText(nameString);
    }

    public void ActionSelect()
    {
        gamePanel.DiselectAllQuestCells();

        AudioManager.Instance.PlaySfx(SoundType.Click);
        gamePanel.SelectQuest(quest);
        selectedImage.SetActive(true);
    }

    public void Diselect() => selectedImage.SetActive(false);   
}
