using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Z.Expressions;
using TMPro;

public class GamePanel : MonoBehaviour
{      
    public RectTransform questionsContent, paramsContent, questionsRect, paramsRect;
    public GameObject questionCellPref, parameterTextPref, victoryCell, defeatCell, textBg;
    public GameObject questionsNode, arrowsNode, startNode;
    public Button leftArrow, rightArrow, startButton;
    public AliveText mainText; 
    public RectTransform mainPictureRect;
    public AutoCanvasScaler autoCanvasScaler;
    public PictureNode pictureNode;

    [HideInInspector]
    public Player player;

    List<QuestionCell> questionCells = new List<QuestionCell>(); 
 
    int lastDescriptionIndex;

    string[] mainArray;
    int mainIndex;

    public static GamePanel Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        //todo в версии реальной игры сделать обязательно загрузку квеста из файла и при рестарте игры также загрузка должна быть из файла, чтобы обнулить все значения в квесте!                                        

        mainPictureRect.sizeDelta = new Vector2(mainPictureRect.sizeDelta.x, mainPictureRect.rect.width / autoCanvasScaler.scaleFactor);
        paramsRect.sizeDelta = new Vector2(paramsRect.sizeDelta.x, paramsRect.rect.width / 3f / autoCanvasScaler.scaleFactor);

        float questionH = (Screen.safeArea.height - paramsRect.sizeDelta.y * autoCanvasScaler.scaleFactor - mainPictureRect.sizeDelta.y * autoCanvasScaler.scaleFactor) / autoCanvasScaler.scaleFactor;
        questionsRect.sizeDelta = new Vector2(questionsRect.sizeDelta.x, questionH);       

