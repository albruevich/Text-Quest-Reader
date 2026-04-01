using UnityEngine;
using Z.Expressions;

public class LocationDescriptionResolver
{
    private readonly GamePanel gamePanel;

    public int LastDescriptionIndex { get; private set; }

    public LocationDescriptionResolver(GamePanel gamePanel)
    {
        this.gamePanel = gamePanel;
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
                index = Eval.Execute<int>(location.formula, FillFormulaDict());
            }
            catch
            {
                Director.Instance.WarningWithText("Invalid description selection formula!");
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

    private System.Collections.Generic.Dictionary<string, object> FillFormulaDict()
    {
        var parameters = new System.Collections.Generic.Dictionary<string, object>();

        foreach (Parameter parameter in gamePanel.Player.quest.parameters)
            parameters.Add("p" + parameter.index, parameter.value);

        return parameters;
    }
}