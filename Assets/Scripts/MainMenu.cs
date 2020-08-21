using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement; //useful namespace, allows use of common code relating to Scene Management
using UnityEngine.UI; //allows the use of common code relating to UI
//Above are namespaces this script accesses, each contain various classes, methods and other handy code relating to specific functionality.

public class MainMenu : MonoBehaviour //a Main Menu Class derived from base class MonoBehaviour, which allows us to assign this class as a component to a game object, attached to Menu Manager Game Object
{
    //Be sure to attach script to game
    #region Public Variables
    //variables that you can assign vales in the inspector
    public string LoadScene = "GameScene";//sets a default scene to be loaded, as GameScene, value also settable in inspector.

    public Dropdown qualityDropdown;//allows reference of a dropdown component, quality dropdown referenced 
    public Toggle fullscreenToggle; //allows to reference a Toggle component, fullscreen referenced
    #endregion


    public void Start() //called once when a script is enabled before any Update methods are called
    {
        Debug.Log("Starting Game Main Menu"); //Plays at start of the game
    }

    public void StartGame () //The Method called when New or Continue button is clicked in main menu GUI
    {
        

        SceneManager.LoadScene(LoadScene); //Loads the game scene

        if (!PlayerPrefs.HasKey("fullscreen"))//to see if player prefs has a value for fullscreen saved yet
        {
            PlayerPrefs.SetInt("fullscreen", 0); //set prefs to windowed, where fullscreen value is 0 (similar to false)
            Screen.fullScreen = false;  //makes it windowed
        }
        else
        {
            if (PlayerPrefs.GetInt("fullscreen") == 0) //otherwise, if player prefs fullscreen is set to 0
            {
                Screen.fullScreen = false; //make it windowed
            }
            else
            {
                Screen.fullScreen = true; //otherwise, fullscreen must equal 1, so set to fullscreen
            }
        }

        if(!PlayerPrefs.HasKey("quality"))//to see if player pref quality has a value saved vet
        {
            PlayerPrefs.SetInt("quality", 5); //if not, set player pref quality to 5 i.e. Ultra(highest) quality
            QualitySettings.SetQualityLevel(5);
            //dont have magic numbers in the future, like where does this number come from, what does it mean, it isnt a previously defined variable. 
        }
        else
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));//otherwise, set to player pref
        }

    }

    #region Change Settings
    public void SetFullScreen(bool fullscreen) // if true we are fullscreen, flase is windowed, Fullscreen toggle on value change
    {
        Screen.fullScreen = fullscreen;
    }

    public void ChangeQuality(int index) //called with quality dropdown on value change
    {
        QualitySettings.SetQualityLevel(index); ///sets the quality, it will match the quality settings as they are aligned the same way
    }
    #endregion

    public void QuitGame () //called with exit button
    {
        Debug.Log("Quitting Game");

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); //If using unity, run this code, exit play mode
#endif
        Application.Quit(); //if not unity editor, run the quit application/exe
                            //quit exe game, i.e. once published, this will quit
    }

    #region Save and Load Player Prefs
    public void SavePlayerPrefs() //called when exiting Options Menu, back button on click
    {
        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel()); //Get value of current Quality Level and set value as player pref quality

        if(fullscreenToggle.isOn) //the toggle ui, if full screen set as fullscreen, otherwise set as windowed
        {
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("fullscreen", 0);
        }

        PlayerPrefs.Save();//above makes/sets changes, this line saves those changes
        //PlayerPrefs.SetInt("somevalue", 60); //a key of 'somevalue' stores a value of 60, saves to a file for us, similar to a hashtable, we use this key to call value too, cant save bools, no custom classes either
    }

    public void LoadPlayerPrefs()//reads player prefs and applies its values, option button on click
    {
        qualityDropdown.value = PlayerPrefs.GetInt("quality"); //get/access player prefs quality value

        if(PlayerPrefs.GetInt("fullscreen") == 0) //if player pref fullscreen value is 0 (like false)
        {
            //Set GUI toggle off
            fullscreenToggle.isOn = false;
        }
        else
        {
            //set GUI toggle on
            fullscreenToggle.isOn = true;
        }
    }
    #endregion

    public void OnGUI() //A less customizable button, but one we can add using code.
    {
        GUI.Box(new Rect(10, 10, 180, 120), "Testing Box"); //A box around the button
        //if press me button is pressed
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
}


//EditorApplication.Exit(0); // Exits Unity ITSELF
