using UnityEngine;

public class Director : MonoBehaviour
{           
    private void OnApplicationQuit()
    {
        SaveLoadManager.Instance.SavePlayer();
    }
}