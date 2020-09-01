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
    public AudioMixer mixer; //allows to reference the audio manager, a universal game audio control to e.g. adjust volume sliders for game music or sound effects individually from the same menu
    public Slider musicSlider; //reference the options slider UI for Music, to allow user to adjust volume in game
    public Slider SFXSlider;//reference the options slider UI for Sound Effects, to allow user to adjust volume in game
    #endregion

    public void Awake()//connecting to other things, loading things, so when we run start, everything is already set up for us
    {
    }

    public void Start() //called once when a script is enabled before any Update methods are called
    {
        LoadPlayerPrefs(); //loads the player prefs that have values, check method for detail
        Debug.Log("Starting Game Main Menu"); //Plays at start of the game
    }

    public void StartGame () //The Method called when New or Continue button is clicked in main menu GUI
    {
        SceneManager.LoadScene(LoadScene); //Loads the game scene

        if (!PlayerPrefs.HasKey("fullscreen"))//to see if player prefs has a value for fullscreen saved yet
        {
            PlayerPrefs.SetInt("fullscreen", 0); //if it doesnt, set prefs to windowed, where fullscreen value is 0 i.e. not fullscreen
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

        if(!PlayerPrefs.HasKey("quality"))//to see if player pref quality has a value saved yet
        {
            PlayerPrefs.SetInt("quality", 5); //if it doesnt, set player pref quality to 5 i.e. Ultra(highest) quality, 5 = number of quality levels available minus one. 0 is lowest quality.
            QualitySettings.SetQualityLevel(5); //applies highest quality setting
            //dont have magic numbers in the future, like where does this number come from, what does it mean, it isnt a previously defined variable. 
        }
        else
        {
            QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("quality"));//otherwise, set actual quality to player pref i.e. last saved value.
        }

    }

    #region Change Settings
    public void SetFullScreen(bool fullscreen) // if true we are fullscreen, flase is windowed, Fullscreen toggle on UI value change
    {
        Screen.fullScreen = fullscreen; //applies corresponding bool value
    }

    public void ChangeQuality(int index) //called with quality UI dropdown on value change
    {
        QualitySettings.SetQualityLevel(index); //sets the quality, it will match the quality settings as they are aligned the same way, 0 being the lowest and the highest being the number of the quality options available minus 1.
    }
    #endregion

    public void QuitGame () //called with on click of UI exit button
    {
        Debug.Log("Quitting Game"); //console line

#if UNITY_EDITOR //if a specific platform, i.e. unity EDITOR, other e.g. IOS, PS4 etc.
        EditorApplication.ExitPlaymode(); //If using unity, run this code, exit play mode
#endif
        Application.Quit(); //if not unity editor, run the quit application/exe
                            //quit exe game, i.e. once published, this will quit
    }

    public void SetMusicVolume(float value) //on value change of UI sliders, take value to set below
    {
        mixer.SetFloat("MusicVol", value); //Set value of mixers like named exposed parameter
    }
    public void SetSFXVolume(float value) //on value change of UI sliders, take value to set below
    {
        mixer.SetFloat("SFXVol", value); //Set value of mixers like named exposed parameter
    }


    #region Save and Load Player Prefs
    public void SavePlayerPrefs() //called when exiting Options Menu, back button on click, save values to player prefs
    {
        //PlayerPrefs.SetInt("somevalue", 60); //a key of 'somevalue' stores a value of 60, saves to a file for us, similar to a hashtable, we use this key to call value too, cant save bools, no custom classes either

        //save quality
        PlayerPrefs.SetInt("quality", QualitySettings.GetQualityLevel()); //Get value of current Quality Level and set the value player pref quality

        //save fullscreen
        if(fullscreenToggle.isOn) //the toggle ui, if full screen is ticked , 
        {
            PlayerPrefs.SetInt("fullscreen", 1); //set the fullscreen playerPrefs value as 1(ticked)
        }
        else //otherwise
        {
            PlayerPrefs.SetInt("fullscreen", 0); //set the value as 0(unticked), windowed
        }

        //save audio sliders
        float musicVol; //variable declairation for music volume value
        //out lets changes to external variables, typically we would use return, only use out if you have a REALLY GOOD REASON, i.e. the method we are calling requests an out
        if (mixer.GetFloat("MusicVol", out musicVol)) //method returns a bool of true if mixer has a value for parameter of MusicVol and stores mixers MusicVol value in musicVol variable
        {
            PlayerPrefs.SetFloat("MusicVol", musicVol); //if true, musicVol value is applied to PlayerPrefs MusicVol
        }

        float SFXVol; //variable declairation for sound effects volume value
        //out lets changes to external variables, typically we would use return, only use out if you have a REALLY GOOD REASON, i.e. the method we are calling requests an out
        if (mixer.GetFloat("SFXVol", out SFXVol)) //method returns a bool of true if mixer has a value for parameter of SFXVol and stores mixers SFXVol value in SFXVol variable
        {
            PlayerPrefs.SetFloat("SFXVol", SFXVol); //if true, SFXVol value is applied to PlayerPrefs SFXVol
        }

        PlayerPrefs.Save();//above sets changes, this line saves those changes to PlayerPrefs on device.
    }

    public void LoadPlayerPrefs()//reads player prefs and applies its values
    {
        //Load and set quality from player prefs
        if (PlayerPrefs.HasKey("quality"))//checks to see if there is a key called quality before trying to assign values, to avoid errors
        {
            int quality = PlayerPrefs.GetInt("quality"); //get player pref quality value, store in quality int var.
            qualityDropdown.value = quality; //set quality dropdown to select quality the same as player pref for quality

            if (QualitySettings.GetQualityLevel() != quality) //if the game engines quality setting isnt the same value.
            {
                ChangeQuality(quality); //set its quality setting to the last chosen value
            }
        }

        if (PlayerPrefs.HasKey("fullscreen"))//checks to see if there is a key called fullscreen before trying to assign values, to avoid errors
        {
            if (PlayerPrefs.GetInt("fullscreen") == 0) //if player pref fullscreen value is 0 (like false)
            {
                //Set GUI toggle off
                fullscreenToggle.isOn = false;
            }
            else //otherwise
            {
                //set GUI toggle on
                fullscreenToggle.isOn = true;
            }
        }


        //load audio Slider
        if (PlayerPrefs.HasKey("MusicVol"))//checks to see if there is a key called MusicVol before trying to assign values, to avoid errors
        {
            float musicVol = PlayerPrefs.GetFloat("MusicVol"); //set musicVol to equal player pref
            musicSlider.value = musicVol; //apply value to UI
            mixer.SetFloat("MusicVol", musicVol); //apply value to mixer (actual volume control)
        }

        if (PlayerPrefs.HasKey("SFXVol"))//checks to see if there is a key called SFXVol before trying to assign values, to avoid errors
        {
            float SFXVol = PlayerPrefs.GetFloat("SFXVol"); //set SFXVol to player pref
            SFXSlider.value = SFXVol; //apply value to slider
            mixer.SetFloat("SFXVol", SFXVol); //apply value to mixer, actual volume
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
