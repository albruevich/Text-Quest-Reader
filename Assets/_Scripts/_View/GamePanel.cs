using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
    [SerializeField] private GameObject sourcesNode;

    [SerializeField] private QuestionCell victoryCell;
    [SerializeField] private QuestionCell defeatCell;
    [SerializeField] private QuestionCell nextCell;
    [SerializeField] private QuestionCell questionCellPref;

    [SerializeField] private SettingsPanel settingsPref;
    [SerializeField] private AliveText mainText;
    [SerializeField] private PictureNode pictureNode;
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

    private QuestShort selectedQuest;
    private bool selectedQuestIsRemote;

    public event Action HandleLocalizationsEvent;

    #region Inits

    private void Awake()
    {
        Instance = this;

        textParser = new TextParser(this);
        locationDescriptionResolver = new LocationDescriptionResolver(textParser);
        passageResolver = new PassageResolver(this, textParser);

        parameterService = new ParameterService(
            this,
            textParser,
            paramsContent,
            parameterTextPref,
            victoryCell,
            defeatCell,
            nextCell,
            questionsContent
        );
    }

    private void Start()
    {
        mainPictureRect.sizeDelta = new Vector2(mainPictureRect.rect.height, mainPictureRect.sizeDelta.y);
        paramsRect.sizeDelta = new Vector2(mainPictureRect.rect.height, paramsRect.sizeDelta.y);
        mainTextRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, mainTextRect.sizeDelta.y);
        questionsRect.sizeDelta = new Vector2(canvas.rect.width - mainPictureRect.sizeDelta.x, questionsRect.sizeDelta.y);

        HandleLocalizations();

        Player loadedPlayer = SaveLoadManager.Instance.LoadPlayer();

        if (loadedPlayer == null)
            UpdateLocalQuests();
        else
            player = loadedPlayer;

        if (player == null)
            return;

        sourcesNode.SetActive(false);

        ShowCurrentLocation();
    }

    public void HandleLocalizations()
    {
        if (!PlayerPrefs.HasKey(Localization.LANGUAGE_KEY))
        {
            string lang = Localization.GetLangCode(Application.systemLanguage);
            PlayerPrefs.SetString(Localization.LANGUAGE_KEY, lang);
        }

        Localization.SetCurrentLanguage(PlayerPrefs.GetString(Localization.LANGUAGE_KEY, "en"));

        nextCell.SetText(Localization.Get(LocKeys.Next));
        victoryCell.SetText(Localization.Get(LocKeys.YouWin));
        defeatCell.SetText(Localization.Get(LocKeys.YouLose));

        HandleLocalizationsEvent?.Invoke();
    }

    #endregion

    #region Publics

    public void StartQuest()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

        if (selectedQuest == null)
            return;

        if (selectedQuestIsRemote)
            StartRemoteQuest(selectedQuest.Id);
        else
            StartLocalQuest(selectedQuest.QuestName);
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

        SettingsPanel panel = Instantiate(settingsPref, canvas);
        panel.Init(this);
    }

    public void AbandonQuest()
    {
        ClearQuestions();
        parameterService.ClearParams();
        pictureNode.ClearPicturesColor();

        sourcesNode.SetActive(true);
        nextCell.gameObject.SetActive(false);
        victoryCell.gameObject.SetActive(false);
        defeatCell.gameObject.SetActive(false);

        player = null;
        singlePassage = null;

        UpdateLocalQuests();

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
                question = Localization.Get(LocKeys.Next),
                ignoreDemonstration = true
            };

            singlePassage = next;
        }
    }

    public void DiselectAllQuestCells()
    {
        foreach (Transform cell in questionsContent)
        {
            if (cell.TryGetComponent(out QuestCell questCell))
                questCell.Diselect();
        }
    }

    public void SelectQuest(QuestShort questShort)
    {
        if (questShort == null)
            return;

        selectedQuest = questShort;

        string title = string.IsNullOrEmpty(questShort.DisplayName)
            ? questShort.QuestName
            : questShort.DisplayName;

        mainText.SetText($"<b>{title}</b>\n\n{questShort.Description}");
        pictureNode.SetNewPicture(questShort.StartImage, questShort.QuestName, mayBeSame: true);
        AudioManager.Instance.PlayMusic(questShort.StartMusic, questShort.QuestName, stoppable: true);
    }

    public void ApplyLanguageChangeToQuestView()
    {
        HandleLocalizations();

        if (player == null)
        {
            string selectedQuestName = selectedQuest != null ? selectedQuest.QuestName : null;
            UpdateLocalQuests(selectedQuestName);
            return;
        }

        player.quest = CreateLocalizedQuestWithProgress(player.quest);
        ShowCurrentLocation();
    }

    public void UpdateLocalQuests(string questNameToSelect = null)
    {
        selectedQuestIsRemote = false;

        ClearQuestions();

        var builtInQuestFolders = QuestHelper.GetAllQuestFolders();
        var userQuestFolders = QuestHelper.GetUserQuestFolders();

        List<string> allQuestFolders = new List<string>();

        allQuestFolders.AddRange(userQuestFolders);

        foreach (string folder in builtInQuestFolders)
        {
            if (!allQuestFolders.Contains(folder))
                allQuestFolders.Add(folder);
        }

        List<QuestShort> quests = new List<QuestShort>();

        foreach (string folder in allQuestFolders)
        {
            QuestShort questShort = SaveLoadManager.Instance.LoadQuestShortFromFolder(folder);

            if (questShort == null)
                continue;

            quests.Add(questShort);
        }

        quests = quests
            .OrderBy(q => q.Order)
            .ThenBy(q => q.QuestName)
            .ToList();

        ShowQuestShortList(quests, questNameToSelect);
    }

    public void UpdateRemoteQuests(List<QuestShort> list)
    {
        selectedQuestIsRemote = true;

        ClearQuestions();

        if (list == null || list.Count == 0)
            return;

        list = list
            .OrderBy(x => x.Order)
            .ThenBy(x => x.QuestName)
            .ToList();

        string selectedQuestName = selectedQuest != null ? selectedQuest.QuestName : null;

        ShowQuestShortList(list, selectedQuestName);
    }

    #endregion

    #region Quest Preview

    private void ShowQuestShortList(List<QuestShort> quests, string questNameToSelect)
    {
        QuestShort firstQuest = null;
        QuestShort questToSelect = null;

        for (int i = 0; i < quests.Count; i++)
        {
            QuestShort quest = quests[i];

            bool isSelected;

            if (!string.IsNullOrEmpty(questNameToSelect))
                isSelected = quest.QuestName == questNameToSelect;
            else
                isSelected = i == 0;

            QuestCell cell = Instantiate(questCellPref, questionsContent);
            cell.StartWith(this, quest, isSelected);

            if (i == 0)
                firstQuest = quest;

            if (isSelected)
                questToSelect = quest;
        }

        if (questToSelect == null)
            questToSelect = firstQuest;

        if (questToSelect == null)
            return;

        pictureNode.InitImages(questToSelect.StartImage, questToSelect.QuestName);
        SelectQuest(questToSelect);
    }

    #endregion

    #region Start Quest

    private void StartLocalQuest(string questName)
    {
        Quest quest = SaveLoadManager.Instance.LoadQuestFromFolder(questName);

        if (quest == null)
        {
            Debug.LogWarning("Quest not found: " + questName);
            return;
        }

        CreatePlayer(quest);

        sourcesNode.SetActive(false);

        ShowCurrentLocation();
    }

    private void StartRemoteQuest(int questId)
    {
        ApiManager.Instance.DownloadQuestPackage(questId, (bytes) =>
        {
            string tempRoot = null;
            string tempZipPath = null;

            try
            {
                if (bytes == null || bytes.Length == 0)
                {
                    Debug.LogWarning("Downloaded package is empty.");
                    return;
                }

                tempRoot = Path.Combine(Application.temporaryCachePath, "RemoteQuestImport");
                Directory.CreateDirectory(tempRoot);

                tempZipPath = Path.Combine(tempRoot, $"quest_{questId}.zip");
                string extractFolder = Path.Combine(tempRoot, $"quest_{questId}");

                if (Directory.Exists(extractFolder))
                    Directory.Delete(extractFolder, true);

                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);

                File.WriteAllBytes(tempZipPath, bytes);
                System.IO.Compression.ZipFile.ExtractToDirectory(tempZipPath, extractFolder);

                string questJsonPath = Path.Combine(extractFolder, "quest.json");

                if (!File.Exists(questJsonPath))
                {
                    Debug.LogWarning("Downloaded package does not contain quest.json.");
                    return;
                }

                string json = File.ReadAllText(questJsonPath);
                Quest quest = JsonConvert.DeserializeObject<Quest>(json, SaveLoadManager.JsonSettings);

                if (quest == null)
                {
                    Debug.LogWarning("Quest data is empty.");
                    return;
                }

                CreatePlayer(quest);

                sourcesNode.SetActive(false);

                ShowCurrentLocation();
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Error loading remote quest: " + ex.Message);
            }
            finally
            {
                try
                {
                    if (!string.IsNullOrEmpty(tempZipPath) && File.Exists(tempZipPath))
                        File.Delete(tempZipPath);

                    string extractFolder = Path.Combine(Application.temporaryCachePath, "RemoteQuestImport", $"quest_{questId}");

                    if (Directory.Exists(extractFolder))
                        Directory.Delete(extractFolder, true);
                }
                catch (Exception cleanupEx)
                {
                    Debug.LogWarning("Remote import cleanup warning: " + cleanupEx.Message);
                }
            }
        },
        (error) =>
        {
            Debug.LogWarning("Error downloading quest package: " + error);
        });
    }

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

        singlePassage = null;
    }

    #endregion

    #region Privates

    private Quest CreateLocalizedQuestWithProgress(Quest oldQuest)
    {
        Quest loadedQuest = SaveLoadManager.Instance.LoadQuestFromFolder(oldQuest.questName);
        Quest newQuest = (Quest)loadedQuest.Clone();

        for (int i = 0; i < newQuest.parameters.Count && i < oldQuest.parameters.Count; i++)
        {
            newQuest.parameters[i].value = oldQuest.parameters[i].value;
            newQuest.parameters[i].isActive = oldQuest.parameters[i].isActive;
        }

        for (int i = 0; i < newQuest.locations.Count && i < oldQuest.locations.Count; i++)
            newQuest.locations[i].visitCounter = oldQuest.locations[i].visitCounter;

        for (int i = 0; i < newQuest.passages.Count && i < oldQuest.passages.Count; i++)
            newQuest.passages[i].visitCounter = oldQuest.passages[i].visitCounter;

        foreach (Passage passage in newQuest.passages)
            passage.FindControversials(newQuest);

        return newQuest;
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
            pictureNode.SetNewPicture(imageName, player.quest.questName, mayBeSame: false);

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