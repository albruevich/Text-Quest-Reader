using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager
{
    // Put your quest folder in Assets/Resources/Quests
    // Name your quest here 
    public const string QuestFolderName = "Dream"; 

    private const string SavesFolderName = "Saves";
    private const string SaveFileName = "save.txt";
   
    private static SaveLoadManager instance;

    private readonly JsonSerializerSettings serializerSettings;
    private readonly string saveFolderPath;

    public Player LoadedPlayer { get; private set; }

    public static SaveLoadManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SaveLoadManager();

            return instance;
        }
        set => instance = value;
    }

    public SaveLoadManager()
    {
        saveFolderPath = Path.Combine(Application.persistentDataPath, SavesFolderName);
        EnsureSaveFolderExists();

        serializerSettings = CreateSerializerSettings();

        LoadQuestFromResources();
        LoadPlayer();
    }

    public void SavePlayer()
    {
        var player = GamePanel.Instance.Player;
        if (player == null)
            return;

        PlayerSaveData saveData = CreateSaveData(player);

        string json = JsonConvert.SerializeObject(saveData, serializerSettings);
        string playerSavePath = GetPlayerSavePath();

        File.WriteAllText(playerSavePath, json);
    }

    public void LoadPlayer()
    {
        string playerSavePath = GetPlayerSavePath();

        Debug.Log(playerSavePath);

        if (!File.Exists(playerSavePath))
            return;

        string json = File.ReadAllText(playerSavePath);
        PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json, serializerSettings);

        if (saveData == null)
            return;

        Quest questClone = (Quest)Quest.Instance.Clone();

        Player player = new Player
        {
            locationID = saveData.locationID,
            passageID = saveData.passageID,
            gameOver = saveData.gameOver,
            quest = questClone
        };

        RestoreParameters(questClone, saveData);
        RestoreLocations(questClone, saveData);
        RestorePassages(questClone, saveData);

        LoadedPlayer = player;
    }

    private PlayerSaveData CreateSaveData(Player player)
    {
        PlayerSaveData saveData = new PlayerSaveData
        {
            locationID = player.locationID,
            passageID = player.passageID,
            gameOver = player.gameOver
        };

        foreach (Parameter parameter in player.quest.parameters)
        {
            saveData.parameterValues.Add(parameter.value);
            saveData.parameterHidden.Add(parameter.isHidden);
        }

        foreach (Location location in player.quest.locations)
        {
            if (location.visitCounter > 0)
                saveData.locationVisitCounters[location.id] = location.visitCounter;
        }

        foreach (Passage passage in player.quest.passages)
        {
            if (passage.visitCounter > 0)
                saveData.passageVisitCounters[passage.id] = passage.visitCounter;
        }

        return saveData;
    }

    private void RestoreParameters(Quest quest, PlayerSaveData saveData)
    {
        int count = Mathf.Min(quest.parameters.Count, saveData.parameterValues.Count);

        for (int i = 0; i < count; i++)
        {
            quest.parameters[i].value = saveData.parameterValues[i];

            if (i < saveData.parameterHidden.Count)
                quest.parameters[i].isHidden = saveData.parameterHidden[i];
        }
    }

    private void RestoreLocations(Quest quest, PlayerSaveData saveData)
    {
        foreach (Location location in quest.locations)
        {
            if (saveData.locationVisitCounters.TryGetValue(location.id, out int counter))
                location.visitCounter = counter;
        }
    }

    private void RestorePassages(Quest quest, PlayerSaveData saveData)
    {
        foreach (Passage passage in quest.passages)
        {
            if (saveData.passageVisitCounters.TryGetValue(passage.id, out int counter))
                passage.visitCounter = counter;

            passage.FindControversials();
        }
    }

    private void LoadQuestFromResources()
    {
        TextAsset questAsset = Resources.Load<TextAsset>("Quests/" + QuestFolderName + "/quest");       

        if (questAsset == null)
            return;
      
        Quest.Instance = JsonConvert.DeserializeObject<Quest>(questAsset.text, serializerSettings);
    }

    private void EnsureSaveFolderExists()
    {
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);
    }

    private string GetPlayerSavePath()
    {
        return Path.Combine(saveFolderPath, $"{Quest.Instance.questName}_{SaveFileName}");
    }

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }
}