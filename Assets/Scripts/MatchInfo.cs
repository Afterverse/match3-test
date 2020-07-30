using UnityEngine;
using System.Collections.Generic;

public class MatchInfo
{
    public int matchStart;
    public int matchEnd;

    private List<Item> _match = null;
    public List<Item> match
    {
        get { return _match; }
        set { _match = value; }
    }

    public bool valid
    {
        get { return _match != null && _match.Count >= 3; }
    }

    public int Count
    {
        get { return _match.Count; }
    }

    public MatchInfo(List<Item> match)
    {
        _match = match;
    }

    public int GetMinX()
    {
        int[] indices = new int[_match.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = _match[i].x;
        }
        return (int)Mathf.Min(indices);
    }

    public int GetMaxX()
    {
        int[] indices = new int[_match.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = _match[i].x;
        }
        return (int)Mathf.Max(indices);
    }

    public int GetMinY()
    {
        int[] indices = new int[_match.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = _match[i].y;
        }
        return (int)Mathf.Min(indices);
    }

    public int GetMaxY()
    {
        int[] indices = new int[_match.Count];
        for (int i = 0; i < indices.Length; i++)
        {
            indices[i] = _match[i].y;
        }
        return (int)Mathf.Max(indices);
    }
}
