using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Director : MonoBehaviour
{
    //public RectTransform uiCanvas;    
    //public GameObject warningPref;  

    public static Director Instance;

    GameObject warningPanel;   

    static bool AUTO_LOAD = true;
  
    void Start()
    {     
        Instance = this;                       

        SaveLoadManager.Manager.Init();                                
    }  
    
    public void WarningWithText(string text)
    {
        if (warningPanel != null)
            Destroy(warningPanel);

        //warningPanel = Instantiate(warningPref, uiCanvas);
        //warningPanel.GetComponent<WarningPanel>().warningText.text = text;
    }      
    
    public Color ColorFromLocationType(LocationType locationType)
    {
        Color color = new Color();
        switch (locationType)
        {
            case LocationType.Start: color = new Color(0, 1, 1); break;
            case LocationType.Neutral: color = new Color(1, 1, 1); break;
            case LocationType.Fail: color = new Color(1, 0.5f, 0); break;
            case LocationType.Victory: color = new Color(0, 1, 0.5f); break;
            case LocationType.Empty: color = new Color(0.5f, 0.5f, 0.5f); break;
        }

        return color;
    }          

    //-----------------------------------------------------------------------
    public static string GetTextForUnit(Unit unit, int paramIndex)
    {
        string nameText = "";
       
        if (unit.influences.Count == 0 || paramIndex > unit.influences.Count)
            return nameText;       

        Influence influence = unit.influences[paramIndex - 1];
        string add = "";

        switch (influence.influenceType)
        {
            case InfluenceType.Units:
                if (influence.value > 0)
                    add = ", +" + influence.value;
                else if (influence.value < 0)
                    add = ", " + influence.value;
                break;

            case InfluenceType.Percent:
                if (influence.value > 0)
                    add = ", +" + influence.value + "%";
                else if (influence.value < 0)
                    add = ", " + influence.value + "%";
                break;

            case InfluenceType.Value:
                add = Mathf.Abs(influence.value) > 0 ? ", =" + influence.value : "";
                break;
            case InfluenceType.Formula:
                add = influence.formula != "" ? ", =" + influence.formula : "";
                break;
        }

        string actStr = "";
        ParamsAction paramsAction = unit.paramsActions[paramIndex - 1];

        switch (paramsAction)
        {
            case ParamsAction.Hide: actStr = ", (скрыть)"; break;
            case ParamsAction.Show: actStr = ", (показать)"; break;
        }

        nameText = add + actStr;

        if (unit.GetType() == typeof(Passage))
        {
            Passage passage = (Passage)unit;

            NecessaryRange range = passage.necessaryRanges[paramIndex - 1];
            string rangeStr = "";

            if (range.isOn)
                rangeStr = ", [" + range.min + ".." + range.max + "]";

            TakenValues takenValues = passage.takenValues[paramIndex - 1];
            string takenStr = "";
            if (takenValues.formula != null && takenValues.formula != "")
            {
                if (takenValues.nonTaken)
                    takenStr = ", !=" + takenValues.formula;
                else
                    takenStr = ", ==" + takenValues.formula;
            }

            MultipleValues multiple = passage.multipleValues[paramIndex - 1];
            string multipleStr = "";
            if (multiple.formula != null && multiple.formula != "")
            {
                if (multiple.nonMultiple)
                    multipleStr = ", X" + multiple.formula;
                else
                    multipleStr = ", /" + multiple.formula;
            }

            nameText += rangeStr + takenStr + multipleStr;           
        }

        return nameText;
    }  
}

public enum MainTogleType
{
    Location,
    Pass,
    Move
}

