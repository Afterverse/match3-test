using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public int height = 5; // Board height
    public int width = 5; // Board width

    public int minMatch = 3; // Minimum items in line to be a valid match

    public float itemSwapTime = 0.1f;
    public float delayBetweenMatches = 0.2f;

    public float gravity = 9.8f;

    // Reference for loading our tile prefabs
    private GameObject[] _tiles;

    // Reference to store which item is on each board position
    private Item[,] _items;

    // Store a reference to the transform of our Board object.
    private Transform boardHolder;

    private Item _selectedItem; // Player's current selected item

    public bool canPlay = true;

    private bool updating = true;

    void BoardSetup()
    {

        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;

        _items = new Item[width, height];

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

    void RepositionItems()
    {
        for (int x = 0; x < width; x++)
        {
            //Loop along y axis
            for (int y = 0; y < height; y++)
            {
                Item current = _items[x, y];
                current.transform.position = new Vector3(current.x, current.y, 0f);
            }
        }
    }


    // Shuffle board using Fisher-Yates algorithm
    public void ShuffleBoard(System.Random random)
    {
        // Get 2d array dimensions
        int num_rows = _items.GetUpperBound(0) + 1;
        int num_cols = _items.GetUpperBound(1) + 1;
        int num_cells = num_rows * num_cols;

        // Randomize the array.
        for (int i = 0; i < num_cells - 1; i++)
        {
            // Pick a random cell between i and the end of the array
            int j = random.Next(i, num_cells);

            // Convert to row/column indexes
            int row_i = i / num_cols;
            int col_i = i % num_cols;
            int row_j = j / num_cols;
            int col_j = j % num_cols;

            // Swap cells i and j
            Item temp = _items[row_i, col_i];
            _items[row_i, col_i] = _items[row_j, col_j];
            _items[row_j, col_j] = temp;
            SwapIndices(_items[row_i, col_i], _items[row_j, col_j]);
            RepositionItems();
            // Swap(_items[row_i, col_i], _items[row_j, col_j]);
        }
    }

    Item InstantiateDoddle(int x, int y, int offsetX = 0, int offsetY = 0)
    {
        //Choose a random tile from our _items of tile prefabs and prepare to instantiate it.
        GameObject toInstantiate = _tiles[Random.Range(0, _tiles.Length)];

        //Instantiate the GameObject instance using the prefab chosen for to Instantiate at the Vector3 corresponding to current grid position in loop.
        Item newDoddle = ((GameObject)Instantiate(toInstantiate, new Vector3(x + offsetX, y + offsetY, 0f), Quaternion.identity)).GetComponent<Item>();

        //Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
        newDoddle.transform.SetParent(boardHolder);

        // Set current item position in board
        newDoddle.SetPosition(x, y);

        return newDoddle;
    }

    void LoadTiles()
    {
        _tiles = Resources.LoadAll<GameObject>("Prefabs");
        for (int i = 0; i < _tiles.Length; i++)
        {
            _tiles[i].GetComponent<Item>().id = i; // Set an UID for the tile
            Debug.Log(string.Format("Loaded game object {0} with id {1}", _tiles[i].name, i));
        }
        Debug.Log(string.Format("Loaded {0} objects", _tiles.Length));
    }

    void SweepBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                MatchInfo match = GetMatch(_items[i, j]);
                if (match.valid) // We have a match, change this item
                {
                    Debug.Log("Found a match on game init, changing tile");
                    Destroy(_items[i, j].gameObject);
                    _items[i, j] = InstantiateDoddle(i, j);
                    j--; // Recheck this position
                }
            }
        }
    }

    public void Init()
    {
        LoadTiles(); // TODO make this a global referenced Scriptable Object
        BoardSetup();
        SweepBoard();

        // Register a callback to be called everytime player selects an item
        Item.OnMouseOverItemEventHandler += OnMouseOverItem;
    }

    MatchInfo GetMatch(Item item)
    {
        MatchInfo m = null;
        List<Item> horizontalMatch = GetMatchHorizontal(item);
        List<Item> verticalMatch = GetMatchVertical(item);

        bool shouldPreferenceHorizontal = horizontalMatch.Count >= verticalMatch.Count; // Is the horizontal match higher than the vertical?

        if (shouldPreferenceHorizontal)
        {
            m = new MatchInfo(horizontalMatch);
        }
        else
        {
            m = new MatchInfo(verticalMatch);
        }
        return m;
    }

    List<Item> GetMatchHorizontal(Item item)
    {
        List<Item> matched = new List<Item> { item };
        int leftItem = item.x - 1;  // Imediatelly left item
        int rightItem = item.x + 1; // Imediatelly right item
        while (leftItem >= 0 && _items[leftItem, item.y].id == item.id)
        {
            matched.Add(_items[leftItem, item.y]);
            leftItem--;
        }
        while (rightItem < width && _items[rightItem, item.y].id == item.id)
        {
            matched.Add(_items[rightItem, item.y]);
            rightItem++;
        }
        return matched;
    }

    List<Item> GetMatchVertical(Item item)
    {
        List<Item> matched = new List<Item> { item };
        int lowerItem = item.y - 1; // Imediatelly lower item
        int upperItem = item.y + 1; // Imediatelly upper item
        while (lowerItem >= 0 && _items[item.x, lowerItem].id == item.id)
        {
            matched.Add(_items[item.x, lowerItem]);
            lowerItem--;
        }
        while (upperItem < height && _items[item.x, upperItem].id == item.id)
        {
            matched.Add(_items[item.x, upperItem]);
            upperItem++;
        }
        return matched;
    }

    void OnMouseOverItem(Item item)
    {
        if (_selectedItem == item || !canPlay)
        {
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
                // Try to swap items
                StartCoroutine(TryMatch(_selectedItem, item));
            }
            else
            {
                Debug.Log("This move is forbidden.");
            }
            _selectedItem = null;
        }
    }

    IEnumerator TryMatch(Item a, Item b)
    {
        canPlay = false;

        SwapIndices(a, b);
        yield return StartCoroutine(Swap(a, b));

        MatchInfo matchA = GetMatch(a);
        MatchInfo matchB = GetMatch(b);

        if (!matchA.valid && !matchB.valid)
        {
            // Swap not resulted in a valid match, undo swap
            Debug.Log("Swap not valid");
            SwapIndices(a, b);
            yield return StartCoroutine(Swap(a, b));
            canPlay = true;
            yield break;
        }

        if (matchA.valid)
        {
            StartCoroutine(DestroyMatch(matchA.match));
            yield return StartCoroutine(UpdateBoardIndices(matchA));
            DestroyMatchObjects(matchA.match);
            yield return new WaitForSeconds(delayBetweenMatches);
        }
        else if (matchB.valid)
        {
            StartCoroutine(DestroyMatch(matchB.match));
            yield return StartCoroutine(UpdateBoardIndices(matchB));
            DestroyMatchObjects(matchB.match);
            yield return new WaitForSeconds(delayBetweenMatches);
        }

        yield return StartCoroutine(CheckForMatches());

        canPlay = true;
    }

    void SwapIndices(Item a, Item b)
    {
        int tempX = a.x;
        int tempY = a.y;
        UpdateItemPositions(a, b.x, b.y);
        UpdateItemPositions(b, tempX, tempY);
    }

    IEnumerator Swap(Item a, Item b)
    {
        StartCoroutine(a.transform.Move(b.transform.position, itemSwapTime));
        StartCoroutine(b.transform.Move(a.transform.position, itemSwapTime));
        yield return new WaitForSeconds(itemSwapTime);
    }

    IEnumerator DestroyMatch(List<Item> items)
    {
        foreach (var item in items)
        {
            StartCoroutine(item.transform.Scale(Vector3.zero, 0.1f));
        }
        yield return null;
    }

    void DestroyMatchObjects(List<Item> items)
    {
        foreach (var item in items)
        {
            Destroy(item.gameObject);
        }
    }

    void UpdateItemPositions(Item item, int x, int y)
    {
        _items[x, y] = item;
        item.SetPosition(x, y);
    }

    IEnumerator UpdateBoardIndices(MatchInfo match)
    {
        int minX = match.GetMinX();
        int maxX = match.GetMaxX();
        int minY = match.GetMinY();
        int maxY = match.GetMaxY();

        List<Item> fallingItems = new List<Item> { };

        if (minY == maxY) // We have to update several columns
        {
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j < height - 1; j++)
                {
                    Item upperIndex = _items[i, j + 1];
                    Item current = _items[i, j];
                    _items[i, j] = upperIndex;
                    _items[i, j + 1] = current;
                    _items[i, j].SetPosition(_items[i, j].x, _items[i, j].y - 1);
                    fallingItems.Add(_items[i, j]);
                }
                _items[i, height - 1] = InstantiateDoddle(i, height - 1, 0, 1);
                Item newItem = _items[i, height - 1];
                fallingItems.Add(newItem);
            }
        }
        else if (minX == maxX) // We have to update one column
        {
            int matchHeight = (maxY - minY) + 1;
            int currentX = minX;
            for (int j = minY + matchHeight; j <= height - 1; j++)
            {
                Item lowerIndex = _items[currentX, j - matchHeight];
                Item current = _items[currentX, j];
                _items[currentX, j - matchHeight] = current;
                _items[currentX, j] = lowerIndex;
            }

            for (int y = 0; y < height - matchHeight; y++)
            {
                _items[currentX, y].SetPosition(currentX, y);
                fallingItems.Add(_items[currentX, y]);
            }
            for (int i = 0; i < match.Count; i++)
            {
                _items[currentX, (height - 1) - i] = InstantiateDoddle(currentX, (height - 1) - i, 0, match.Count);
                Item intantiated = _items[currentX, (height - 1) - i];
                fallingItems.Add(intantiated);
            }
        }

        yield return StartCoroutine(FallItems(fallingItems)); // Fall all items and waits for finish

        CheckForMatches();
        if (!CheckForPossibleMoves())
        {
            Debug.Log("Should shuffle board");
            ShuffleBoard(new System.Random());
        };

        yield return null;
    }

    IEnumerator FallItems(List<Item> items)
    {
        foreach (var item in items)
        {
            StartCoroutine(item.Fall(new Vector3(item.x, item.y, 0.0f), gravity));
        }
        bool hasFallingItems = true;
        while (hasFallingItems)
        {
            yield return null;
            hasFallingItems = false;
            foreach (var item in items)
            {
                hasFallingItems = hasFallingItems || item.isFalling;
            }
            Debug.Log(hasFallingItems);
        }
        Debug.Log("Finished falling");
        yield return null;
    }

    IEnumerator CheckForMatches()
    {
        //Loop along x axis
        for (int x = 0; x < width; x++)
        {
            //Loop along y axis
            for (int y = 0; y < height; y++)
            {
                MatchInfo matchInfo = GetMatch(_items[x, y]);
                if (matchInfo.valid)
                {
                    StartCoroutine(DestroyMatch(matchInfo.match));
                    yield return StartCoroutine(UpdateBoardIndices(matchInfo));
                    DestroyMatchObjects(matchInfo.match);

                    yield return new WaitForSeconds(delayBetweenMatches);
                }
            }
        }
    }

    bool CheckForPossibleMoves()
    {
        MatchInfo matchA;
        MatchInfo matchB;
        List<List<Item>> possibleSwaps = new List<List<Item>>();
        for (int x = 0; x < width - 1; x++)
        {
            //Loop along y axis
            for (int y = 0; y < height - 1; y++)
            {
                Item current = _items[x, y];
                Item upperItem = _items[x, y + 1];
                Item rightItem = _items[x + 1, y];

                SwapIndices(current, rightItem);
                matchA = GetMatch(current);
                matchB = GetMatch(rightItem);
                if (matchA.valid || matchB.valid)
                {
                    possibleSwaps.Add(new List<Item> { current, rightItem });
                }
                SwapIndices(current, rightItem); // Swap back as we dont want the actual swap

                SwapIndices(current, upperItem);
                matchA = GetMatch(current);
                matchB = GetMatch(upperItem);
                if (matchA.valid || matchB.valid)
                {
                    possibleSwaps.Add(new List<Item> { current, upperItem });
                }
                SwapIndices(current, upperItem); // Swap back as we dont want the actual swap
            }
        }
        Debug.Log(possibleSwaps.Count);
        return possibleSwaps.Count > 0;
    }
    void OnDisable()
    {
        Item.OnMouseOverItemEventHandler -= OnMouseOverItem;
    }

}
