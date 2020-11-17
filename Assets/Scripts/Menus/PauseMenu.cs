using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    #region References and bools
    //Don't attach to the pause menu UI itself, this would end up with the script being inactive and no longer able to detect user input
    [SerializeField]
    [Tooltip("Reference Pause Menu Game Object, to toggle active on/off")]
    private GameObject[] pauseMenuUI;
    [SerializeField, Tooltip("Reference to player to see if alive")]
    private Player player;

    [Header("Bool Conditionals")]
    [Tooltip("is the game currently paused.")]
    public static bool gameIsPaused = false; //Was private, made public static for Inventory I to open conditionals, TODO:  may make Inventory State cover ALL possible windows like this.
    [Tooltip("is options currently open.")]
    private bool isOptions = false; //Dont want player resuming from options
    #endregion

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked; //stops cursor from going off window
        Cursor.visible = false;  //hides the cursor from view
    }

    // Update is called once per frame
    void Update()
    {
        #region Resume/Pause on Esc
        if (Input.GetKeyDown(KeyCode.Escape) && !isOptions && !player.isDead) //if ESC key is pressed and options isnt open and isnt dead.
        {
            if (gameIsPaused) //if game is paused
            {
                Resume();
            }
            else if (!GameManager.isDisplay)
            {
                Pause();
            }
        }
        #endregion
    }

    #region Pause Menu Methods
    /// <summary>
    /// To toggle options bool for conditionals
    /// </summary>
    /// <param name="isActive">Does this action cause options to open?</param>
    public void IsOptions(bool isActive)
    {
        isOptions = isActive;
    }

    #region Resume/Pause Methods
    /// <summary>
    /// Hides Pause Menu, returns game time to normal, locks and hides cursor.
    /// </summary>
    public void Resume()
    {
        GameManager.isDisplay = false;

        foreach (GameObject element in pauseMenuUI)
        {
            element.SetActive(false); //hide pause menu UI
        }
        Time.timeScale = 1f;//time resumes normal game time
        gameIsPaused = false; //sets bool for conditionals

        Cursor.lockState = CursorLockMode.Locked; //locks cursor midscreen
        Cursor.visible = false;  //hides the cursor from view
    }

    /// <summary>
    /// Reveals Pause Menu, freezes game time, unlocks and reveals cursor.
    /// </summary>
    public void Pause()
    {
        GameManager.isDisplay = true;

        foreach (GameObject element in pauseMenuUI)
        {
            element.SetActive(true); //reveals Pause Menu UI
        }
        Time.timeScale = 0f;//Make game time stand still
        gameIsPaused = true; //sets bool for conditionals

        Cursor.lockState = CursorLockMode.None; //Unlocks cursor for use.
        Cursor.visible = true; //makes cursor visable for use.
    }
    #endregion
    
    /// <summary>
    /// Returns Time Scale to normal and loads the Main Menu
    /// </summary>
    public void GoMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Quits Application or if using Unity Editor stops play mode.
    /// </summary>
    public void QuitGame() //called with exit button
    {
        Debug.Log("Quitting Game");

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); //If using unity, run this code, exit play mode
#endif
        Application.Quit(); //if not unity editor, run the quit application/exe
                            //quit exe game, i.e. once published, this will quit
    }
    #endregion
}