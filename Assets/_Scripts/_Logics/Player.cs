using System;

public class Player : ICloneable
{
    public int locationID;
    public int passageID;
    public bool gameOver;
    public Quest quest;

    public object Clone()
    {
        var clone = (Player)MemberwiseClone();
        clone.quest = (Quest)quest.Clone();

        return clone;
    }

    public override string ToString()
    {
        return $"Player: locationID={locationID}, passageID={passageID}, quest={quest}";
    }
}