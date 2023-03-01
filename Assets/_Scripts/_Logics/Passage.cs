using System.Collections.Generic;
using System;
using UnityEngine;

public class Passage : Unit, ICloneable, IComparable
{   
    public int from;
    public int to;   
    public int same;

    public string question;
    public string description;
    public string logicalCondition;
    public float priority;
    public int displayOrder;
    public bool alwaysShow;    

    //для игры
    public bool ignoreDemonstration;

    public List<NecessaryRange> necessaryRanges = new List<NecessaryRange>();
    public List<TakenValues> takenValues = new List<TakenValues>();
    public List<MultipleValues> multipleValues = new List<MultipleValues>();

    [NonSerialized]
    public List<Passage> controversials = new List<Passage>();

    public object Clone()
    {
        Passage clone = (Passage)MemberwiseClone();
        clone.influences = new List<Influence>(influences);
        clone.paramsActions = new List<ParamsAction>(paramsActions);
        clone.necessaryRanges = new List<NecessaryRange>(necessaryRanges);
        clone.takenValues = new List<TakenValues>(takenValues);
        clone.multipleValues = new List<MultipleValues>(multipleValues);
        return clone;
    }

    public virtual int CompareTo(object otherObject)
    {
        Passage other = (Passage)otherObject;
        if (displayOrder < other.displayOrder) return -1;
        if (displayOrder > other.displayOrder) return 1;
        return 0;
    }

    public static void RecountAllPassages()
    {
        List<Passage> copy = new List<Passage>();

        foreach (Passage passage in Quest.Instance.passages)
        {
            int same = 0;
            foreach (Passage p in copy)
            {
                if ((p.from == passage.from && p.to == passage.to) || (p.from == passage.to && p.to == passage.from))
                    same++;
            }

            passage.same = same;
            copy.Add(passage);           
        }            
    }

    public override string ToString()
    {
        return string.Format("Passage: id={0}, from={1}, to={2}, same={3}", id, from, to, same);
    }

    public void FindControversials()
    {
        controversials.Clear();

        foreach (Passage p in Quest.Instance.passages)
        {           
            if (p.from == from && p.id != id && p.question == question)
                controversials.Add(p);
        }       
    }
}

public class NecessaryRange
{
    public bool isOn;
    public int min;
    public int max;

    public override string ToString()
    {
        return string.Format("NecessaryRange: isOn={0}, min={1}, max={2}", isOn, min, max);
    }
}

public class TakenValues
{
    public bool nonTaken;
    public string formula;
}

public class MultipleValues
{
    public bool nonMultiple;
    public string formula;
}
