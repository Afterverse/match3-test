using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;	//Static instance of GameManager which allows it to be accessed by any other script.

    public float levelStartDelay = 2f;  //Time to wait before starting a new level, in seconds.

    private BoardManager boardScript;  // Store a reference to our BoardManager which will set up the level.
    private int level = 1;  //  Current level number.
    private bool doingSetup = true; // Boolean to check if we're setting and updating up board, prevent various actions during setup.

    private Item _selectedItem = null; // Reference to selected item

    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    // This is called only once, and the paramter tell it to be called only after the scene was loaded
    // (otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        instance.level++;
        instance.InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //While doingSetup is true the player can't move, prevent player from moving while title card is up.
        doingSetup = true;

        //Call the Init function of the BoardManager script.
        boardScript.Init();

    }

    //Update is called every frame.
    void Update()
    {
        if (doingSetup)
            return;
    }

    //GameOver is called when the timer reaches 0
    public void GameOver()
    {
        //Disable this GameManager.
        enabled = false;
    }
}
