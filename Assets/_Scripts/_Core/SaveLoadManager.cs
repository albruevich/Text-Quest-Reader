using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager
{
    private const string SavesFolderName = "Saves";
    private const string SaveFileName = "save.json";
   
    private static SaveLoadManager instance;

    private readonly JsonSerializerSettings serializerSettings;
    private readonly string saveFolderPath;   

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
    }

    public void SavePlayer()
    {
        var player = GamePanel.Instance.Player;
        if (player == null)
            return;

        PlayerSaveData saveData = CreateSaveData(player);

        string json = JsonConvert.SerializeObject(saveData, serializerSettings);     
        File.WriteAllText(GetPlayerSavePath(), json);
    }

    public void ClearPlayerSaveData() => File.Delete(GetPlayerSavePath());    

    public Player LoadPlayer()
    {      
        string playerSavePath = GetPlayerSavePath();        

        if (!File.Exists(playerSavePath))
            return null;

        string json = File.ReadAllText(playerSavePath);
        PlayerSaveData saveData = JsonConvert.DeserializeObject<PlayerSaveData>(json, serializerSettings);       

        if (saveData == null)
            return null;

        Player player = null;

        try
        {
            Quest quest = LoadQuestFromFrolder(saveData.questName);

            Quest questClone = (Quest)quest.Clone();

            player = new Player
            {
                locationID = saveData.locationID,
                passageID = saveData.passageID,
                quest = questClone,
                gameOver = saveData.gameOver
            };

            if (!string.IsNullOrEmpty(saveData.lastPlayedMusic))
                AudioManager.Instance.PlayMusic(saveData.lastPlayedMusic, quest.questName, stoppable: false);

            RestoreParameters(questClone, saveData);
            RestoreLocations(questClone, saveData);
            RestorePassages(questClone, saveData);
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning(ex);
        }      

        return player;
    }

    private PlayerSaveData CreateSaveData(Player player)
    {
        PlayerSaveData saveData = new PlayerSaveData
        {
            locationID = player.locationID,
            passageID = player.passageID,   
            lastPlayedMusic = AudioManager.Instance.CurrentMusicName,
            questName = player.quest.questName,
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

            passage.FindControversials(quest);
        }
    }

    public Quest LoadQuestFromFrolder(string folderName)
    {
        string lang = PlayerPrefs.GetString("language", "en");

        string localizedPath = $"Quests/{folderName}/quest_{lang}";
        TextAsset questAsset = Resources.Load<TextAsset>(localizedPath);

        if (questAsset == null)
        {
            string defaultPath = $"Quests/{folderName}/quest";
            questAsset = Resources.Load<TextAsset>(defaultPath);
        }

        if (questAsset == null)
            return null;

        Quest quest = JsonConvert.DeserializeObject<Quest>(questAsset.text, serializerSettings);
        return quest;
    }

    private void EnsureSaveFolderExists()
    {
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);
    }

    private string GetPlayerSavePath() => Path.Combine(saveFolderPath, SaveFileName);    

    private static JsonSerializerSettings CreateSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }
}