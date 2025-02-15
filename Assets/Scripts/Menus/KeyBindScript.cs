﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindScript : MonoBehaviour
{

    private static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>(); //where a key(string) returns a value(KeyCode)
    #region References
    //we store keycodes as strings in these Text variables, then use these as the text in the buttons, therefore buttons show keycode for action.
    //can be found on Keybinds menu with referrence variables matching the names below
    [SerializeField]
    [Tooltip("To store keycodes as stings in them, this sets the text in the UI buttons. Therefore the buttons show the KeyCode to use their action.")]
    private Text up, down, left, right, jump; //creates a bunch of text variables, up is one, down is another one etc, SAME AS CREATING 5 DIFFIRENT VARIABLES

    [SerializeField]
    [Tooltip("Reference Buttons to reset colors to white")]
    private Image[] buttons;

    private GameObject currentKey; //Used to store clicked key clickKey, the key the user wants to change button to.
    #endregion
    #region Colors
    [Header("Button Colors")]
    //values between 0-255
    [SerializeField]
    [Tooltip("The color the button changes when value is changed.")]
    private Color32 changedKey = new Color32(39,171,249,255);//This one by default is blue
    [SerializeField]
    [Tooltip("The color the button changes when it is selected.")]
    private Color32 selectedKey = new Color32(239, 116, 36, 255);//this one by default is orange
    [SerializeField]
    [Tooltip("The color the button changes when value fails to change.")]
    private Color32 unassignedKey = new Color32(255, 0, 0, 255); //This is red by default
    #endregion

    private void Start()
    {
        if (!keys.ContainsKey("Up"))//if dictonary isnt created
        {
            InitializeDictionary();
        }

        UpdateButtonsText();
    }

    private void Update()
    {
        #region User Input Controls
        //instead of Input.GetKeyDown(KeyCode.W), we use the dictonary 'keys' with value "Up" to call what KeyCode is required to be pressed for said action
        //Although the KeyBindScript. part on the line is not required in this example, as we are in that script already, we have coded it this way so if we copy the code to run characterController in the game scene, it will still work.
        if (Input.GetKeyDown(KeyBindScript.keys["Up"]))//(Keycode.W)
        {
            Debug.Log("Character moves forward");
        }
        if (Input.GetKeyDown(KeyBindScript.keys["Down"]))//(Keycode.S)
        {
            Debug.Log("Character moves backward");
        }
        if (Input.GetKeyDown(KeyBindScript.keys["Left"]))//(Keycode.A)
        {
            Debug.Log("Character moves left");
        }
        if (Input.GetKeyDown(KeyBindScript.keys["Right"]))//(Keycode.D)
        {
            Debug.Log("Character moves right");
        }
        if (Input.GetKeyDown(KeyBindScript.keys["Jump"]))//(Keycode.Space)
        {
            //Make the character jump
            Debug.Log("Jump");
        }
        #endregion
    }

    private void OnGUI() //allows us to run Events
    {
        #region Get key change information
        string newKey = ""; //a variable to store key pressed
        Event e = Event.current; //e is now the current GUI event

        if (currentKey != null)//only when we are trying to reassign a key, so when we click a button, currentKey will NOT equal null
        {
            if (e.isKey) //if what happened is a key was pressed
            {
                newKey = e.keyCode.ToString(); //stores the keycode in newKey as a string
                //e.keycode DOESNT store SHIFT KEYS
            }

            //There is an issue getting shift keys, the following will fix that
            if (Input.GetKey(KeyCode.LeftShift)) 
            {
                newKey = "LeftShift"; //set key as Left Shift
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                newKey = "RightShift"; //set key as right shift
            }
            #endregion
            #region My Double Assignment Code
            if (newKey != "") //this is done twice, first is to test if its an assigned key, 
            {
                foreach (var key in keys) //var as each element of keys is a complex data type
                {
                    if (key.Value.ToString() == newKey && newKey != (keys[currentKey.name]).ToString()) //(keys[currentKey.name]).ToString(); || currentKey.GetComponentInChildren<Text>().text; Both work
                    {
                        Debug.LogWarning("That key is already assigned");
                        currentKey.GetComponent<Image>().color = unassignedKey; //gives a color to notify user key is now unassigned
                        newKey = "";
                        currentKey = null;
                    }
                }
            }
            #endregion
            #region Applies valid key change
            if (newKey != "") //if we have a set key
            {
                //we change our dictionary (that means our keybind changes too)
                //as buttons were named correctly after the string values of the dictonary, currentKey.name returns a keys key name
                //that key of string now has the value, KeyCode.newKey, e.g. KeyCode.P
                keys[currentKey.name] = (KeyCode)System.Enum.Parse(typeof(KeyCode), newKey);
                //getting the text component in our button and changing the text
                currentKey.GetComponentInChildren<Text>().text = newKey; //changes Text of the child of the button clicked to newKey, e.g. P
                currentKey.GetComponent<Image>().color = changedKey; //gives a color to notify user key has been changed
                currentKey = null; //sets currentKey back to null to reset conditionals
            }
            #endregion
        }
    }
    #region Initialize Dictionary and Update Text Methods
    /// <summary>
    /// Initializes the dictonary Keys Up, Down, Left, Right, Jump and thier corresponding PlayerPref values.
    /// If there aren't preferences, defaults to WSAD and Space Bar.
    /// </summary>
    private void InitializeDictionary()
    {
        //Initializes the dictonary Keys by adding the typical elements below
        //the string/Key returns the KeyCode./Value
        //keys.Add("Up", KeyCode.W);
        keys.Add("Up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        //keys.Add("Down", KeyCode.S);
        keys.Add("Down", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down", "S"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        //keys.Add("Left", KeyCode.A);
        keys.Add("Left", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left", "A"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        //keys.Add("Right", KeyCode.D);
        keys.Add("Right", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right", "D"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        //keys.Add("Jump", KeyCode.Space);
        keys.Add("Jump", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Jump", "Space"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        //So at start, add listings to the keys dictionary with the values Up,Down,Left,Right,Jump and of keys of the same names string values which are converted to data type KeyCode, and if they DONT have a value yet, default to sting W,S,A,D,Space.
    }

    /// <summary>
    /// Makes the text of the buttons match the KeyCode of their actions.
    /// </summary>
    private void UpdateButtonsText()
    {
        //using the keys.Add("Key", Value/KeyCode);'s above we put the dictonaries values into the text values of the buttons
        up.text = keys["Up"].ToString(); //I.E. the up button text now equals the Keys dictonary of Up which initally would be a default value of KeyCode.W, which converted to string is simply "W".
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
    }
    #endregion

    #region Change Key Method
    /// <summary>
    /// Used as an onclick method for Keybind buttons
    /// clickKey is the button we clicked in GU.
    /// Method is connected to the UI button, so works on press.
    /// </summary>
    /// <param name="clickKey">clickKey is the button we clicked in GUI.</param>
    public void ChangeKey(GameObject clickKey) // used as an onclick method, clickKey is the button we clicked in GUI, method is connected to the UI button, so works on press
    {
        currentKey = clickKey; //current button

        if (clickKey != null) //if we have clicked a key and its selected
        {
            currentKey.GetComponent<Image>().color = selectedKey;//gives us a visual update (changes the button color) that this key is currently selected for changing
        }
    }
    #endregion
    #region Save/Reset Keys Methods
    /// <summary>
    /// Creates player prefs on current keyCode settings
    /// </summary>
    public void SaveKeys()
    {
        foreach(var key in keys) //var because keys are a complex data type.
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
    }

    /// <summary>
    /// Resets Actions Keycodes to typical settings
    /// </summary>
    public void ResetKeys()
    {
        //What are generally considered good default values
        keys["Up"] = KeyCode.W;
        keys["Down"] = KeyCode.S;
        keys["Left"] = KeyCode.A;
        keys["Right"] = KeyCode.D;
        keys["Jump"] = KeyCode.Space;

        UpdateButtonsText();

        foreach (Image image in buttons)//sets the buttons colors back to white
        {
            image.color = Color.white;
        }
    }
    #endregion
}
