using System;
using System.Collections.Generic;

public class Quest : ICloneable
{
    public string questName;
    public string displayName;
    public string descrition;
    public string startMusic;
    public string startImage;
    public int locationCount;
    public int passageCount;
    public int order;

    public List<Parameter> parameters = new List<Parameter>();
    public List<Location> locations = new List<Location>();
    public List<Passage> passages = new List<Passage>();

    public Location FindLocationWith(int id)
    {
        foreach (var location in locations)
        {
            if (location.id == id)
                return location;
        }

        return null;
    }

    public Passage FindPassageWith(int id)
    {
        foreach (var passage in passages)
        {
            if (passage.id == id)
                return passage;
        }

        return null;
    }

    public Location FindStartLocation()
    {
        foreach (var location in locations)
        {
            if (location.locationType == LocationType.Start)
                return location;
        }

        return null;
    }

    public List<Passage> FindAllPassagesFromLocation(int locationId)
    {
        var result = new List<Passage>();

        foreach (var passage in passages)
        {
            if (passage.from == locationId)
                result.Add(passage);
        }

        return result;
    }

    public object Clone()
    {
        var clone = (Quest)MemberwiseClone();

        clone.parameters = new List<Parameter>();
        clone.locations = new List<Location>();
        clone.passages = new List<Passage>();

        clone.displayName = displayName;
        clone.startImage = startImage;
        clone.startMusic = startMusic;
        clone.descrition = descrition;

        foreach (var parameter in parameters)
            clone.parameters.Add((Parameter)parameter.Clone());

        foreach (var location in locations)
            clone.locations.Add((Location)location.Clone());

        foreach (var passage in passages)
            clone.passages.Add((Passage)passage.Clone());

        return clone;
    }

    public override string ToString()
    {
        return $"Quest: questName={questName}, parameters.count={parameters.Count}";
    }
}