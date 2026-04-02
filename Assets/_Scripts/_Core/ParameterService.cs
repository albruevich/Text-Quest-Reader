using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Z.Expressions;
using Random = UnityEngine.Random;

public class ParameterService
{
    private readonly GamePanel gamePanel;
    private readonly TextParser textParser;
    private readonly RectTransform paramsContent;
    private readonly GameObject parameterTextPref;
    private readonly GameObject victoryCell;
    private readonly GameObject defeatCell;
    private readonly Transform questionsContent;

    public ParameterService(GamePanel gamePanel, TextParser textParser, RectTransform paramsContent, GameObject parameterTextPref,
                            GameObject victoryCell, GameObject defeatCell, Transform questionsContent)
    {
        this.gamePanel = gamePanel;
        this.textParser = textParser;
        this.paramsContent = paramsContent;
        this.parameterTextPref = parameterTextPref;
        this.victoryCell = victoryCell;
        this.defeatCell = defeatCell;
        this.questionsContent = questionsContent;
    }

    public void ApplyInfluences(Unit unit, Action<string> showMainText)
    {
        for (int j = 0; j < unit.influences.Count; j++)
        {
            Parameter parameter = gamePanel.Player.quest.parameters[j];

            if (!parameter.isActive)
                continue;

            Influence influence = unit.influences[j];

            switch (influence.influenceType)
            {
                case InfluenceType.Units:
                    parameter.value = Clamp(parameter.value + influence.value, parameter);
                    break;

                case InfluenceType.Percent:
                    parameter.value = Clamp(
                        (int)(parameter.value * (influence.value / 100f + 1f)),
                        parameter);
                    break;

                case InfluenceType.Value:
                    parameter.value = Clamp(influence.value, parameter);
                    break;

                case InfluenceType.Formula:
                    ApplyFormulaInfluence(influence.formula, parameter);
                    break;
            }

            if (parameter.paramType != ParamType.Usual &&
                ((parameter.isCriticMax && parameter.value >= parameter.maxValue) ||
                 (!parameter.isCriticMax && parameter.value <= parameter.minValue)))
            {
                gamePanel.Player.gameOver = true;

                showMainText(textParser.Parse(parameter.criticText));
                ClearQuestions();

                switch (parameter.paramType)
                {
                    case ParamType.Successful:
                        victoryCell.SetActive(true);
                        break;

                    case ParamType.Failed:
                        defeatCell.SetActive(true);
                        break;
                }
            }
        }
    }

    public void Demonstrate(Unit unit)
    {
        ApplyVisibilityActions(unit);
        ClearParameters();

        List<Parameter> visibleParameters = GetVisibleParameters();

        const float interval = 70f;
        int index = 0;

        foreach (Parameter parameter in visibleParameters)
        {
            ParamsRange range = parameter.FindCorrectRange();
            if (range == null)
            {
                index++;
                continue;
            }

            string output = range.output;
            output = output.Replace("<>", parameter.value.ToString());
            output = textParser.Parse(output, ignoreColor: true);

            for (int i = 0; i < gamePanel.Player.quest.parameters.Count; i++)
            {
                Parameter p = gamePanel.Player.quest.parameters[i];
                string key = $"p{i + 1}";
                output = output.Replace(key, p.value.ToString());
            }
       
            GameObject cell = UnityEngine.Object.Instantiate(parameterTextPref, paramsContent);            
            cell.GetComponent<TMP_Text>().text = output;

            index++;
        }

        RectTransform viewPort = (RectTransform)paramsContent.parent;
        paramsContent.sizeDelta = new Vector2(
            paramsContent.sizeDelta.x,
            Mathf.Max(viewPort.rect.height, index * interval));
    }

    private void ApplyFormulaInfluence(string formula, Parameter parameter)
    {
        if (string.IsNullOrEmpty(formula))
        {
            Debug.LogWarning($"Empty influence formula! Parameter: p{parameter.index}");
            return;
        }

        try
        {
            if (formula.Contains("rnd"))
            {
                List<string> list = textParser.GetBetween(formula, "(", ")");

                foreach (string str in list)
                {
                    formula = str.Replace("(", "");
                    formula = formula.Replace(")", "");
                }

                string[] ints = formula.Split(',');

                if (ints.Length > 1 &&
                    int.TryParse(ints[0], out int minValue) &&
                    int.TryParse(ints[1], out int maxValue))
                {
                    parameter.value = Random.Range(minValue, maxValue);
                }
            }
            else
            {
                int value = Eval.Execute<int>(formula, textParser.FillFormulaDict());
                parameter.value = Clamp(value, parameter);
            }
        }
        catch
        {
            Debug.LogWarning($"Invalid influence formula! Parameter: p{parameter.index}");
        }
    }

    private void ApplyVisibilityActions(Unit unit)
    {
        for (int j = 0; j < gamePanel.Player.quest.parameters.Count; j++)
        {
            Parameter parameter = gamePanel.Player.quest.parameters[j];
            ParamsAction action = unit.paramsActions[j];

            switch (action)
            {
                case ParamsAction.Hide:
                    parameter.isHidden = true;
                    break;

                case ParamsAction.Show:
                    parameter.isHidden = false;
                    break;
            }
        }
    }

    private List<Parameter> GetVisibleParameters()
    {
        List<Parameter> visibleParameters = new List<Parameter>();

        foreach (Parameter parameter in gamePanel.Player.quest.parameters)
        {
            if (!parameter.isActive || parameter.isHidden)
                continue;

            ParamsRange range = parameter.FindCorrectRange();
            if (range == null)
                continue;

            if (!string.IsNullOrEmpty(range.output))
                visibleParameters.Add(parameter);
        }

        return visibleParameters;
    }

    private void ClearParameters()
    {
        foreach (Transform tr in paramsContent)
            UnityEngine.Object.Destroy(tr.gameObject);
    }

    private void ClearQuestions()
    {
        foreach (Transform tr in questionsContent)
            UnityEngine.Object.Destroy(tr.gameObject);
    }

    private static int Clamp(int value, Parameter parameter)
    {
        return Mathf.Clamp(value, parameter.minValue, parameter.maxValue);
    }
}