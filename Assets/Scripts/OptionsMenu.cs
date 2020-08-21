using System.Collections;
using System.Collections.Generic;//for lists
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public Resolution[] resolutions;
    public Dropdown resolution;

    private void Start()
    {
        resolutions = Screen.resolutions; //fills array with all resolutions possible on current monitor
        resolution.ClearOptions();//clears out original dropdown options
        List<string> options = new List<string>(); //more powerful than arrays but less efficient

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)// go through for every resolution, however many are in the array
        {
            //Build a string for displaying the resolution
            string option = resolutions[i].width + "x" + resolutions[i].height; //Generates the names of the resolutions like 1920x1080
            options.Add(option);//adds them to the options list
            if(resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)//checks to see if this option is the same as the screens current resolution
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
        Screen.SetResolution(res.width, res.height, false);//I think flase needs to be changed at a later date
    }
}
