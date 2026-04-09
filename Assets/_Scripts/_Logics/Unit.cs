using System.Collections.Generic;

public class Unit
{
    public int id;
    public int passability;
    public int visitCounter;   

    public List<Influence> influences = new List<Influence>();
    public List<ParamsAction> paramsActions = new List<ParamsAction>();
}

public class Influence
{
    public InfluenceType influenceType;
    public int value;
    public string formula;

    public override string ToString()
    {
        return $"Influence: influenceType={influenceType}, value={value}, formula={formula}";
    }
}

public enum ParamsAction
{
    Hide,
    Show,
    Ignore
}

public enum InfluenceType
{
    Units,
    Percent,
    Value,
    Formula
}