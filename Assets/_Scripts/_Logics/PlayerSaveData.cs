using System.Collections.Generic;

public class PlayerSaveData
{
    public int locationID;
    public int passageID;
    public bool gameOver;

    public List<int> parameterValues = new List<int>();
    public List<bool> parameterHidden = new List<bool>();

    public Dictionary<int, int> locationVisitCounters = new Dictionary<int, int>();
    public Dictionary<int, int> passageVisitCounters = new Dictionary<int, int>();
}