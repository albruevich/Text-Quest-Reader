using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : MonoBehaviour
{
    [SerializeField] private RectTransform questionsContent;
    [SerializeField] private RectTransform paramsContent;
    [SerializeField] private RectTransform questionsRect;
    [SerializeField] private RectTransform paramsRect;
    [SerializeField] private RectTransform canvas;

    [SerializeField] private SettingsPanel settingsPref;
    [SerializeField] private QuestionCell questionCellPref;

    [SerializeField] private GameObject parameterTextPref;
    [SerializeField] private GameObject victoryCell;
    [SerializeField] private GameObject defeatCell;
    [SerializeField] private GameObject textBg;
    [SerializeField] private GameObject questionsNode;
    [SerializeField] private GameObject arrowsNode;
    [SerializeField] private GameObject startNode;
    [SerializeField] private GameObject furtherNode;

    [SerializeField] private Button leftArrow;
    [SerializeField] private Button rightArrow;
    [SerializeField] private Button startButton;

    [SerializeField] private AliveText mainText;
    [SerializeField] private RectTransform mainPictureRect;

    [SerializeField] private AutoCanvasScaler autoCanvasScaler;
    [SerializeField] private PictureNode pictureNode;
   
    public Player Player { get; set; }

    string[] mainArray;
    int mainIndex;

    public static GamePanel Instance;

    private Passage singlePassage;

    private TextParser textParser;
    public TextParser TextParser => textParser;

    private LocationDescriptionResolver locationDescriptionResolver;
    private PassageResolver passageResolver;
    private ParameterService parameterService;

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

        float questionH = (Screen.safeArea.height - paramsRect.sizeDelta.y * autoCanvasScaler.scaleFactor - mainPictureRect.sizeDelta.y * autoCanvasScaler.scaleFactor) / autoCanvasScaler.scaleFactor;
        questionsRect.sizeDelta = new Vector2(questionsRect.sizeDelta.x, questionH);       

        if(SaveLoadManager.Instance.LoadedPlayer == null)
        {
            RestartQuest();
        }
        else
        {          
            Player = SaveLoadManager.Instance.LoadedPlayer;
            ActionStart();
        }     
    }    

    public void RestartQuest()
    {
        startButton.interactable = true;

        pictureNode.InitializePictures();
        startNode.SetActive(true);

        furtherNode.SetActive(false);
        questionsNode.SetActive(false);
        textBg.SetActive(false);
        arrowsNode.SetActive(false);
        victoryCell.SetActive(false);
        defeatCell.SetActive(false);
        paramsRect.gameObject.SetActive(false);

        Player = new Player
        {
            locationID = Quest.Instance.FindStartLocation().id,
            quest = (Quest)Quest.Instance.Clone()
        };

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

    private void ShowCurrentLocation()
    {
        furtherNode.SetActive(false);

        Location location = Player.quest.FindLocationWith(Player.locationID);

        parameterService.ApplyInfluences(location, GutMainText);
        parameterService.Demonstrate(location);

        if (Player.gameOver)
            return;

        string description = locationDescriptionResolver.Resolve(location);
        GutMainText(textParser.Parse(description));

        foreach (Transform tr in questionsContent)
            Destroy(tr.gameObject);      

        List<PassageInfo> visiblePassages = passageResolver.ResolveVisiblePassages(location);

        singlePassage = null;

        if (visiblePassages.Count > 1)
        {
            const float interval = 120;
            int m = 0;

            foreach (PassageInfo info in visiblePassages)
            {
                QuestionCell cell = Instantiate(questionCellPref, questionsContent);
                cell.StartWith(this, info.pass, m * 0.15f);

                RectTransform cellRect = cell.GetComponent<RectTransform>();

                float y = (visiblePassages.Count - 1) * interval / 2 - interval * m;
                cellRect.anchoredPosition = new Vector2(60, y);

                if (!info.isAllConditions && info.pass.alwaysShow)
                    cell.DisableButton();
                
                m++;
            }

            RectTransform viewPort = (RectTransform)questionsContent.parent;
            questionsContent.sizeDelta = new Vector2(
                questionsContent.sizeDelta.x,
                Mathf.Max(viewPort.rect.height, m * interval));
        }
        else if (visiblePassages.Count == 1)
        {
            singlePassage = visiblePassages[0].pass;
        }

        if (location.locationType == LocationType.Victory)
            FinalWithText("Quest completed!");
        else if (location.locationType == LocationType.Fail)
            FinalWithText("Quest failed!");
        else if (visiblePassages.Count == 0)
            Director.Instance.WarningWithText("Error: no available transitions!");

        location.visitCounter++;
    }

    private void GutMainText(string text)
    {
        List<string> list = textParser.GetBetween(text, "<im", "im>");
        string imageString = "";
        foreach(string str in list)
        {            
            text = text.Replace(str, "");
            imageString = str.Replace("<im", "");
            imageString = imageString.Replace("im>", "");
            imageString = imageString.Replace(" ", "");            
        }

        text = text.Replace(System.Environment.NewLine, "");

        pictureNode.SetNewPicture(imageString);       

        mainIndex = 0;
        mainArray = text.Split('&');
        mainText.SetText(mainArray[mainIndex]);       

        if (mainArray.Length > 1)
        {
            furtherNode.SetActive(false);
            arrowsNode.SetActive(true);
            questionsNode.SetActive(false);            
            leftArrow.interactable = false;            
            StartCoroutine(TurnOnButton(rightArrow, 0.35f));            
        }
        else
        {
            furtherNode.SetActive(true);
            arrowsNode.SetActive(false);
            questionsNode.SetActive(true);           
        }
    }

    public void ActionNext()
    {      
        mainIndex++;

        if (mainIndex < mainArray.Length)
            mainText.SetText(textParser.CleanLeadingSpaces(mainArray[mainIndex]));

        if (mainIndex >= mainArray.Length - 1)
        {
            if(singlePassage != null)            
                furtherNode.SetActive(true);            
            else            
                questionsNode.SetActive(true);            
           
            rightArrow.interactable = false;
        }
        else
        {
            rightArrow.interactable = false;            
            StartCoroutine(TurnOnButton(rightArrow, 0.2f));
        }

        leftArrow.interactable = false;
        StartCoroutine(TurnOnButton(leftArrow, 0.2f));
    } 

    public void ActionPrevious()
    {              
        mainIndex--;
        if (mainIndex >= 0)
            mainText.SetText(textParser.CleanLeadingSpaces(mainArray[mainIndex]));

        questionsNode.SetActive(false);
        furtherNode.SetActive(false);

        if (mainIndex == 0)
            leftArrow.interactable = false;
        else
        {            
            leftArrow.interactable = false;
            StartCoroutine(TurnOnButton(leftArrow, 0.2f));            
        }

        rightArrow.interactable = false;
        StartCoroutine(TurnOnButton(rightArrow, 0.2f));
    }

    private IEnumerator TurnOnButton(Button button, float delay)
    {       
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    } 

    public void ShowPassage(Passage passage)
    {
        leftArrow.interactable = false;
        rightArrow.interactable = false;

        singlePassage = null;

        furtherNode.SetActive(false);

        parameterService.ApplyInfluences(passage, GutMainText);

        if (!passage.ignoreDemonstration)
            parameterService.Demonstrate(passage);

        if (Player.gameOver) return;

        passage.visitCounter++;

        Location location = Player.quest.FindLocationWith(Player.locationID);

        // if passage has no description, show location immediately
        if (string.IsNullOrEmpty(passage.description) || location.locationType == LocationType.Empty)
        {
            // handling empty location
            if (location.locationType == LocationType.Empty && !string.IsNullOrEmpty(passage.description))
                location.descriptions[0] = passage.description;

            ShowCurrentLocation();
        }
        else // show passage description and "Next" button
        {
            GutMainText(textParser.Parse(passage.description));

            foreach (Transform tr in questionsContent)
                Destroy(tr.gameObject);

            Passage next = new Passage
            {
                to = passage.to,
                question = "Next",
                ignoreDemonstration = true
            };

            singlePassage = next;
        }
    }
  
    private void FinalWithText(string text)
    {
        Director.Instance.WarningWithText(text);
        Player.gameOver = true;
        foreach (Transform tr in questionsContent)
            Destroy(tr.gameObject);
    }     

    public void ActionFinal(bool victory)
    {
        if (victory)
            Director.Instance.WarningWithText("Quest completed!");
        else
            Director.Instance.WarningWithText("Quest failed!");

        victoryCell.SetActive(false);
        defeatCell.SetActive(false);
    }

    public void ActionStart()
    {
        rightArrow.interactable = false;        

        startButton.interactable = false;
        paramsRect.gameObject.SetActive(true);
        startNode.SetActive(false);
        textBg.SetActive(true);
        arrowsNode.SetActive(true);
        ShowCurrentLocation();
    }

    public void ActionSettings()
    {       
        Instantiate(settingsPref, canvas);
    }

    public void ActionFurther()
    {
        if(singlePassage != null)
        {
            Player.locationID = singlePassage.to;
            Player.passageID = singlePassage.id;
            ShowPassage(singlePassage);
        }      
    }
}