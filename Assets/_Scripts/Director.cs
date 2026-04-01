using UnityEngine;

public class Director : MonoBehaviour
{ 
    public static Director Instance; 

    private void Awake()
    {
        Instance = this;      
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