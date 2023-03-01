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
}
