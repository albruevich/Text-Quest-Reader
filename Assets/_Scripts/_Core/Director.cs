using UnityEngine;

public class Director : MonoBehaviour
{           
    private void OnApplicationPause(bool pause)
    {
        SaveLoadManager.Instance.SavePlayer();
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.Instance.SavePlayer();
    }
}