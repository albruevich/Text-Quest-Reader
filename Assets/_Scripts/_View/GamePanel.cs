using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private RectTransform questionsContent;
    [SerializeField] private RectTransform paramsContent;
    [SerializeField] private RectTransform questionsRect;
    [SerializeField] private RectTransform paramsRect;
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform mainPictureRect;

    [SerializeField] private GameObject parameterTextPref;
    [SerializeField] private GameObject victoryCell;
    [SerializeField] private GameObject defeatCell;
    [SerializeField] private GameObject nextNode;
    [SerializeField] private GameObject startButton;

    [SerializeField] private SettingsPanel settingsPref;
    [SerializeField] private QuestionCell questionCellPref;
    [SerializeField] private AliveText mainText;
    [SerializeField] private AutoCanvasScaler autoCanvasScaler;
    [SerializeField] private PictureNode pictureNode;

    public Player Player { get; set; }
    public static GamePanel Instance { get; private set; }

    private Passage singlePassage;

    private TextParser textParser;
    public TextParser TextParser => textParser;

    private LocationDescriptionResolver locationDescriptionResolver;
    private PassageResolver passageResolver;
    private ParameterService parameterService;

    #region Inits

    private void Awake()
    {
        Instance = this;

        textParser = new TextParser(this);
        locationDescriptionResolver = new LocationDescriptionResolver(this);
        passageResolver = new PassageResolver(this, textParser);

        parameterService = new ParameterService(this, textParser, paramsContent, parameterTextPref,
                                                victoryCell, defeatCell, questionsContent);
    }

    private void Start()
    {
        mainPictureRect.sizeDelta = new Vector2(mainPictureRect.sizeDelta.x, mainPictureRect.rect.width / autoCanvasScaler.scaleFactor);
        paramsRect.sizeDelta = new Vector2(paramsRect.sizeDelta.x, paramsRect.rect.width / 3f / autoCanvasScaler.scaleFactor);

        float questionHeight = (Screen.safeArea.height - paramsRect.sizeDelta.y * autoCanvasScaler.scaleFactor -
                                mainPictureRect.sizeDelta.y * autoCanvasScaler.scaleFactor) / autoCanvasScaler.scaleFactor;
        questionsRect.sizeDelta = new Vector2(questionsRect.sizeDelta.x, questionHeight);

        if (SaveLoadManager.Instance.LoadedPlayer == null)
            RestartQuest();
        else
            Player = SaveLoadManager.Instance.LoadedPlayer;

        if (Player.gameOver)
        {
            mainText.SetText($"Quest: '{Player.quest.questName}'");

            startButton.SetActive(true);
            nextNode.SetActive(false);
        }
        else
        {
            startButton.SetActive(false);
            ShowCurrentLocation();
        }
    }

    #endregion

    #region Publics

    public void ActionStart()
    {
        Player.gameOver = false;
        startButton.SetActive(false);

        ShowCurrentLocation();
    }   

    public void ActionNext()
    {
        if (singlePassage == null)
            return;

        Player.locationID = singlePassage.to;
        Player.passageID = singlePassage.id;

        ShowPassage(singlePassage);
    }

    public void ActionSettings()
    {
        Instantiate(settingsPref, canvas);
    }

    public void RestartQuest()
    {
        ClearQuestions();

        pictureNode.InitializePictures();

        nextNode.SetActive(false);
        startButton.SetActive(true);
        victoryCell.SetActive(false);
        defeatCell.SetActive(false);

        Player = new Player
        {
            locationID = Quest.Instance.FindStartLocation().id,
            quest = (Quest)Quest.Instance.Clone(),
            gameOver = true
        };

        mainText.SetText($"Quest: '{Player.quest.questName}'");

        foreach (Location location in Player.quest.locations)
            location.visitCounter = 0;

        foreach (Passage passage in Player.quest.passages)
        {
            passage.visitCounter = 0;
            passage.FindControversials();
        }

        foreach (Parameter parameter in Player.quest.parameters)
            parameter.value = parameter.startValue;
    }

    public void ShowPassage(Passage passage)
    {
        singlePassage = null;
        nextNode.SetActive(true);

        parameterService.ApplyInfluences(passage, ShowMainText);

        if (!passage.ignoreDemonstration)
            parameterService.Demonstrate(passage);

        if (Player.gameOver)
            return;

        passage.visitCounter++;

        Location location = Player.quest.FindLocationWith(Player.locationID);

        if (string.IsNullOrEmpty(passage.description) || location.locationType == LocationType.Empty)
        {
            if (location.locationType == LocationType.Empty && !string.IsNullOrEmpty(passage.description))
                location.descriptions[0] = passage.description;

            ShowCurrentLocation();
        }
        else
        {
            ShowMainText(textParser.Parse(passage.description));
            ClearQuestions();

            Passage next = new Passage
            {
                to = passage.to,
                question = "Next",
                ignoreDemonstration = true
            };

            singlePassage = next;
        }
    }

    #endregion

    #region Privates

    private void ShowCurrentLocation()
    {
        nextNode.SetActive(false);

        Location location = Player.quest.FindLocationWith(Player.locationID);

        parameterService.ApplyInfluences(location, ShowMainText);
        parameterService.Demonstrate(location);

        if (Player.gameOver)
            return;

        string description = locationDescriptionResolver.Resolve(location);
        ShowMainText(textParser.Parse(description));

        ClearQuestions();

        List<PassageInfo> visiblePassages = passageResolver.ResolveVisiblePassages(location);

        singlePassage = null;
        nextNode.SetActive(visiblePassages.Count == 1);

        if (visiblePassages.Count > 1)
        {
            const float interval = 120f;
            int index = 0;

            foreach (PassageInfo info in visiblePassages)
            {
                QuestionCell cell = Instantiate(questionCellPref, questionsContent);
                cell.StartWith(this, info.pass, index * 0.15f);

                RectTransform cellRect = cell.GetComponent<RectTransform>();
                float y = (visiblePassages.Count - 1) * interval / 2f - interval * index;
                cellRect.anchoredPosition = new Vector2(60f, y);

                if (!info.isAllConditions && info.pass.alwaysShow)
                    cell.DisableButton();

                index++;
            }

            RectTransform viewPort = (RectTransform)questionsContent.parent;
            questionsContent.sizeDelta = new Vector2(
                questionsContent.sizeDelta.x,
                Mathf.Max(viewPort.rect.height, index * interval));
        }
        else if (visiblePassages.Count == 1)
        {
            singlePassage = visiblePassages[0].pass;
        }

        if (location.locationType == LocationType.Victory)
        {
            victoryCell.SetActive(true);
            Final();
        }
        else if (location.locationType == LocationType.Fail)
        {
            defeatCell.SetActive(true);
            Final();
        }
        else if (visiblePassages.Count == 0)
            Debug.LogWarning("Error: no available transitions!");

        location.visitCounter++;
    }

    private void ShowMainText(string text)
    {
        List<string> imageTags = textParser.GetBetween(text, "<im", "im>");
        string imageName = "";

        foreach (string tag in imageTags)
        {
            text = text.Replace(tag, "");
            imageName = tag.Replace("<im", "");
            imageName = imageName.Replace("im>", "");
            imageName = imageName.Replace(" ", "");
        }

        text = text.Replace("\r", "");
        text = text.Replace("\n", "");

        pictureNode.SetNewPicture(imageName);
        mainText.SetText(text);
    }

    private void Final()
    {
        Player.gameOver = true;
        ClearQuestions();
    }

    private void ClearQuestions()
    {
        foreach (Transform tr in questionsContent)
            Destroy(tr.gameObject);
    }

    #endregion
}