        if(SaveLoadManager.Manager.loadedPlayer == null)
        {
            RestartQuest();
        }
        else
        {          
            player = SaveLoadManager.Manager.loadedPlayer;
            ActionStart();
        }     
    }    

    public void RestartQuest()
    {
        startButton.interactable = true;

        pictureNode.StartPictures();
        startNode.SetActive(true);

        textBg.SetActive(false);
        arrowsNode.SetActive(false);
        victoryCell.SetActive(false);
        defeatCell.SetActive(false);
        paramsRect.gameObject.SetActive(false);

        player = new Player
        {
            locationID = Quest.Instance.FindStartLocation().id,
            quest = (Quest)Quest.Instance.Clone()
        };

        foreach (Location location in player.quest.locations)
            location.visitCounter = 0;

        foreach (Passage passage in player.quest.passages)
        {
            passage.visitCounter = 0;
            passage.FindControversials();
        }

        foreach (Parameter parameter in player.quest.parameters)
            parameter.value = parameter.startValue;
    }

    private void ShowCurrentLocation()
    {       
        Location location = player.quest.FindLocationWith(player.locationID);             

        //влияние на параметры
        InfluenceOnParameters(location);

        //показ параметров
        ParameterDemonstration(location);

        if (player.gameOver) return;       

        //описания
        if (location.descriptions.Count == 1 || location.locationType == LocationType.Empty)
        {
            GutMainText(ParseText(location.descriptions[0]));           
        }
        else
        {           
            if (location.chooseWithFormula)
            {
                int index = 0;
                if (string.IsNullOrEmpty(location.formula))
                {
                    index = Random.Range(0, location.descriptions.Count);                   
                }
                else
                {
                    try { index = Eval.Execute<int>(location.formula, FillFormulaDict()); }
                    catch { Director.Instance.WarningWithText("Неправильная формула выбора описаний!"); }

                    index -= 1;

                    if (index < 0 || index > location.descriptions.Count - 1)
                        index = 0;
                }            
                
                lastDescriptionIndex = index;
                GutMainText(ParseText(location.descriptions[index]));                
            }
            else
            {              
                lastDescriptionIndex = location.visitCounter % location.descriptions.Count;
                GutMainText(ParseText(location.descriptions[lastDescriptionIndex]));                
            }
        }
       
        //переходы
        foreach (Transform tr in questionsContent)        
            Destroy(tr.gameObject);

        questionCells.Clear();

        List<Passage> passages = player.quest.FindAllPassagesFromLocation(location.id);
        List<Passage> workPassages = new List<Passage>();

        //переходы с вероятностью
        List<Passage> toDeleteV = new List<Passage>();
        foreach (Passage p in passages)
        {
            if (p.priority < 1)
            {                          
                if (Random.Range(0, 100) <= p.priority * 100)
                    workPassages.Add(p);

                toDeleteV.Add(p);
            }
        }

        foreach (Passage p in toDeleteV)
            passages.Remove(p);

        //работа со спорными переходами        
        for (int n = passages.Count - 1; n >= 0; n--)
        {           
            try
            {
                Passage passage = passages[n];

                if (workPassages.Contains(passage))
                    continue;

                if (passage.controversials.Count == 0 && passage.priority >= 1)
                {                            
                    workPassages.Add(passage);                    
                }                   
                else 
                {
                    List<Passage> allContra = new List<Passage>(passage.controversials);
                    allContra.Add(passage);                 

                    List<Vector2> segmens = new List<Vector2>();

                    int last = 0;
                    foreach (Passage contra in allContra)
                    {                        
                        Vector2 segment = new Vector2(last, last + (int)contra.priority - 1);
                        last += (int)contra.priority;
                        segmens.Add(segment);                      
                    }

                    int rnd = Random.Range(0, last);                    

                    int i = 0;
                    foreach (Vector2 segment in segmens)
                        if (rnd >= segment.x && rnd <= segment.y)
                        {
                            i = segmens.IndexOf(segment);
                            break;
                        }

                    workPassages.Add(allContra[i]);

                    List<Passage> toDelete = new List<Passage>();
                    foreach (Passage contra in allContra)
                        foreach (Passage p in passages)
                            if (p.id == contra.id)
                            {
                                toDelete.Add(p);
                                break;
                            }

                    foreach (Passage p in toDelete)
                        passages.Remove(p);
                }
            }
            catch { }                   
        }

        workPassages.Sort();

        List<PassageInfo> visiblePassages = new List<PassageInfo>();
       
        foreach (Passage passage in workPassages)
        {            
            Location toLoc = player.quest.FindLocationWith(passage.to);

            //проходимость
            bool passCondition = (passage.passability == 0 || passage.visitCounter < passage.passability) && (toLoc.passability == 0 || toLoc.visitCounter < toLoc.passability);

            //выполнение условий для показа перехода
            bool logicalCondition = true;
            bool inRange = true;
            bool takesOrNotValues = true;
            bool multipleOrNotValues = true;
            if (passage.logicalCondition != null && passage.logicalCondition != "")
            {
                try { logicalCondition = Eval.Execute<bool>(passage.logicalCondition, FillFormulaDict()); }
                catch { Director.Instance.WarningWithText("Неправильная формула логического условия!"); }               
            }
            
            for (int i = 0; i < player.quest.parameters.Count; i++)
            {
                Parameter parameter = player.quest.parameters[i];
                NecessaryRange range = passage.necessaryRanges[i];
                if (range.isOn && (parameter.value < range.min || parameter.value > range.max))
                {
                    inRange = false;
                    break;
                }

                TakenValues takenValues = passage.takenValues[i];
                if(takenValues.formula != null && takenValues.formula != "")
                {                 
                    try
                    {
                        int val = Eval.Execute<int>(takenValues.formula, FillFormulaDict());
                        if(takenValues.nonTaken)
                            takesOrNotValues = parameter.value != val;
                        else
                            takesOrNotValues = parameter.value == val;                        
                    }
                    catch { Director.Instance.WarningWithText("Неправильная формула принятия значений!"); }
                }

                MultipleValues multipleValues = passage.multipleValues[i];
                if (multipleValues.formula != null && multipleValues.formula != "")
                {
                    try
                    {
                        int val = Eval.Execute<int>(multipleValues.formula, FillFormulaDict());
                        if (multipleValues.nonMultiple)
                            multipleOrNotValues = parameter.value % val != 0;
                        else
                            multipleOrNotValues = parameter.value % val == 0;
                    }
                    catch { Director.Instance.WarningWithText("Неправильная формула кратности значениям!"); }
                }
            }

            bool allConditions = passCondition && logicalCondition && inRange && takesOrNotValues && multipleOrNotValues;

            if (allConditions || passage.alwaysShow)            
                visiblePassages.Add(new PassageInfo { pass = passage, isAllConditions = allConditions });                                                
        }

        const float interval = 120;
        int m = 0;
        foreach (PassageInfo info in visiblePassages)
        {
            GameObject obj = Instantiate(questionCellPref, questionsContent);

            QuestionCell cell = obj.GetComponent<QuestionCell>();
            cell.StartWith(this, info.pass, m * 0.15f);          

            RectTransform cellRect = obj.GetComponent<RectTransform>();

            float Y = (visiblePassages.Count - 1) * interval / 2 - interval * m; //центровка

            cellRect.anchoredPosition = new Vector2(60, Y);

            if (!info.isAllConditions && info.pass.alwaysShow)
                cell.DisableButton();

            questionCells.Add(cell);
            m++;
        }

        RectTransform viewPort = (RectTransform)questionsContent.parent;
        questionsContent.sizeDelta = new Vector2(questionsContent.sizeDelta.x, Mathf.Max(viewPort.rect.height, m * interval));

        //проверка на победу, покажение, наличие переходов
        if (location.locationType == LocationType.Victory)        
            FinalWithText("Квест пройден!");                
        else if (location.locationType == LocationType.Fail)        
            FinalWithText("Квест провален!");                 
        else if (questionCells.Count == 0)
            Director.Instance.WarningWithText("Ошибка, нет доступных переходов!");              

        location.visitCounter++;
    }

    private void GutMainText(string text)
    {
        List<string> list = GetBetween(text, "<im", "im>");
        string imageString = "";
        foreach(string str in list)
        {            
            text = text.Replace(str, "");
            imageString = str.Replace("<im", "");
            imageString = imageString.Replace("im>", "");
            imageString = imageString.Replace(" ", "");
            //imageString += ".png"; 
        }

        text = text.Replace(System.Environment.NewLine, "");

        pictureNode.SetNewPicture(imageString);       

        mainIndex = 0;
        mainArray = text.Split('&');
        mainText.SetText(mainArray[mainIndex]);       

        if (mainArray.Length > 1)
        {
            arrowsNode.SetActive(true);
            questionsNode.SetActive(false);            
            leftArrow.interactable = false;            
            StartCoroutine(TurnOnButton(rightArrow, 0.35f));            
        }
        else
        {
            arrowsNode.SetActive(false);
            questionsNode.SetActive(true);           
        }
    }

    public void ActionNext()
    {      
        mainIndex++;

        if (mainIndex < mainArray.Length)
            mainText.SetText(CleanText(mainArray[mainIndex]));

        if (mainIndex >= mainArray.Length - 1)
        {
            questionsNode.SetActive(true);
            rightArrow.interactable = false;
        }
        else
        {
            rightArrow.interactable = false;            
            StartCoroutine(TurnOnButton(rightArrow, 0.2f));
        }

        leftArrow.interactable = false;
        StartCoroutine(TurnOnButton(leftArrow, 0.2f));
    } 

    public void ActionPreviuos()
    {              
        mainIndex--;
        if (mainIndex >= 0)
            mainText.SetText(CleanText(mainArray[mainIndex]));

        questionsNode.SetActive(false);

        if (mainIndex == 0)
            leftArrow.interactable = false;
        else
        {            
            leftArrow.interactable = false;
            StartCoroutine(TurnOnButton(leftArrow, 0.2f));            
        }

        rightArrow.interactable = false;
        StartCoroutine(TurnOnButton(rightArrow, 0.2f));
    }

    IEnumerator TurnOnButton(Button button, float delay)
    {       
        yield return new WaitForSeconds(delay);
        button.interactable = true;
    }

    private string CleanText(string text)
    {
        int spaces = 0;
        for (int i = 0; i < text.Length; i++)
        {
            if (text.Substring(i, 1) == " ")
                spaces++;
            else break;
        }

        if (spaces > 0)
            text = text.Remove(0, spaces);
        return text;
    }

    public void ShowPassage(Passage passage)
    {
        leftArrow.interactable = false;
        rightArrow.interactable = false;

        //влияние на параметры
        InfluenceOnParameters(passage);

        //показ параметров
        if (!passage.ignoreDemonstration)
            ParameterDemonstration(passage);

        if (player.gameOver) return;

        passage.visitCounter++;       

        Location location = player.quest.FindLocationWith(player.locationID);

        //если переход без описания, то покахываем сразу локацию
        if (passage.description == null || passage.description == "" || location.locationType == LocationType.Empty)
        {
            //работа с пустой локацией
            if (location.locationType == LocationType.Empty && passage.description != null && passage.description != "")                           
                location.descriptions[0] = passage.description;                       

            ShowCurrentLocation();
        }
        else // показ описания перехода и кнопку "далее"
        {
            GutMainText(ParseText(passage.description));

            foreach (Transform tr in questionsContent)
                Destroy(tr.gameObject);

            Passage next = new Passage
            {
                to = passage.to,
                question = "Далее",
                ignoreDemonstration = true
            };

            GameObject obj = Instantiate(questionCellPref, questionsContent);

            QuestionCell cell = obj.GetComponent<QuestionCell>();
            cell.StartWith(this, next);

            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(60, -6);            
        }       
    }

    private void InfluenceWithFormula(string formula, Parameter parameter)
    {
        if (string.IsNullOrEmpty(formula))
        {
            Director.Instance.WarningWithText($"Пустая формула влияния! Параметр: p{parameter.index}");
            return;
        }

        try
        {
            if(formula.Contains("rnd"))
            {
                List<string> list = GetBetween(formula, "(", ")");
                foreach (string str in list)
                {
                    formula = str.Replace("(", "");
                    formula = formula.Replace(")", "");                          
                }

                string[] ints = formula.Split(',');
                if (ints.Length > 1 && int.TryParse(ints[0], out int v1) && int.TryParse(ints[1], out int v2))                
                    parameter.value = Random.Range(v1, v2);                                   
            }
            else
            {
                int value = Eval.Execute<int>(formula, FillFormulaDict());
                parameter.value = Mathf.Max(Mathf.Min(value, parameter.maxValue), parameter.minValue);
            }     
        }
        catch { Director.Instance.WarningWithText($"Неправильная формула влияния! Параметр: p{parameter.index}"); }
    }

    private Dictionary<string, object> FillFormulaDict()
    {
        Dictionary<string, object> paramsDict = new Dictionary<string, object>();
        foreach (Parameter p in player.quest.parameters)
            paramsDict.Add("p" + p.index, p.value);
        return paramsDict;
    }
     
    public void ParameterDemonstration(Unit unit)
    {       
        for (int j = 0; j < player.quest.parameters.Count; j++)
        {
            Parameter parameter = player.quest.parameters[j];
            ParamsAction action = unit.paramsActions[j];
            switch (action)
            {
                case ParamsAction.Hide: parameter.isHidden = true; break;
                case ParamsAction.Show: parameter.isHidden = false; break;
            }
        }

        foreach (Transform tr in paramsContent)
            Destroy(tr.gameObject);

        List<Parameter> visibleParams = new List<Parameter>();
       
        foreach (Parameter parameter in player.quest.parameters)
        {
            if (parameter.isActive && !parameter.isHidden)
            {              
                ParamsRange range = parameter.FindCorrectRange();
                if (range != null)
                {
                    string output = range.output;
                    if (!string.IsNullOrEmpty(output))                    
                        visibleParams.Add(parameter);                                                      
                }               
            }
        }

        const float interval = 70f;
        int m = 0;
        foreach (Parameter parameter in visibleParams)
        {
            ParamsRange range = parameter.FindCorrectRange();

            if (range != null)
            {
                string output = range.output;
                output = output.Replace("<>", parameter.value.ToString());
                output = ParseText(output, ignoreColor: true);

                for (int i = 0; i < player.quest.parameters.Count; i++)
                {
                    Parameter p = player.quest.parameters[i];
                    string key = $"p{i + 1}";
                    output = output.Replace(key, p.value.ToString());
                }

                float Y = -(visibleParams.Count - 1) * interval / 2 + interval * m; //центровка
                
                GameObject cell = Instantiate(parameterTextPref, paramsContent);
                cell.GetComponent<RectTransform>().anchoredPosition = new Vector2(20, Y);
                cell.GetComponent<TMP_Text>().text = output;
            }

            m++;
        }
        RectTransform viewPort = (RectTransform)paramsContent.parent;      
        paramsContent.sizeDelta = new Vector2(paramsContent.sizeDelta.x, Mathf.Max(viewPort.rect.height, m * interval));        
    }

    private void InfluenceOnParameters(Unit unit)
    {
        for (int j = 0; j < unit.influences.Count; j++)
        {
            Parameter parameter = player.quest.parameters[j];

            if (parameter.isActive)
            {
                Influence influence = unit.influences[j];
                switch (influence.influenceType)
                {
                    case InfluenceType.Units:
                        parameter.value = Mathf.Max(Mathf.Min(parameter.value + influence.value, parameter.maxValue), parameter.minValue);
                        break;
                    case InfluenceType.Percent:
                        parameter.value = (int)Mathf.Max(Mathf.Min(parameter.value * (influence.value / 100f + 1), parameter.maxValue), parameter.minValue);
                        break;
                    case InfluenceType.Value:
                        parameter.value = Mathf.Max(Mathf.Min(influence.value, parameter.maxValue), parameter.minValue);
                        break;
                    case InfluenceType.Formula:
                        InfluenceWithFormula(influence.formula, parameter);
                        break;
                }

                //print("parameter: " + parameter);

                if (parameter.paramType != ParamType.Usual &&
                    ((parameter.isCriticMax && parameter.value >= parameter.maxValue) || (!parameter.isCriticMax && parameter.value <= parameter.minValue)))
                {                  
                    player.gameOver = true;
                    GutMainText(ParseText(parameter.criticText));

                     foreach (Transform tr in questionsContent)
                         Destroy(tr.gameObject);

                     switch (parameter.paramType)
                     {
                         case ParamType.Successful: victoryCell.SetActive(true); break;
                         case ParamType.Failed: defeatCell.SetActive(true); break;
                     }
                }
            }
        }
    }

    private void FinalWithText(string text)
    {
        Director.Instance.WarningWithText(text);
        player.gameOver = true;
        foreach (Transform tr in questionsContent)
            Destroy(tr.gameObject);
    }

    public string ParseText(string text, bool ignoreColor = false)
    {     
        string t = $"{text}";
        List<string> list = GetBetween(t, "{", "}");

        foreach(string str in list)
        {            
            string formula = str.Replace("{", "");
            formula = formula.Replace("}", "");
          
            try
            {
                int val = Eval.Execute<int>(formula, FillFormulaDict());

                if (ignoreColor)
                    t = t.Replace(str, val.ToString());
                else
                    t = t.Replace(str, "<color=#6BBEFF>" + val.ToString() + "</color>");
            }
            catch { Director.Instance.WarningWithText("Неправильная формула подстановки!"); }
        }
        return t;
    }

    public List<string> GetBetween(string strSource, string strStart, string strEnd)
    {
        List<string> list = new List<string>();

        while (strSource.Contains(strStart) && strSource.Contains(strEnd))
        {
            int start = strSource.IndexOf(strStart, 0, System.StringComparison.Ordinal);
            int end = strSource.IndexOf(strEnd, start, System.StringComparison.Ordinal) + strEnd.Length;
            string content = strSource.Substring(start, end - start);
            list.Add(content);
            strSource = strSource.Replace(content, "");            
        }

        return list;
    }  

    public void ActionFinal(bool victory)
    {
        if(victory)        
            Director.Instance.WarningWithText("Квест пройден!");        
        else        
            Director.Instance.WarningWithText("Квест провален!");

        victoryCell.SetActive(false);
        defeatCell.SetActive(false);
    }

    public void ActionStart()
    {
        rightArrow.interactable = false;        

        startButton.interactable = false;
        paramsRect.gameObject.SetActive(true);
        startNode.SetActive(false);
        textBg.SetActive(true);
        arrowsNode.SetActive(true);
        ShowCurrentLocation();
    }
}

public struct PassageInfo
{
    public Passage pass;
    public bool isAllConditions;
}
