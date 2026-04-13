using System.Collections.Generic;
using UnityEngine;
using QuestFormula;

public class PassageResolver
{
    private readonly GamePanel gamePanel;
    private readonly TextParser textParser;

    public PassageResolver(GamePanel gamePanel, TextParser textParser)
    {
        this.gamePanel = gamePanel;
        this.textParser = textParser;
    }

    public List<PassageInfo> ResolveVisiblePassages(Location location)
    {
        List<Passage> workPassages = BuildWorkPassages(location);
        workPassages.Sort();

        List<PassageInfo> visiblePassages = new List<PassageInfo>();

        foreach (Passage passage in workPassages)
        {
            bool allConditions = CheckAllConditions(passage);

            if (allConditions || passage.alwaysShow)
            {
                visiblePassages.Add(new PassageInfo
                {
                    pass = passage,
                    isAllConditions = allConditions
                });
            }
        }

        return visiblePassages;
    }

    private List<Passage> BuildWorkPassages(Location location)
    {
        List<Passage> passages = gamePanel.Player.quest.FindAllPassagesFromLocation(location.id);
        List<Passage> workPassages = new List<Passage>();

        List<Passage> toDeleteByProbability = new List<Passage>();

        foreach (Passage passage in passages)
        {
            if (passage.priority < 1)
            {
                if (Random.Range(0, 100) <= passage.priority * 100)
                    workPassages.Add(passage);

                toDeleteByProbability.Add(passage);
            }
        }

        foreach (Passage passage in toDeleteByProbability)
            passages.Remove(passage);

        HashSet<int> excludedIds = new HashSet<int>();

        for (int n = passages.Count - 1; n >= 0; n--)
        {
            Passage passage = passages[n];

            if (excludedIds.Contains(passage.id))
                continue;

            if (workPassages.Contains(passage))
                continue;

            if (passage.controversials.Count == 0 && passage.priority >= 1)
            {
                workPassages.Add(passage);
                continue;
            }

            List<Passage> allControversials = new List<Passage>(passage.controversials);
            allControversials.Add(passage);

            List<Vector2> segments = new List<Vector2>();

            int last = 0;
            foreach (Passage controversialPassage in allControversials)
            {
                Vector2 segment = new Vector2(last, last + (int)controversialPassage.priority - 1);
                last += (int)controversialPassage.priority;
                segments.Add(segment);
            }

            int randomValue = Random.Range(0, last);

            int selectedIndex = 0;
            for (int i = 0; i < segments.Count; i++)
            {
                Vector2 segment = segments[i];
                if (randomValue >= segment.x && randomValue <= segment.y)
                {
                    selectedIndex = i;
                    break;
                }
            }

            workPassages.Add(allControversials[selectedIndex]);

            foreach (Passage controversialPassage in allControversials)
                excludedIds.Add(controversialPassage.id);
        }

        return workPassages;
    }

    private bool CheckAllConditions(Passage passage)
    {
        Location toLocation = gamePanel.Player.quest.FindLocationWith(passage.to);

        bool passCondition =
            (passage.passability == 0 || passage.visitCounter < passage.passability) &&
            (toLocation.passability == 0 || toLocation.visitCounter < toLocation.passability);

        bool logicalCondition = true;
        bool inRange = true;
        bool takesOrNotValues = true;
        bool multipleOrNotValues = true;

        if (!string.IsNullOrEmpty(passage.logicalCondition))
        {
            try
            {
                logicalCondition = FormulaEvaluator.EvaluateBool(passage.logicalCondition, textParser.FillFormulaDict());
            }
            catch
            {
                Debug.LogWarning("Invalid logical condition formula!");
            }
        }

        for (int i = 0; i < gamePanel.Player.quest.parameters.Count; i++)
        {
            Parameter parameter = gamePanel.Player.quest.parameters[i];

            NecessaryRange necessaryRange = passage.necessaryRanges[i];
            if (necessaryRange.isOn && (parameter.value < necessaryRange.min || parameter.value > necessaryRange.max))
            {
                inRange = false;
                break;
            }

            TakenValues takenValues = passage.takenValues[i];
            if (!string.IsNullOrEmpty(takenValues.formula))
            {
                try
                {
                    int value = FormulaEvaluator.Evaluate(takenValues.formula, textParser.FillFormulaDict());

                    if (takenValues.nonTaken)
                        takesOrNotValues = parameter.value != value;
                    else
                        takesOrNotValues = parameter.value == value;
                }
                catch
                {
                    Debug.LogWarning("Invalid accepted values formula!");
                }
            }

            MultipleValues multipleValues = passage.multipleValues[i];
            if (!string.IsNullOrEmpty(multipleValues.formula))
            {
                try
                {
                    int value = FormulaEvaluator.Evaluate(multipleValues.formula, textParser.FillFormulaDict());

                    if (multipleValues.nonMultiple)
                        multipleOrNotValues = parameter.value % value != 0;
                    else
                        multipleOrNotValues = parameter.value % value == 0;
                }
                catch
                {
                    Debug.LogWarning("Invalid divisibility formula!");
                }
            }
        }

        return passCondition && logicalCondition && inRange && takesOrNotValues && multipleOrNotValues;
    }
}

public struct PassageInfo
{
    public Passage pass;
    public bool isAllConditions;
}