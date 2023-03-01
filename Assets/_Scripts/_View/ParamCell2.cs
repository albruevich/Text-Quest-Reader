using UnityEngine;
using UnityEngine.UI;

public class ParamCell2 : MonoBehaviour
{
    public Text indexText, nameText;
    public GameObject selectImage;
    public GameObject checkmark;

    public int index;    

    [HideInInspector]
    public Parameter parameter;

    public delegate void CellSelected(ParamCell2 cell);
    public CellSelected cellSelected;     

    public void StartWith(Parameter parameter, Unit unit, int index)
    {        
        this.parameter = parameter;
        this.index = index;           

        indexText.text = "p" + index;

        if (parameter != null)
        {
            UpdateText(unit);          
            checkmark.SetActive(parameter.isActive);           
        }
    }
    
    public void UpdateText(Unit unit)
    {
        nameText.text = parameter.workingName + Director.GetTextForUnit(unit, index);        
    }

    public void ActionSelect()
    {
        selectImage.SetActive(true);

        indexText.color = new Color(1, 1, 1);
        nameText.color = new Color(1, 1, 1);

        cellSelected(this);       
    }   

    public void Diselect()
    {
        selectImage.SetActive(false);

        indexText.color = new Color(0, 0, 0);
        nameText.color = new Color(0, 0, 0);       
    }
}
