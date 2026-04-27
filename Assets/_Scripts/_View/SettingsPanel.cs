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

        int value = PlayerPrefs.GetString(Localization.LANGUAGE_KEY, "en") switch
        {
            "en" => 0,
            "es" => 1,
            "fr" => 2,
            "de" => 3,
            "it" => 4,
            "pt" => 5,
            "uk" => 6,
            "pl" => 7,
            "ru" => 8,
            _ => 0
        };

        langDropdown.SetValueWithoutNotify(value);
        langDropdown.RefreshShownValue();
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

        string newLang = dropdown.value switch
        {
            0 => "en",
            1 => "es",
            2 => "fr",
            3 => "de",
            4 => "it",
            5 => "pt",
            6 => "uk",
            7 => "pl",
            8 => "ru",
            _ => "en"
        };

        if (newLang == currentLang)
            return;

        PlayerPrefs.SetString(Localization.LANGUAGE_KEY, newLang);
        PlayerPrefs.Save();

        gamePanel.HandleLocalizations();
        HandleLocalizations();
    }

    private void HandleLocalizations()
    {
        titleText.text = Localization.Get(LocKeys.Settings);
        resetText.text = Localization.Get(LocKeys.AbandonQuest);
        quitText.text = Localization.Get(LocKeys.Quit);
    }
}