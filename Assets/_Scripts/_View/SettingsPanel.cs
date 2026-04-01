using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public void ActionClose()
    {
        Destroy(gameObject);
    }

    public void ActionRestart()
    {
        GamePanel.Instance.RestartQuest();
        Destroy(gameObject);
    }
}
