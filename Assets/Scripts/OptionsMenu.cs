using System.Collections.Generic;//for lists
using UnityEngine;
using UnityEngine.Audio;//controlling audio in script
using UnityEngine.UI; //allows the use of common code relating to UI
//Above are namespaces this script accesses, each contain various classes, methods and other handy code relating to specific functionality.

public class OptionsMenu : MonoBehaviour //a Main Menu Class derived from base class MonoBehaviour, which allows us to assign this class as a component to a game object, attached to Menu Manager Game Object
{
    //Be sure to attach script to game
    #region Public Variables
    //variables that you can assign vales in the inspector
    public Resolution[] resolutions;
    public Dropdown resolution;
    public Dropdown qualityDropdown;//allows reference of a dropdown component, quality dropdown referenced 
    public Toggle fullscreenToggle; //allows to reference a Toggle component, fullscreen referenced
    public AudioMixer mixer; //allows to reference the audio manager, a universal game audio control to e.g. adjust volume sliders for game music or sound effects individually from the same menu
    public Slider musicSlider; //reference the options slider UI for Music, to allow user to adjust volume in game
    public Slider SFXSlider;//reference the options slider UI for Sound Effects, to allow user to adjust volume in game
    #endregion

    private void Start()
    {
        LoadPlayerPrefs(); //loads the player prefs that have values, check method for detail
        PopulateResolutions();
    }


    #region Change Settings
    public void PopulateResolutions()
    {
        resolutions = Screen.resolutions; //fills array with all resolutions possible on current monitor
        resolution.ClearOptions();//clears out original dropdown options
        List<string> options = new List<string>(); //more powerful than arrays but less efficient

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)// go through for every resolution, however many are in the array
        {
            //Build a string for displaying the resolution
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz"; //Generates the names of the resolutions like 1920x1080 60Hz
            options.Add(option);//adds them to the options list
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)//checks to see if this option is the same as the screens current resolution and refresh rate
            {
                //weve found the current screen resolution, save that number, i.
                currentResolutionIndex = i;//sets current resolution index value to the true current resolution setting
            }
        }
        //sets up our dropdown
        resolution.AddOptions(options);
        resolution.value = currentResolutionIndex;
        resolution.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);//I think flase needs to be changed at a later date
    }

    public void SetFullScreen(bool fullscreen) // if true we are fullscreen, flase is windowed, Fullscreen toggle on UI value change
    {
        Screen.fullScreen = fullscreen; //applies corresponding bool value
    }

    public void ChangeQuality(int index) //called with quality UI dropdown on value change
    {
        QualitySettings.SetQualityLevel(index); //sets the quality, it will match the quality settings as they are aligned the same way, 0 being the lowest and the highest being the number of the quality options available minus 1.
    }
    #endregion

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
}
