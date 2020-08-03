using System.Collections;
using UnityEngine;
using UnityEditor;

public class Item : MonoBehaviour
{
    public int x
    {
        get;
        set;
    }

    public int y
    {
        get;
        set;
    }

    public int id;

    public bool isFalling
    {
        get;
        set;
    }

    public void SetPosition(int newX, int newY)
    {
        x = newX;
        y = newY;
        gameObject.name = string.Format("[{0}][{1}]", x, y);
    }

    public IEnumerator Fall(Vector3 target, float gravity)
    {
        isFalling = true;
        Vector3 velocity = Vector3.zero;

        while (transform.position.y > target.y)
        {
            yield return null;
            if (transform == null) { yield break; };
            velocity = velocity + new Vector3(0f, -gravity, 0f) * Time.deltaTime;
            transform.position = transform.position + velocity * Time.deltaTime;
        }

        // Make sure that it snaps to the end position.
        transform.position = target;
        isFalling = false;
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

    public delegate void OnMouseOverItem(Item item);
    public static event OnMouseOverItem OnMouseOverItemEventHandler;
}
