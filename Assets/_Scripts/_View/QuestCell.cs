using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestCell : MonoBehaviour
{
    [SerializeField] private AliveText nameText;
    [SerializeField] private AliveText infoText;
    [SerializeField] private GameObject selectedImage;
    [SerializeField] private GameObject circle;
    [SerializeField] private Button button;

    GamePanel gamePanel;
    QuestShort questShort;

    string nameString;
    string infoString;

    public void StartWith(GamePanel gamePanel, QuestShort questShort, bool selected)
    {
        this.gamePanel = gamePanel;
        this.questShort = questShort;

        nameString = string.IsNullOrEmpty(questShort.DisplayName) ? questShort.QuestName : questShort.DisplayName;

        infoString = "";

        if (!string.IsNullOrEmpty(questShort.Author))
            infoString = questShort.Author;

        if (!string.IsNullOrEmpty(questShort.Lang))
        {
            if (!string.IsNullOrEmpty(infoString))
                infoString += "   ";

            infoString += $"  [{questShort.Lang.ToUpper()}]";
        }

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
        infoText.SetText(infoString);
    }

    public void ActionSelect()
    {
        gamePanel.DiselectAllQuestCells();

        AudioManager.Instance.PlaySfx(SoundType.Click);
        gamePanel.SelectQuest(questShort);

        selectedImage.SetActive(true);
    }

    public void Diselect() => selectedImage.SetActive(false);
}