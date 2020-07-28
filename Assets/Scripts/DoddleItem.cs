using UnityEngine;

public class DoddleItem : MonoBehaviour
{
    public int x
    {
        get;
        private set;
    }

    public int y
    {
        get;
        private set;
    }

    public int id;

    public void OnItemPositionChanged(int newX, int newY)
    {
        x = newX;
        y = newY;
        gameObject.name = string.Format("[{0}][{1}]", x, y);
    }

    void OnMouseDown()
    {
        if (OnMouseOverItemEventHandler != null)
        {
            OnMouseOverItemEventHandler(this);
        }
    }

    public delegate void OnMouseOverItem(DoddleItem item);
    public static event OnMouseOverItem OnMouseOverItemEventHandler;
}
