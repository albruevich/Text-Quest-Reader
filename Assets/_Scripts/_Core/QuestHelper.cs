using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class QuestHelper
{
    public static List<string> GetAllQuestFolders()
    {
        string questsPath = Path.Combine(Application.streamingAssetsPath, "Quests");

        List<string> result = new List<string>();

        if (!Directory.Exists(questsPath))
        {
            Debug.LogWarning($"Quests folder not found: {questsPath}");
            return result;
        }

        string[] directories = Directory.GetDirectories(questsPath);

        foreach (string dir in directories)
        {
            string folderName = Path.GetFileName(dir);
            result.Add(folderName);
        }

        return result;
    }
}