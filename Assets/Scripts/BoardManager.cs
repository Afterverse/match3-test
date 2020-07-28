using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour
{
    public int height = 11;
    public int width = 7;

    public float tileSwapTime = 0.1f;

    private GameObject[] _tiles;
    private DoddleItem[,] _items;

    private DoddleItem _selectedItem;

    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {

        _items = new DoddleItem[width, height];

        //Loop along x axis.
        for (int x = 0; x < width; x++)
        {
            //Loop along y axis
            for (int y = 0; y < height; y++)
            {
                _items[x, y] = InstantiateDoddle(x, y);
            }
        }
    }

    DoddleItem InstantiateDoddle(int x, int y)
    {
        //Choose a random tile from our array of tile prefabs and prepare to instantiate it.
        GameObject toInstantiate = _tiles[Random.Range(0, _tiles.Length)];

        //Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop.
        DoddleItem newDoddle = ((GameObject)Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity)).GetComponent<DoddleItem>();

        //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
        newDoddle.transform.SetParent(this.transform);

        //Send change event to new doddle
        newDoddle.OnItemPositionChanged(x, y);

        return newDoddle;
    }

    void OnMouseOverItem(DoddleItem item)
    {
        if (_selectedItem == item)
        {
            Debug.Log("Selected same item");
            _selectedItem = null;
            return;
        }
        if (_selectedItem == null)
        {
            _selectedItem = item;
        }
        else
        {
            float xDiff = Mathf.Abs(item.x - _selectedItem.x);
            float yDiff = Mathf.Abs(item.y - _selectedItem.y);
            if (Mathf.Abs(xDiff - yDiff) == 1)
            {
                // We can swap items
                StartCoroutine(Swap(_selectedItem, item));
            }
            else
            {
                Debug.Log("LOL cannot swap these.");
            }
            _selectedItem = null;
        }
    }

    IEnumerator Swap(DoddleItem a, DoddleItem b)
    {
        SetPhysicsStatus(false);

        Vector3 aPos = a.transform.position; // We have to save this in order to move both simultaneously
        StartCoroutine(a.transform.Move(b.transform.position, 0.1f));
        StartCoroutine(b.transform.Move(aPos, 0.1f));

        yield return new WaitForSeconds(tileSwapTime);

        SwapIndices(a, b);
        SetPhysicsStatus(true);
    }

    void SwapIndices(DoddleItem a, DoddleItem b)
    {
        DoddleItem tempA = _items[a.x, a.y];
        _items[a.x, a.y] = b;
        _items[b.x, b.y] = tempA;
        int bOldX = b.x; int bOldY = b.y;
        b.OnItemPositionChanged(a.x, a.y);
        a.OnItemPositionChanged(bOldX, bOldY);
    }

    public void SetPhysicsStatus(bool status)
    {
        foreach (DoddleItem item in _items)
        {
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>();
            rb.isKinematic = !status;
        }
    }

    void LoadDoddles()
    {
        _tiles = Resources.LoadAll<GameObject>("Prefabs");
        Debug.Log(_tiles.Length);
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].GetComponent<DoddleItem>().id = i;
        }
    }

    public void SetupScene()
    {
        LoadDoddles();
        BoardSetup();
        DoddleItem.OnMouseOverItemEventHandler += OnMouseOverItem;
    }

    void Start()
    {
        SetupScene();
    }
    void OnDisable()
    {
        // Unsubscribe events
        DoddleItem.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }
}
