using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCell : MonoBehaviour, IKeyboardSelectable
{
    [SerializeField] private AliveText questionText;
    [SerializeField] private GameObject selectedImage;
    [SerializeField] private GameObject circle;
    [SerializeField] private Button button;

    [SerializeField] private GamePanel gamePanel;

    private Passage passage;
    private string text;
    private float delay;
    private bool ignoreFirstHover;

    private bool isNextButton;

    private Vector2 hoverSize = new Vector2(1.01f, 1.01f);
    private Vector2 normalSize = new Vector2(1f, 1f);

    public bool IsKeyboardSelectable => gameObject.activeInHierarchy && button != null && button.enabled;

    public void StartWith(GamePanel gamePanel, Passage passage, float delay = 0f)
    {
        this.gamePanel = gamePanel;
        this.passage = passage;
        this.delay = delay;

        text = gamePanel.TextParser.Parse(passage.question);

        selectedImage.SetActive(false);
    }

    public void SetText(string textString) => text = textString;

    private void OnEnable()
    {
        ignoreFirstHover = true;

        circle.SetActive(false);
        StartCoroutine(ShowWithDelay());

        StartCoroutine(TurnOffIgnor());
        IEnumerator TurnOffIgnor()
        {
            yield return null;
            ignoreFirstHover = false;
        }
    }

    private IEnumerator ShowWithDelay()
    {
        yield return new WaitForSeconds(delay);

        circle.SetActive(true);
        questionText.SetText(text);
    }

    public void ActionSelect()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

        if (isNextButton)
        {
            gamePanel.ActionNext();
            return;
        }

        var player = gamePanel.Player;

        if (passage != null)
        {
            player.locationID = passage.to;
            player.passageID = passage.id;
            gamePanel.ShowPassage(passage);
        }
        else
        {
            gamePanel.AbandonQuest();
        }
    }

    public void DisableButton()
    {
        button.enabled = false;
        questionText.Text.color = Color.gray;
    }

    public void SetKeyboardSelected(bool selected)
    {
        selectedImage.SetActive(selected);
        transform.localScale = selected ? hoverSize : normalSize;
    }

    public void SubmitKeyboard()
    {
        button.onClick.Invoke();
    }

    public void OnPointerEnter()
    {
        if (button != null && button.enabled)
        {
            selectedImage.SetActive(true);
            transform.localScale = hoverSize;

            if (ignoreFirstHover)
                return;

            AudioManager.Instance.PlaySfx(SoundType.Hover);
        }
    }

    public void OnPointerExit()
    {
        if (button != null && button.enabled)
            selectedImage.SetActive(false);

        transform.localScale = normalSize;
    }

    public void StartAsNext(GamePanel gamePanel)
    {
        this.gamePanel = gamePanel;
        passage = null;
        isNextButton = true;
        selectedImage.SetActive(false);
    }
}