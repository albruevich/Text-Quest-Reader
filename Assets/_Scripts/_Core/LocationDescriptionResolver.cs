using UnityEngine;
using QuestFormula;

public class LocationDescriptionResolver
{
    private readonly TextParser textParser;

    public int LastDescriptionIndex { get; private set; }

    public LocationDescriptionResolver(TextParser textParser)
    {       
        this.textParser = textParser;
    }

    public string Resolve(Location location)
    {
        if (location.descriptions == null || location.descriptions.Count == 0)
            return string.Empty;

        if (location.descriptions.Count == 1 || location.locationType == LocationType.Empty)
        {
            LastDescriptionIndex = 0;
            return location.descriptions[0];
        }

        if (location.chooseWithFormula)
            return ResolveWithFormula(location);

        return ResolveSequentially(location);
    }

    private string ResolveWithFormula(Location location)
    {
        int index = 0;

        if (string.IsNullOrEmpty(location.formula))
        {
            index = Random.Range(0, location.descriptions.Count);
        }
        else
        {
            try
            {
                index = FormulaEvaluator.Evaluate(location.formula, textParser.FillFormulaDict());
            }
            catch
            {
                Debug.LogWarning("Invalid description selection formula!");
            }

            index -= 1;

            if (index < 0 || index > location.descriptions.Count - 1)
                index = 0;
        }

        LastDescriptionIndex = index;
        return location.descriptions[index];
    }

    private string ResolveSequentially(Location location)
    {
        LastDescriptionIndex = location.visitCounter % location.descriptions.Count;
        return location.descriptions[LastDescriptionIndex];
    }  
}