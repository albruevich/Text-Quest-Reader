using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadManager 
{      
    public static string SAVE_FOLDER;
    public static string fileName = "player.txt";

    private static SaveLoadManager instance;

    JsonSerializerSettings settings;

    public Player loadedPlayer;

    public static SaveLoadManager Manager
    {
        get { if (instance == null) instance = new SaveLoadManager(); return instance; }
        set { instance = value; }
    }

    public void Init() {}

    public SaveLoadManager()
    {
        SAVE_FOLDER = Application.persistentDataPath + "/Saves/";

        if (!Directory.Exists(SAVE_FOLDER))
            Directory.CreateDirectory(SAVE_FOLDER);

        settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

        TextAsset[] allQuests = Resources.LoadAll<TextAsset>("_Quests");

        if(allQuests.Length > 0)
        {
            TextAsset asset = allQuests[0];
            Quest.Instance = JsonConvert.DeserializeObject<Quest>(asset.text, settings);           
        }

        LoadPlayer();
    }

    public void SavePlayer()
    {
        if(GamePanel.Instance.player != null)
        {
            string jsonString = JsonConvert.SerializeObject(GamePanel.Instance.player, settings);
            File.WriteAllText(SAVE_FOLDER + fileName, jsonString);
        }       
    }

    public void LoadPlayer()
    {
        string filePath = SAVE_FOLDER + fileName;

        if (File.Exists(filePath))
        {
            string saveString = File.ReadAllText(filePath);
            loadedPlayer = JsonConvert.DeserializeObject<Player>(saveString, settings);            
        }
    }
}
