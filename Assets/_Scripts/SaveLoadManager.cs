using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadManager
{
    private const string SavesFolderName = "Saves";
    private const string PlayerFileName = "player.txt";
    private const string QuestsResourcesPath = "_Quests";

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

        string json = JsonConvert.SerializeObject(player, serializerSettings);
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
        LoadedPlayer = JsonConvert.DeserializeObject<Player>(json, serializerSettings);
    }

    private void LoadQuestFromResources()
    {
        TextAsset[] allQuests = Resources.LoadAll<TextAsset>(QuestsResourcesPath);
        if (allQuests.Length == 0)
            return;

        TextAsset questAsset = allQuests[0];
        Quest.Instance = JsonConvert.DeserializeObject<Quest>(questAsset.text, serializerSettings);
    }

    private void EnsureSaveFolderExists()
    {
        if (!Directory.Exists(saveFolderPath))
            Directory.CreateDirectory(saveFolderPath);
    }

    private string GetPlayerSavePath()
    {
        return Path.Combine(saveFolderPath, PlayerFileName);
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