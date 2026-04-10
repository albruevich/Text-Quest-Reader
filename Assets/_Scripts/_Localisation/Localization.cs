using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    public const string LANGUAGE_KEY = "language";

    public static Lang CurrentLang = Lang.en;

    private static readonly Dictionary<string, Dictionary<Lang, string>> data = new Dictionary<string, Dictionary<Lang, string>>
    {
        {
            LocKeys.Next,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Next" },
                { Lang.uk, "Далі" },
                { Lang.ru, "Далее" }
            }
        },

        {
            LocKeys.YouWin,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You win!" },
                { Lang.uk, "Ви перемогли!" },
                { Lang.ru, "Вы победили!" }
            }
        },

        {
            LocKeys.YouLose,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You lose!" },
                { Lang.uk, "Ви програли!" },
                { Lang.ru, "Вы проиграли!" }
            }
        },

        {
            LocKeys.Settings,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Settings" },
                { Lang.uk, "Налаштування" },
                { Lang.ru, "Настройки" }
            }
        },

        {
            LocKeys.AbandonQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Abandon Quest" },
                { Lang.uk, "Покинути Квест" },
                { Lang.ru, "Бросить Квест" }
            }
        },

        {
            LocKeys.Quit,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Quit" },
                { Lang.uk, "Вийти" },
                { Lang.ru, "Выйти" }
            }
        },

        {
            LocKeys.StartQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Start Selected Quest" },
                { Lang.uk, "Почати Обраний Квест" },
                { Lang.ru, "Начать Выбранный Квест" }
            }
        },

    };    

    public static void SetCurrentLanguage(string langCode)
    {
        switch(langCode)
        {
            case "en": CurrentLang = Lang.en; break;
            case "uk": CurrentLang = Lang.uk; break;
            case "ru": CurrentLang = Lang.ru; break;
            default: CurrentLang = Lang.en; break;
        }
    }

    public static string Get(string key)
    {
        if (!data.TryGetValue(key, out var langs))
            return key;

        if (langs.TryGetValue(CurrentLang, out var value))
            return value;

        if (langs.TryGetValue(Lang.en, out var fallback))
            return fallback;

        return key;
    }

    public static string GetLangCode(SystemLanguage lang)
    {
        switch (lang)
        {
            case SystemLanguage.English: return "en";
            case SystemLanguage.Russian: return "ru";
            case SystemLanguage.Ukrainian: return "uk";
            default: return "en";
        }
    }
}

    