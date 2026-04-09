using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resetText;
    [SerializeField] private TMP_Text quitText;

    private void Start()
    {
        titleText.text = Localization.Get(LocKeys.Settings);
        resetText.text = Localization.Get(LocKeys.ResetGame);
        quitText.text = Localization.Get(LocKeys.Quit);
    }

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
