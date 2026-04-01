using UnityEngine;

public class Director : MonoBehaviour
{
    //public RectTransform uiCanvas;    
    //public GameObject warningPref;  

    public static Director Instance;

    GameObject warningPanel;

    private void Awake()
    {
        Instance = this;      
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
        SaveLoadManager.Instance.SavePlayer();
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.Instance.SavePlayer();
    }
}

public enum MainTogleType
{
    Location,
    Pass,
    Move
}

