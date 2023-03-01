using System.Collections.Generic;

public class Unit 
{
    public int id;
    public int passability;
    public List<Influence> influences = new List<Influence>();
    public List<ParamsAction> paramsActions = new List<ParamsAction>();

    //для проигрывателя
    public int visitCounter;
}

public class Influence
{
    public InfluenceType influenceType;
    public int value;
    public string formula;

    public override string ToString()
    {
        return string.Format("Influence: influenceType={0}, value={1}, formula={2}", influenceType, value, formula);
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