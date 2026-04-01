using System;
using System.Collections.Generic;

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
    public bool ignoreDemonstration;

    public List<NecessaryRange> necessaryRanges = new List<NecessaryRange>();
    public List<TakenValues> takenValues = new List<TakenValues>();
    public List<MultipleValues> multipleValues = new List<MultipleValues>();

    [NonSerialized]
    public List<Passage> controversials = new List<Passage>();

    public object Clone()
    {
        var clone = (Passage)MemberwiseClone();

        clone.influences = new List<Influence>(influences);
        clone.paramsActions = new List<ParamsAction>(paramsActions);
        clone.necessaryRanges = new List<NecessaryRange>(necessaryRanges);
        clone.takenValues = new List<TakenValues>(takenValues);
        clone.multipleValues = new List<MultipleValues>(multipleValues);

        return clone;
    }

    public virtual int CompareTo(object otherObject)
    {
        var other = (Passage)otherObject;

        if (displayOrder < other.displayOrder)
            return -1;

        if (displayOrder > other.displayOrder)
            return 1;

        return 0;
    }

    public static void RecountAllPassages()
    {
        var copy = new List<Passage>();

        foreach (var passage in Quest.Instance.passages)
        {
            int same = 0;

            foreach (var existingPassage in copy)
            {
                if ((existingPassage.from == passage.from && existingPassage.to == passage.to) ||
                    (existingPassage.from == passage.to && existingPassage.to == passage.from))
                {
                    same++;
                }
            }

            passage.same = same;
            copy.Add(passage);
        }
    }

    public void FindControversials()
    {
        controversials.Clear();

        foreach (var passage in Quest.Instance.passages)
        {
            if (passage.from == from && passage.id != id && passage.question == question)
                controversials.Add(passage);
        }
    }

    public override string ToString()
    {
        return $"Passage: id={id}, from={from}, to={to}, same={same}";
    }
}

public class NecessaryRange
{
    public bool isOn;
    public int min;
    public int max;

    public override string ToString()
    {
        return $"NecessaryRange: isOn={isOn}, min={min}, max={max}";
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