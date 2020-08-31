using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;//controlling audio in script
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
    public AudioMixer mixer;
    public Slider musicSlider;
    public Slider SFXSlider;
    #endregion

    //
    public void Awake()//connecting to other things, loading things, so when we run start, everything is already set up for us
    {
    }

    public void Start() //called once when a script is enabled before any Update methods are called
    {
        LoadPlayerPrefs();
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

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVol", value);//same name as exposed parameters
    }
    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVol", value);//same name as exposed parameters
    }


    #region Save and Load Player Prefs
    public void SavePlayerPrefs() //called when exiting Options Menu, back button on click
    {
        //save quality
        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel()); //Get value of current Quality Level and set value as player pref quality

        //save fullscreen
        if(fullscreenToggle.isOn) //the toggle ui, if full screen set as fullscreen, otherwise set as windowed
        {
            PlayerPrefs.SetInt("fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("fullscreen", 0);
        }

        //save audio sliders
        float musicVol;
        if (mixer.GetFloat("MusicVol", out musicVol)) //out lets changes to external variables, typically we would use return, only use out if you have a REALLY GOOD REASON, the method we are calling must request an out
        {
            PlayerPrefs.SetFloat("MusicVol", musicVol); //when you pass a var as a parameter, 
        }

        float SFXVol;
        if (mixer.GetFloat("SFXVol", out SFXVol)) //out lets changes to external variables, typically we would use return, only use 'out' if you have a REALLY GOOD REASON, i.e. the method we are calling requests an out
        {
            PlayerPrefs.SetFloat("SFXVol", SFXVol);
        }

        PlayerPrefs.Save();//above makes/sets changes, this line saves those changes
        //PlayerPrefs.SetInt("somevalue", 60); //a key of 'somevalue' stores a value of 60, saves to a file for us, similar to a hashtable, we use this key to call value too, cant save bools, no custom classes either
    }

    public void LoadPlayerPrefs()//reads player prefs and applies its values, option button on click
    {
        //Load and set quality from player prefs
        if (PlayerPrefs.HasKey("quality"))//checks to see if there is a key called quality before trying to assign values, to avoid errors
        {
            int quality = PlayerPrefs.GetInt("quality");
            qualityDropdown.value = quality;

            if (QualitySettings.GetQualityLevel() != quality)
            {
                ChangeQuality(quality);
            }
        }

        if (PlayerPrefs.HasKey("fullscreen"))//checks to see if there is a key called quality before trying to assign values, to avoid errors
        {
            if (PlayerPrefs.GetInt("fullscreen") == 0) //if player pref fullscreen value is 0 (like false)
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


        //load audio Slider
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVol");
            musicSlider.value = musicVol;
            mixer.SetFloat("MusicVol", musicVol);
        }

        if (PlayerPrefs.HasKey("SFXVol"))
        {
            float SFXVol = PlayerPrefs.GetFloat("SFXVol");
            SFXSlider.value = SFXVol;
            mixer.SetFloat("SFXVol", SFXVol);
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
