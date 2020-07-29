using UnityEngine;
using System.Collections.Generic;

public class Match
{
    public List<Item> match = null;
    public int matchStartX;
    public int matchEndX;
    public int matchStartY;
    public int matchEndY;

    public bool isValidMatch
    {
        get { return match != null; }
    }
}
