using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Newtonsoft.Json;
using System.IO;

public class SaveLoadManager 
{  
    public static string last_save = "last_save";

    private static SaveLoadManager instance;

    public static SaveLoadManager Manager
    {
        get { if (instance == null) instance = new SaveLoadManager(); return instance; }
        set { instance = value; }
    }

    public void Init() { }

    public SaveLoadManager()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,           
        };

        TextAsset[] allQuests = Resources.LoadAll<TextAsset>("_Quests");

        if(allQuests.Length > 0)
        {
            TextAsset asset = allQuests[0];
            Quest.Instance = JsonConvert.DeserializeObject<Quest>(asset.text, settings);           
        }       
    }
 
    /*
    public void Save(string saveFile)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        };
        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

        string jsonString = JsonConvert.SerializeObject(Quest.Instance, settings);
        File.WriteAllText(SAVE_FOLDER + saveFile + ".txt", jsonString);

    }

    public void Load(string saveFile)
    {
        saveFile += ".txt";

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None
        };

        if (!File.Exists(SAVE_FOLDER + saveFile))
        {           
            Director.Instance.WarningWithText("Нет такого файла!");

            return;
        }           

        string saveString = File.ReadAllText(SAVE_FOLDER + saveFile);

        Quest.Instance = JsonConvert.DeserializeObject<Quest>(saveString, settings);
    }*/
}
