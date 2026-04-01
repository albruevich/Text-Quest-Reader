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
    public bool isHidden;

    public string criticText;

    public List<ParamsRange> paramsRanges;

    public Parameter(int index)
    {
        this.index = index;

        workingName = "Parameter " + index;
        paramType = ParamType.Usual;
        maxValue = 1;
        isActive = true;
        criticText = "Message on reaching a critical value";

        paramsRanges = new List<ParamsRange>();
        AddRange();
    }

    public void AddRange()
    {
        var range = new ParamsRange
        {
            min = 0,
            max = maxValue,
            output = "Parameter number " + index + ": <>"
        };

        paramsRanges.Add(range);
        UpdateAllRanges();
    }

    public void DeleteRange()
    {
        paramsRanges.RemoveAt(paramsRanges.Count - 1);
        UpdateAllRanges();
    }

    public void UpdateAllRanges()
    {
        float step = (maxValue - minValue) / paramsRanges.Count;
        int lastMax = minValue;

        if ((int)step > 0)
        {
            int i = 0;

            foreach (var range in paramsRanges)
            {
                range.min = lastMax == minValue ? minValue : lastMax + 1;
                range.max = minValue + (int)(step * (i + 1));
                lastMax = range.max;

                if (i == paramsRanges.Count - 1)
                    range.max = maxValue;

                i++;
            }
        }
        else
        {
            int i = minValue;

            foreach (var range in paramsRanges)
            {
                range.min = i;
                range.max = i;
                i++;
            }
        }
    }

    public void CorrectRanges(int index, bool isMin)
    {
        var currentRange = paramsRanges[index];

        int lastMin;
        int lastMax;

        if (isMin)
        {
            lastMax = currentRange.min;

            if (currentRange.min > currentRange.max)
                currentRange.max = currentRange.min;

            lastMin = currentRange.max;
        }
        else
        {
            lastMin = currentRange.max;

            if (currentRange.max < currentRange.min)
                currentRange.min = currentRange.max;

            lastMax = currentRange.min;
        }

        for (int i = index + 1; i < paramsRanges.Count; i++)
        {
            var range = paramsRanges[i];

            if (range.min <= lastMin)
                range.min = Mathf.Min(lastMin + 1, maxValue);

            if (range.max <= lastMin)
                range.max = Mathf.Min(lastMin + 1, maxValue);

            lastMin = range.max;
        }

        for (int i = index - 1; i >= 0; i--)
        {
            var range = paramsRanges[i];

            if (range.max >= lastMax)
                range.max = Mathf.Max(lastMax - 1, minValue);

            if (range.min >= lastMax)
                range.min = Mathf.Max(lastMax - 1, minValue);

            lastMax = range.min;
        }
    }

    public ParamsRange FindCorrectRange()
    {
        foreach (var range in paramsRanges)
        {
            if (range.min <= value && range.max >= value)
                return range;
        }

        return null;
    }

    public object Clone()
    {
        var clone = (Parameter)MemberwiseClone();
        clone.paramsRanges = new List<ParamsRange>(paramsRanges);

        return clone;
    }

    public override string ToString()
    {
        return $"Parameter: workingName={workingName}, paramType={paramType}, value={value}, startValue={startValue}," +
            $" minValue={minValue}, maxValue={maxValue}, isActive={isActive}, isCriticMax={isCriticMax}";
    }
}

public class ParamsRange
{
    public int min;
    public int max;
    public string output;

    public override string ToString()
    {
        return $"ParamsRange: min={min}, max={max}, output={output}";
    }
}

public enum ParamType
{
    Usual,
    Successful,
    Failed
}