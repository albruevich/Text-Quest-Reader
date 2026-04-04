using System.Collections.Generic;
using Z.Expressions;
using UnityEngine;

public class TextParser
{
    private readonly GamePanel gamePanel;
    private const string replacementColor = "<color=#6BBEFF>";

    public TextParser(GamePanel gamePanel)
    {
        this.gamePanel = gamePanel;
    }

    public string Parse(string text, bool ignoreColor = false)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        string result = text;

        List<string> expressions = GetBetween(result, "{", "}");

        foreach (string expression in expressions)
        {
            try
            {
                int value = Eval.Execute<int>(expression.Replace("{", "").Replace("}", ""), FillFormulaDict());
                string replacement = ignoreColor ? value.ToString() : replacementColor + value + "</color>";

                result = result.Replace(expression, replacement);
            }
            catch
            {
                Debug.LogWarning("Invalid substitution formula!");
            }
        }

        return result;
    }

    public string CleanLeadingSpaces(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        int spaces = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
                spaces++;
            else
                break;
        }

        return spaces > 0 ? text.Remove(0, spaces) : text;
    }

    public List<string> GetBetween(string source, string start, string end)
    {
        List<string> result = new List<string>();

        while (source.Contains(start) && source.Contains(end))
        {
            int startIndex = source.IndexOf(start, System.StringComparison.Ordinal);
            int endIndex = source.IndexOf(end, startIndex, System.StringComparison.Ordinal) + end.Length;

            string content = source.Substring(startIndex, endIndex - startIndex);
            result.Add(content);

            source = source.Replace(content, "");
        }

        return result;
    }   

    public Dictionary<string, object> FillFormulaDict()
    {
        Dictionary<string, object> paramsDict = new Dictionary<string, object>();

        foreach (Parameter p in gamePanel.Player.quest.parameters)
            paramsDict.Add("p" + p.index, p.value);

        // писать в поле Expression типо: p9 - rnd(1, 3)
        paramsDict.Add("rnd", new System.Func<int, int, int>((min, max) => Random.Range(min, max + 1)));

        return paramsDict;
    }
}