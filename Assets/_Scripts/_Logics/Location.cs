using System;
using System.Collections.Generic;

public class Location : Unit, ICloneable
{
    public bool chooseWithFormula;
    public string formula;

    public int gridX;
    public int gridY;

    public LocationType locationType;

    public List<string> descriptions = new List<string>();
    public List<string> firstInPair = new List<string>();

    public object Clone()
    {
        var clone = (Location)MemberwiseClone();

        clone.descriptions = new List<string>(descriptions);
        clone.influences = new List<Influence>(influences);
        clone.paramsActions = new List<ParamsAction>(paramsActions);

        return clone;
    }
}

public enum LocationType
{
    Neutral,
    Start,
    Victory,
    Fail,
    Empty
}