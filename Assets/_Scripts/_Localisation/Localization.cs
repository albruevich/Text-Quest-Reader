using System.Collections.Generic;

public static class Localization
{
    public static Lang CurrentLang = Lang.ru;

    private static readonly Dictionary<string, Dictionary<Lang, string>> data = new Dictionary<string, Dictionary<Lang, string>>
    {
        {
            LocKeys.Next,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Next" },
                { Lang.ua, "Далі" },
                { Lang.ru, "Далее" }
            }
        },

        {
            LocKeys.YouWin,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You win!" },
                { Lang.ua, "Ви перемогли!" },
                { Lang.ru, "Вы победили!" }
            }
        },

        {
            LocKeys.YouLose,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You lose!" },
                { Lang.ua, "Ви програли!" },
                { Lang.ru, "Вы проиграли!" }
            }
        },

        {
            LocKeys.Settings,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Settings" },
                { Lang.ua, "Налаштування" },
                { Lang.ru, "Настройки" }
            }
        },

        {
            LocKeys.AbandonQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Abandon Quest" },
                { Lang.ua, "Покинути Квест" },
                { Lang.ru, "Бросить Квест" }
            }
        },

        {
            LocKeys.Quit,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Quit" },
                { Lang.ua, "Покинути" },
                { Lang.ru, "Выйти" }
            }
        },

        {
            LocKeys.StartQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Start Selected Quest" },
                { Lang.ua, "Почати Обраний Квест" },
                { Lang.ru, "Начать Выбранный Квест" }
            }
        },

    };

    public static string Get(string key)
    {
        if (!data.TryGetValue(key, out var langs))
            return key;

        if (!langs.TryGetValue(CurrentLang, out var value))
            value = langs[Lang.en];

        return value;
    }
}

    