using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Director : MonoBehaviour
{
    //public RectTransform uiCanvas;    
    //public GameObject warningPref;  

    public static Director Instance;

    GameObject warningPanel;

    private void Awake()
    {
        Instance = this;
        SaveLoadManager.Manager.Init();
    }
    
    public void WarningWithText(string text)
    {
        if (warningPanel != null)
            Destroy(warningPanel);

        //todo
        //warningPanel = Instantiate(warningPref, uiCanvas);
        //warningPanel.GetComponent<WarningPanel>().warningText.text = text;
    }         
             
    private void OnApplicationPause(bool pause)
    {
        SaveLoadManager.Manager.SavePlayer();
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.Manager.SavePlayer();
    }
}

public enum MainTogleType
{
    Location,
    Pass,
    Move
}

