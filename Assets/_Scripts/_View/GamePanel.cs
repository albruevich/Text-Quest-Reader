using System.Collections.Generic;
using UnityEngine;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private RectTransform mainPictureRect;
    [SerializeField] private RectTransform paramsRect;
    [SerializeField] private RectTransform paramsContent;
    [SerializeField] private RectTransform mainTextRect;
    [SerializeField] private RectTransform questionsRect;
    [SerializeField] private RectTransform questionsContent;     
    [SerializeField] private RectTransform canvas;  

    [SerializeField] private GameObject parameterTextPref;
    [SerializeField] private GameObject victoryCell;
    [SerializeField] private GameObject defeatCell;
    [SerializeField] private GameObject nextNode;
    [SerializeField] private GameObject startButton;

    [SerializeField] private SettingsPanel settingsPref;
    [SerializeField] private QuestionCell questionCellPref;
    [SerializeField] private AliveText mainText;
    [SerializeField] private PictureNode pictureNode;

    public Player Player { get; set; }
    public static GamePanel Instance { get; private set; }

    private Passage singlePassage;

    private TextParser textParser;
    public TextParser TextParser => textParser;

    public PictureNode PictureNode => pictureNode;

    private LocationDescriptionResolver locationDescriptionResolver;
    private PassageResolver passageResolver;
    private ParameterService parameterService;

    #region Inits

    private void Awake()
    {
        Instance = this;

        textParser = new TextParser(this);
        locationDescriptionResolver = new LocationDescriptionResolver(textParser);
        passageResolver = new PassageResolver(this, textParser);

        parameterService = new ParameterService(this, textParser, paramsContent, parameterTextPref,
                                                victoryCell, defeatCell, questionsContent);
    }

    private void Start()
    {
        mainPictureRect.sizeDelta = new Vector2(mainPictureRect.rect.height, mainPictureRect.sizeDelta.y);
        paramsRect.sizeDelta = new Vector2(mainPictureRect.rect.height, paramsRect.sizeDelta.y);       
        mainTextRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, mainTextRect.sizeDelta.y);
        questionsRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, questionsRect.sizeDelta.y);

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
        AudioManager.Instance.PlaySfx(SoundType.Click);

        Player.gameOver = false;
        startButton.SetActive(false);

        ShowCurrentLocation();
    }   

    public void ActionNext()
    {
        if (singlePassage == null)
            return;

        AudioManager.Instance.PlaySfx(SoundType.Click);

        Player.locationID = singlePassage.to;
        Player.passageID = singlePassage.id;

        ShowPassage(singlePassage);
    }

    public void ActionSettings()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

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

        ShowLocationContent(location);

        if (Player.gameOver)
            return;

        List<PassageInfo> visiblePassages = ShowLocationPassages(location);

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
        {
            Debug.LogWarning("Error: no available transitions!");
        }

        location.visitCounter++;
    }

    private void ShowLocationContent(Location location)
    {
        parameterService.ApplyInfluences(location, ShowMainText);
        parameterService.Demonstrate(location);

        if (Player.gameOver)
            return;

        string description = locationDescriptionResolver.Resolve(location);
        ShowMainText(textParser.Parse(description));

        ClearQuestions();
    }

    private List<PassageInfo> ShowLocationPassages(Location location)
    {
        List<PassageInfo> visiblePassages = passageResolver.ResolveVisiblePassages(location);

        singlePassage = null;
        nextNode.SetActive(false);

        const float interval = 120f;

        for (int index = 0; index < visiblePassages.Count; index++)
        {
            PassageInfo info = visiblePassages[index];

            QuestionCell cell = Instantiate(questionCellPref, questionsContent);
            cell.StartWith(this, info.pass, index * 0.15f);

            RectTransform cellRect = cell.GetComponent<RectTransform>();
            float y = (visiblePassages.Count - 1) * interval / 2f - interval * index;
            cellRect.anchoredPosition = new Vector2(60f, y);

            if (!info.isAllConditions && info.pass.alwaysShow)
                cell.DisableButton();
        }

        RectTransform viewPort = (RectTransform)questionsContent.parent;
        questionsContent.sizeDelta = new Vector2(questionsContent.sizeDelta.x, Mathf.Max(viewPort.rect.height, visiblePassages.Count * interval));

        return visiblePassages;
    }

    private void ShowMainText(string text)
    {
        string imageName = textParser.ExtractLastTagValue(ref text, "im");
        string musicName = textParser.ExtractLastTagValue(ref text, "mu");
        string soundName = textParser.ExtractLastTagValue(ref text, "so");

        if (!string.IsNullOrEmpty(imageName))
            pictureNode.SetNewPicture(imageName);        

        AudioManager.Instance.PlayMusic(musicName);
        AudioManager.Instance.PlaySfx(soundName);

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