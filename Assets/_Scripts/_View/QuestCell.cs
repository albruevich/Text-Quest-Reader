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
    QuestShort questShort;
    string nameString;

    public void StartWith(GamePanel gamePanel, QuestShort questShort, bool selected)
    {
        this.gamePanel = gamePanel;
        this.questShort = questShort;

        nameString = questShort.DisplayName ?? questShort.QuestName;

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
        gamePanel.SelectQuest(questShort);
        selectedImage.SetActive(true);
    }

    public void Diselect() => selectedImage.SetActive(false);
}
