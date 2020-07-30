using UnityEngine;
using UnityEditor;

public class Item : MonoBehaviour
{
    public int x
    {
        get => _item.x;
        private set => _item.x = value;
    }

    public int y
    {
        get => _item.y;
        private set => _item.y = value;
    }

    private ItemBase _item;

    public int id;

    void Awake()
    {
        _item = new ItemBase();
    }

    public void SetPosition(int newX, int newY)
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

    void OnDrawGizmos()
    {
        Handles.Label(transform.position, string.Format("[{0}][{1}]\nID: {2}", x, y, id));
    }

    public ItemBase Copy()
    {
        ItemBase copy = _item.DeepCopy();
        return copy;
    }

    public delegate void OnMouseOverItem(Item item);
    public static event OnMouseOverItem OnMouseOverItemEventHandler;
}
