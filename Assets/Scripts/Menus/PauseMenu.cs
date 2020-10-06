using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    //Don't attach to the pause menu UI itself, this would end up with the script being inactive and no longer able to detect user input
    [SerializeField]
    [Tooltip("Reference Pause Menu Game Object, to toggle active on/off")]
    private GameObject pauseMenuUI;

    [Tooltip("is the game currently paused.")]
    private bool gameIsPaused = false;
    [Tooltip("is options currently open.")]
    private bool isOptions = false; //Dont want player resuming from options

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isOptions) //if ESC key is pressed and options isnt open.
        {
            if (gameIsPaused) //if game is paused
            {
                Resume();
            }
            else //otherwise
            {
                Pause();
            }
        }
    }

    /// <summary>
    /// To toggle options bool for conditionals
    /// </summary>
    /// <param name="isActive">Does this action cause options to open?</param>
    public void IsOptions(bool isActive)
    {
        isOptions = isActive;
    }

    /// <summary>
    /// Hides Pause Menu, returns game time to normal, locks and hides cursor.
    /// </summary>
    public void Resume()
    {
        pauseMenuUI.SetActive(false); //hide pause menu UI
        Time.timeScale = 1f;//time resumes normal game time
        gameIsPaused = false; //sets bool for conditionals

        Cursor.lockState = CursorLockMode.Locked; //lockes cursor to center screen to avoid user moving pointer off game.
        Cursor.visible = false;  //hides the cursor from view
    }

    /// <summary>
    /// Reveals Pause Menu, freezes game time, unlocks and reveals cursor.
    /// </summary>
    public void Pause()
    {
        pauseMenuUI.SetActive(true); //reveals Pause Menu UI
        Time.timeScale = 0f;//Make game time stand still
        gameIsPaused = true; //sets bool for conditionals

        Cursor.lockState = CursorLockMode.None; //unlocks the cursor to use
        Cursor.visible = true; //makes cursor visable for use.
    }

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
}