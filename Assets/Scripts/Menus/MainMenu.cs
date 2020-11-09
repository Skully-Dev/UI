using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; //useful namespace, allows use of common code relating to Scene Management

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Reference Continue button, if save exists, activate")]
    private GameObject continueButton;

    [Tooltip("The name of the scene to load. The GameScene.")]
    private string LoadScene = "GameScene";//sets a default scene to be loaded, as GameScene.

    public void Awake()//connecting to other things, loading things, so when we run start, everything is already set up for us
    {
        //turns off debug logs if not in Unity Editor
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else   
        Debug.unityLogger.logEnabled = false;      
#endif
    }

    public void Start() //called once when a script is enabled before any Update methods are called
    {
        Debug.Log("Starting Game Main Menu"); //Plays at start of the game
        continueButton.SetActive(PlayerBinarySave.SaveExists()); //Active status depends on existence of save file
    }
    #region Start Game and Quit Game Methods
    /// <summary>
    /// Loads the gamescene, Method called when play button is clicked in main menu GUI
    /// </summary>
    public void StartGame()
    { 
        SceneManager.LoadScene(LoadScene); //Loads the game scene
    }

    /// <summary>
    /// Quits Application, or if using Unity Editor, Exits Playmode
    /// </summary>
    public void QuitGame() //called with on click of UI exit button
    {
        Debug.Log("Quitting Game"); //console line

#if UNITY_EDITOR //if a specific platform, i.e. unity EDITOR, other e.g. IOS, PS4 etc.
        EditorApplication.ExitPlaymode(); //If using unity, run this code, exit play mode
#endif
        Application.Quit(); //if not unity editor, run the quit application/exe
                            //quit exe game, i.e. once published, this will quit
    }
    #endregion

    #region IMGUI buttons
    /*
    public void OnGUI() //A less customizable button, but one we can add using code.
    {
        GUI.Box(new Rect(10, 10, 180, 120), "Testing Box"); //A box around the button
        //if pressed returns true
        if (GUI.Button(new Rect(20, 40, 80, 20), "Button 1")) //Creates a usable button, that appears in top left corner during play mode // 20 x position, 40 y position, 80 width, 20 height
        {
            Debug.Log("Button 1 got pressed");
        }
        if (GUI.Button(new Rect(100, 40, 80, 20), "Button 2")) //Creates a usable button, that appears in top right corner during play mode
        {
            Debug.Log("Button 2 was Pressed");
        }
        if (GUI.Button(new Rect(20, 60, 80, 20), "Button 3")) //Creates a usable button, that appears in bottom left corner during play mode
        {
            Debug.Log("Button 3 was Pressed");
        }
        if (GUI.Button(new Rect(100, 60, 80, 20), "Button 4")) //Creates a usable button, that appears in bottom right corner during play mode
        {
            Debug.Log("Button 4 was Pressed");
        }
        if (GUI.Button(new Rect(20, 80, 160, 40), "Stop Game")) //Creates a usable button, that appears below other buttons during play mode
        {
            Debug.Log("Stop Game Button was Pressed");
            QuitGame();
        }
    }
    */
    #endregion
}
