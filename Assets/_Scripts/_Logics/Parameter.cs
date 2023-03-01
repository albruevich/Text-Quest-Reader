using System;
using System.Collections.Generic;
using UnityEngine;

public class Parameter : ICloneable
{
    public string workingName;
    public ParamType paramType;

    public int index;
    public int value;
    public int startValue;
    public int minValue;
    public int maxValue;

    public bool isActive;
    public bool isCriticMax;

    public string criticText;

    //для игры
    public bool isHidden;

    public List<ParamsRange> paramsRanges;

    public Parameter(int index)
    {
        this.index = index;

        workingName = "Параметр " + index;
        paramType = ParamType.Usual;
        maxValue = 1;
        isActive = true;
        criticText = "Сообщение достижения критического значения";

        paramsRanges = new List<ParamsRange>();
        AddRange();
    }

    public void AddRange()
    {
        ParamsRange range = new ParamsRange
        {
            min = 0,
            max = maxValue,
            output = "Параметр номер " + index + ": <>"
        };

        paramsRanges.Add(range);

        UpdateAllRanges();
    }
  
    public void DeleteRange()
    {       
        paramsRanges.RemoveAt(paramsRanges.Count-1);

        UpdateAllRanges();
    }

    public void UpdateAllRanges()
    {
        float step = (maxValue - minValue) / paramsRanges.Count;
        int lastMax = minValue;
       
        if((int)step > 0)
        {
            int i = 0;
            foreach (ParamsRange r in paramsRanges)
            {
                r.min = lastMax == minValue ? minValue : lastMax + 1;
                r.max = minValue + (int)(step * (i + 1));
                lastMax = r.max;

                if (i == paramsRanges.Count - 1)
                    r.max = maxValue;

                i++;
            }
        }
        else
        {
            int i = minValue;
            foreach (ParamsRange r in paramsRanges)
            {
                r.min = r.max = i;             
                i++;
            }
        }        
    }

    public void CorrectRanges(int index, bool isMin)
    {
        ParamsRange curRange = paramsRanges[index];

        int lastMin;
        int lastMax;

        if (isMin)
        {
            lastMax = curRange.min;
            if (curRange.min > curRange.max) curRange.max = curRange.min;
            lastMin = curRange.max;
        }
        else
        {
            lastMin = curRange.max;
            if (curRange.max < curRange.min) curRange.min = curRange.max;
            lastMax = curRange.min;
        }

        for (int i = index + 1; i < paramsRanges.Count; i++)
        {
            ParamsRange r = paramsRanges[i];
            if (r.min <= lastMin) r.min = Mathf.Min(lastMin + 1, maxValue);
            if (r.max <= lastMin) r.max = Mathf.Min(lastMin + 1, maxValue);
            lastMin = r.max;
        }

        for (int i = index - 1; i >= 0; i--)
        {
            ParamsRange r = paramsRanges[i];
            if (r.max >= lastMax) r.max = Mathf.Max(lastMax - 1, minValue);
            if (r.min >= lastMax) r.min = Mathf.Max(lastMax - 1, minValue);
            lastMax = r.min;
        }
    }

    public ParamsRange FindCorrectRange()
    {
        ParamsRange paramsRange = null;      
        foreach (ParamsRange range in paramsRanges)        
            if(range.min <= value && range.max >= value)
            {
                paramsRange = range;
                break;
            }       
        return paramsRange;
    }

    public object Clone()
    {
        Parameter clone = (Parameter)MemberwiseClone();
        clone.paramsRanges = new List<ParamsRange>(paramsRanges);        
        return clone;
    }

    public override string ToString()
    {
        return string.Format("Parameter: workingName={0}, paramType={1}, value={2}, startValue={3}, minValue={4}, maxValue={5}, isActive={6}, isCriticMax={7}",
            workingName, paramType, value, startValue, minValue, maxValue, isActive, isCriticMax);
    }
}

public class ParamsRange
{
    public int min;
    public int max;
    public string output;

    public override string ToString()
    {
        return string.Format("ParamsRange: min={0}, max={1}, output={2}", min, max, output);
    }
}

public enum ParamType
{
    Usual,
    Successful,
    Failed
}
