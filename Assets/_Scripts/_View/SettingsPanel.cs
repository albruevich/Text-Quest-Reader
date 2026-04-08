using UnityEngine;

public class SettingsPanel : MonoBehaviour
{
    public void ActionClose()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);
        Destroy(gameObject);
    }

    public void ActionRestart()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);
        GamePanel.Instance.RestartQuest();
        Destroy(gameObject);
    }

    public void ActionQuit()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
