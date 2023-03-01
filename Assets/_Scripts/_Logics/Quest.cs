using System.Collections.Generic;
using System;

public class Quest : ICloneable
{
    private static Quest instance;

    public static Quest Instance
    {
        get { if (instance == null) instance = new Quest(); return instance; }
        set { instance = value; }
    }

    public string questName;
    public int locationCount;
    public int passageCount;
    public List<Parameter> parameters = new List<Parameter>();
    public List<Location> locations = new List<Location>();
    public List<Passage> passages = new List<Passage>();   

    public Location FindLocationWith(int id)
    {
        Location location = null;
        foreach(Location loc in locations)        
            if(loc.id == id)
            {
                location = loc;
                break;
            }       
        return location;
    }

    public Passage FindPassageWith(int id)
    {
        Passage passage = null;
        foreach (Passage pas in passages)
            if (pas.id == id)
            {
                passage = pas;
                break;
            }
        return passage;
    }

    public Location FindStartLocation()
    {
        Location location = null;
        foreach (Location loc in locations)
            if (loc.locationType == LocationType.Start)
            {
                location = loc;
                break;
            }
        return location;
    }

    public List<Passage> FindAllPassagesFromLocation(int locID)
    {
        List<Passage> fromPassages = new List<Passage>();
        foreach(Passage p in passages)        
            if (p.from == locID)
                fromPassages.Add(p);       
        return fromPassages;
    }

    public object Clone()
    {
        Quest clone = (Quest)MemberwiseClone();
        clone.parameters = new List<Parameter>();
        clone.locations = new List<Location>();
        clone.passages = new List<Passage>();

        foreach (Parameter parameter in parameters)
            clone.parameters.Add((Parameter)parameter.Clone());

        foreach (Location location in locations)        
            clone.locations.Add((Location)location.Clone());

        foreach (Passage passage in passages)
            clone.passages.Add((Passage)passage.Clone());

        return clone;
    }

    public override string ToString()
    {
        return string.Format("Quest: questName={0}, parameters.count={1}", questName, parameters.Count);
    }
}
