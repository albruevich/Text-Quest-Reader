using UnityEngine;
using TMPro;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text resetText;
    [SerializeField] private TMP_Text quitText;
    [SerializeField] private TMP_Dropdown langDropdown;
    [SerializeField] private TMP_Text versionText;

    private GamePanel gamePanel;

    public void Init(GamePanel gamePanel)
    {
        this.gamePanel = gamePanel;

        versionText.text = $"v{Application.version}";
    }

    private void Start()
    {
        HandleLocalizations();

        switch (PlayerPrefs.GetString(Localization.LANGUAGE_KEY, "en"))
        {
            case "en": langDropdown.SetValueWithoutNotify(0); break;
            case "uk": langDropdown.SetValueWithoutNotify(1); break;
            case "ru": langDropdown.SetValueWithoutNotify(2); break;
            default: langDropdown.SetValueWithoutNotify(0); break;
        }
    }

    public void ActionClose()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);
        Destroy(gameObject);
    }

    public void ActionRestart()
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);
        GamePanel.Instance.AbandonQuest();
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

    public void OnLangDropdown(TMP_Dropdown dropdown)
    {
        AudioManager.Instance.PlaySfx(SoundType.Click);

        string currentLang = PlayerPrefs.GetString(Localization.LANGUAGE_KEY, "en");
        string newLang = currentLang;

        switch (dropdown.value)
        {
            case 0: newLang = "en"; break;
            case 1: newLang = "uk"; break;
            case 2: newLang = "ru"; break;
        }

        if (newLang == currentLang)
            return;

        PlayerPrefs.SetString(Localization.LANGUAGE_KEY, newLang);
        PlayerPrefs.Save();

        gamePanel.HandleLocalizations();
        HandleLocalizations();

        gamePanel.ApplyLanguageChangeToQuestView();
    }

    private void HandleLocalizations()
    {
        titleText.text = Localization.Get(LocKeys.Settings);
        resetText.text = Localization.Get(LocKeys.AbandonQuest);
        quitText.text = Localization.Get(LocKeys.Quit);
    }
}