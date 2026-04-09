using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    [SerializeField] private GameObject startButton;

    [SerializeField] private QuestionCell victoryCell;
    [SerializeField] private QuestionCell defeatCell;
    [SerializeField] private QuestionCell nextCell;
    [SerializeField] private QuestionCell questionCellPref;

    [SerializeField] private SettingsPanel settingsPref;    
    [SerializeField] private AliveText mainText;
    [SerializeField] private PictureNode pictureNode;
    [SerializeField] private TMP_Text startQuestText;
    [SerializeField] private QuestFoldersList questFoldersList;
    [SerializeField] private QuestCell questCellPref;

    private Player player;
    public Player Player => player;

    public static GamePanel Instance { get; private set; }

    private Passage singlePassage;

    private TextParser textParser;
    public TextParser TextParser => textParser;

    public PictureNode PictureNode => pictureNode;

    private LocationDescriptionResolver locationDescriptionResolver;
    private PassageResolver passageResolver;
    private ParameterService parameterService;

    private Quest selectedQuest = null;

    #region Inits

    private void Awake()
    {
        Instance = this;

        textParser = new TextParser(this);
        locationDescriptionResolver = new LocationDescriptionResolver(textParser);
        passageResolver = new PassageResolver(this, textParser);

        parameterService = new ParameterService(this, textParser, paramsContent, parameterTextPref,
                                                victoryCell, defeatCell, nextCell, questionsContent);
    }

    private void Start()
    {       
        mainPictureRect.sizeDelta = new Vector2(mainPictureRect.rect.height, mainPictureRect.sizeDelta.y);
        paramsRect.sizeDelta = new Vector2(mainPictureRect.rect.height, paramsRect.sizeDelta.y);       
        mainTextRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, mainTextRect.sizeDelta.y);
        questionsRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, questionsRect.sizeDelta.y);

        InitLocalisations();

        Player loadedPlayer = SaveLoadManager.Instance.LoadPlayer();         

        if (loadedPlayer == null)
            ShowAllQuestsOnStart();        
        else
            player = loadedPlayer;       

        if (player == null)
            return;

        startButton.SetActive(false);
        ShowCurrentLocation();          
    }   

    private void InitLocalisations()
    {
        startQuestText.text = Localization.Get(LocKeys.StartQuest);
        nextCell.SetText(Localization.Get(LocKeys.Next));
        victoryCell.SetText(Localization.Get(LocKeys.YouWin));
        defeatCell.SetText(Localization.Get(LocKeys.YouLose));
    }

    #endregion

    #region Publics   

    public void ActionStart()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

        if (selectedQuest == null)
            return;

        CreatePlayer(selectedQuest);
            
        startButton.SetActive(false);

        ShowCurrentLocation();
    }   

    public void ActionNext()
    {
        if (singlePassage == null)
            return;

        AudioManager.Instance.PlaySfx(SoundType.Click);

        player.locationID = singlePassage.to;
        player.passageID = singlePassage.id;

        ShowPassage(singlePassage);
    }

    public void ActionSettings()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

        Instantiate(settingsPref, canvas);
    }

    public void AbandonQuest()
    {
        ClearQuestions();
        parameterService.ClearParams();
        pictureNode.ClearPicturesColor();
       
        startButton.SetActive(true);
        nextCell.gameObject.SetActive(false);
        victoryCell.gameObject.SetActive(false);
        defeatCell.gameObject.SetActive(false);

        player = null;

        ShowAllQuestsOnStart();       

        SaveLoadManager.Instance.ClearPlayerSaveData();
    }

    public void ShowPassage(Passage passage)
    {
        if (player == null || player.gameOver)
            return;

        singlePassage = null;
        nextCell.gameObject.SetActive(true);

        parameterService.ApplyInfluences(passage, ShowMainText);

        if (!passage.ignoreDemonstration)
            parameterService.Demonstrate(passage);       

        passage.visitCounter++;

        Location location = player.quest.FindLocationWith(player.locationID);

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

    public void DiselectAllQuestCells()
    {
        foreach(Transform cell in questionsContent)        
            if(cell.TryGetComponent(out QuestCell questCell))            
                questCell.Diselect();              
    }

    #endregion

    #region Privates

    private void CreatePlayer(Quest quest)
    {
        Quest questClone = (Quest)quest.Clone();

        player = new Player
        {
            locationID = quest.FindStartLocation().id,
            quest = questClone           
        };      

        foreach (Location location in questClone.locations)
            location.visitCounter = 0;

        foreach (Passage passage in questClone.passages)
        {
            passage.visitCounter = 0;
            passage.FindControversials(questClone);
        }

        foreach (Parameter parameter in questClone.parameters)
            parameter.value = parameter.startValue;
    }

    private void ShowAllQuestsOnStart()
    {       
        ClearQuestions();

        Quest firstQuest = null;

        int i = 0;
        foreach (var folder in questFoldersList.questFolders)
        {
            Quest quest = SaveLoadManager.Instance.LoadQuestFromFrolder(folder);

            QuestCell cell = Instantiate(questCellPref, questionsContent);
            cell.StartWith(this, quest, i == 0);

            if (i == 0)
                firstQuest = quest;

            i++;
        }       

        pictureNode.InitImages(firstQuest.startImage, firstQuest.questName);

        SelectQuest(firstQuest);       
    }

    public void SelectQuest(Quest quest)
    {
        if (quest == null)
            return;

        selectedQuest = quest;

        mainText.SetText($"<b>{quest.displayName}</b>\n\n{quest.descrition}");
        pictureNode.SetNewPicture(quest.startImage, quest.questName);
        AudioManager.Instance.PlayMusic(quest.startMusic, quest.questName, stoppable: true);
    }

    private void ShowCurrentLocation()
    {
        nextCell.gameObject.SetActive(false);

        Location location = player.quest.FindLocationWith(player.locationID);

        ShowLocationContent(location);

        if (location.locationType == LocationType.Victory)
        {
            victoryCell.gameObject.SetActive(true);
            Final();
        }
        else if (location.locationType == LocationType.Fail)
        {
            defeatCell.gameObject.SetActive(true);
            Final();
        }

        List<PassageInfo> visiblePassages = ShowLocationPassages(location);       

        if (visiblePassages != null && visiblePassages.Count == 0)        
            Debug.LogWarning("Error: no available transitions!");        

        location.visitCounter++;
    }

    private void Final()
    {
        player.gameOver = true;
        ClearQuestions();
    }

    private void ShowLocationContent(Location location)
    {
        parameterService.ApplyInfluences(location, ShowMainText);
        parameterService.Demonstrate(location);       

        string description = locationDescriptionResolver.Resolve(location);
        ShowMainText(textParser.Parse(description));

        ClearQuestions();
    }

    private List<PassageInfo> ShowLocationPassages(Location location)
    {
        if (player == null || player.gameOver)
            return null;      

        List<PassageInfo> visiblePassages = passageResolver.ResolveVisiblePassages(location);

        singlePassage = null;
        nextCell.gameObject.SetActive(false);

        const float interval = 120f;

        for (int index = 0; index < visiblePassages.Count; index++)
        {
            PassageInfo info = visiblePassages[index];

            QuestionCell cell = Instantiate(questionCellPref, questionsContent);
            cell.StartWith(this, info.pass, index * 0.15f);        

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
            pictureNode.SetNewPicture(imageName, player.quest.questName);        

        AudioManager.Instance.PlayMusic(musicName, player.quest.questName, stoppable: false);
        AudioManager.Instance.PlaySfx(soundName, player.quest.questName);

        mainText.SetText(text);
    }       

    private void ClearQuestions()
    {      
        foreach (Transform tr in questionsContent)
            Destroy(tr.gameObject);       
    }

    #endregion
}