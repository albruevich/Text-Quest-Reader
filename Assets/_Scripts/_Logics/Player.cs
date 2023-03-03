using UnityEngine;
using System.Collections.Generic;
using System;

public class Player : ICloneable
{
    public int locationID;
    public int passageID;
    public bool gameOver;
    public Quest quest;

    public object Clone()
    {
        Player clone = (Player)MemberwiseClone();
        clone.quest = (Quest)quest.Clone();

        return clone;
    }

    public override string ToString()
    {
        return string.Format("Player: locationID={0}, passageID={1}, quest={2}", locationID, passageID, quest);
    }
}
