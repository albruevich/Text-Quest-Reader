using System.Collections.Generic;
using UnityEngine;

public static class Localization
{
    public const string LANGUAGE_KEY = "language";

    public static Lang CurrentLang = Lang.en;

    private static readonly Dictionary<string, Dictionary<Lang, string>> data = new()
    {
        {
            LocKeys.Next,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Next" },
                { Lang.es, "Siguiente" },
                { Lang.fr, "Suivant" },
                { Lang.de, "Weiter" },
                { Lang.it, "Avanti" },
                { Lang.pt, "Próximo" },
                { Lang.uk, "Далі" },
                { Lang.pl, "Dalej" },
                { Lang.ru, "Далее" }
            }
        },

        {
            LocKeys.YouWin,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You win!" },
                { Lang.es, "¡Has ganado!" },
                { Lang.fr, "Vous avez gagné !" },
                { Lang.de, "Du hast gewonnen!" },
                { Lang.it, "Hai vinto!" },
                { Lang.pt, "Você venceu!" },
                { Lang.uk, "Ви перемогли!" },
                { Lang.pl, "Wygrałeś!" },
                { Lang.ru, "Вы победили!" }
            }
        },

        {
            LocKeys.YouLose,
            new Dictionary<Lang, string>
            {
                { Lang.en, "You lose!" },
                { Lang.es, "¡Has perdido!" },
                { Lang.fr, "Vous avez perdu !" },
                { Lang.de, "Du hast verloren!" },
                { Lang.it, "Hai perso!" },
                { Lang.pt, "Você perdeu!" },
                { Lang.uk, "Ви програли!" },
                { Lang.pl, "Przegrałeś!" },
                { Lang.ru, "Вы проиграли!" }
            }
        },

        {
            LocKeys.Settings,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Settings" },
                { Lang.es, "Configuración" },
                { Lang.fr, "Paramètres" },
                { Lang.de, "Einstellungen" },
                { Lang.it, "Impostazioni" },
                { Lang.pt, "Configurações" },
                { Lang.uk, "Налаштування" },
                { Lang.pl, "Ustawienia" },
                { Lang.ru, "Настройки" }
            }
        },

        {
            LocKeys.AbandonQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Abandon Quest" },
                { Lang.es, "Abandonar misión" },
                { Lang.fr, "Abandonner la quête" },
                { Lang.de, "Quest aufgeben" },
                { Lang.it, "Abbandona missione" },
                { Lang.pt, "Abandonar missão" },
                { Lang.uk, "Покинути квест" },
                { Lang.pl, "Porzuć quest" },
                { Lang.ru, "Бросить квест" }
            }
        },

        {
            LocKeys.Quit,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Quit" },
                { Lang.es, "Salir" },
                { Lang.fr, "Quitter" },
                { Lang.de, "Beenden" },
                { Lang.it, "Esci" },
                { Lang.pt, "Sair" },
                { Lang.uk, "Вийти" },
                { Lang.pl, "Wyjście" },
                { Lang.ru, "Выйти" }
            }
        },

        {
            LocKeys.StartQuest,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Start Selected Quest" },
                { Lang.es, "Iniciar misión seleccionada" },
                { Lang.fr, "Démarrer la quête sélectionnée" },
                { Lang.de, "Ausgewählte Quest starten" },
                { Lang.it, "Avvia missione selezionata" },
                { Lang.pt, "Iniciar missão selecionada" },
                { Lang.uk, "Почати обраний квест" },
                { Lang.pl, "Rozpocznij wybrany quest" },
                { Lang.ru, "Начать выбранный квест" }
            }
        },

        {
            LocKeys.AddQuests,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Add Local Quests\n<size=52>Copy quest folders here" },
                { Lang.es, "Agregar misiones locales\n<size=52>Copia aquí las carpetas" },
                { Lang.fr, "Ajouter des quêtes locales\n<size=52>Copiez les dossiers ici" },
                { Lang.de, "Lokale Quests hinzufügen\n<size=52>Ordner hierher kopieren" },
                { Lang.it, "Aggiungi missioni locali\n<size=52>Copia qui le cartelle" },
                { Lang.pt, "Adicionar missões locais\n<size=52>Copie as pastas aqui" },
                { Lang.uk, "Додати локальні квести\n<size=52>Скопіюйте папки сюди" },
                { Lang.pl, "Dodaj lokalne questy\n<size=52>Skopiuj foldery tutaj" },
                { Lang.ru, "Добавить локальные квесты\n<size=52>Скопируйте папки сюда" }
            }
        },

        {
            LocKeys.Refresh,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Refresh" },
                { Lang.es, "Actualizar" },
                { Lang.fr, "Actualiser" },
                { Lang.de, "Aktualisieren" },
                { Lang.it, "Aggiorna" },
                { Lang.pt, "Atualizar" },
                { Lang.uk, "Оновити" },
                { Lang.pl, "Odśwież" },
                { Lang.ru, "Обновить" }
            }
        },

        {
            LocKeys.Source,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Source:" },
                { Lang.es, "Fuente:" },
                { Lang.fr, "Source :" },
                { Lang.de, "Quelle:" },
                { Lang.it, "Fonte:" },
                { Lang.pt, "Fonte:" },
                { Lang.uk, "Джерело:" },
                { Lang.pl, "Źródło:" },
                { Lang.ru, "Источник:" }
            }
        },

        {
            LocKeys.Local,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Local" },
                { Lang.es, "Local" },
                { Lang.fr, "Local" },
                { Lang.de, "Lokal" },
                { Lang.it, "Locale" },
                { Lang.pt, "Local" },
                { Lang.uk, "Локально" },
                { Lang.pl, "Lokalne" },
                { Lang.ru, "Локально" }
            }
        },

        {
            LocKeys.Remote,
            new Dictionary<Lang, string>
            {
                { Lang.en, "Remote" },
                { Lang.es, "Remoto" },
                { Lang.fr, "Distant" },
                { Lang.de, "Online" },
                { Lang.it, "Remoto" },
                { Lang.pt, "Remoto" },
                { Lang.uk, "Мережа" },
                { Lang.pl, "Sieć" },
                { Lang.ru, "Сеть" }
            }
        },
    };

    public static void SetCurrentLanguage(string langCode)
    {
        CurrentLang = ParseLanguage(langCode);
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

    public static string GetForLanguage(string key, string language)
    {
        if (!data.TryGetValue(key, out var langs))
            return key;

        Lang requestedLang = ParseLanguage(language);

        if (langs.TryGetValue(requestedLang, out var value))
            return value;

        if (langs.TryGetValue(Lang.en, out var fallback))
            return fallback;

        return key;
    }

    private static Lang ParseLanguage(string language)
    {
        return language?.ToLower() switch
        {
            "en" => Lang.en,
            "es" => Lang.es,
            "fr" => Lang.fr,
            "de" => Lang.de,
            "it" => Lang.it,
            "pt" => Lang.pt,
            "uk" => Lang.uk,
            "pl" => Lang.pl,
            "ru" => Lang.ru,
            _ => Lang.en
        };
    }

    public static string GetLangCode(SystemLanguage lang)
    {
        return lang switch
        {
            SystemLanguage.English => "en",
            SystemLanguage.Spanish => "es",
            SystemLanguage.French => "fr",
            SystemLanguage.German => "de",
            SystemLanguage.Italian => "it",
            SystemLanguage.Portuguese => "pt",
            SystemLanguage.Ukrainian => "uk",
            SystemLanguage.Polish => "pl",
            SystemLanguage.Russian => "ru",
            _ => "en"
        };
    }
}