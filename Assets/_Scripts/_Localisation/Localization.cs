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
                { Lang.en, "You win! Play again" },
                { Lang.ua, "Ви перемогли! Грати ще" },
                { Lang.ru, "Вы победили! Играть еще раз" }
            }
        },

        {
            LocKeys.YouLose,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You lose! Play again" },
                { Lang.ua, "Ви програли! Грати ще" },
                { Lang.ru, "Вы проиграли! Играть еще раз" }
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
            LocKeys.ResetGame,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Reset Game" },
                { Lang.ua, "Скинути Гру" },
                { Lang.ru, "Сбросить Игру" }
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

    