using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindScript : MonoBehaviour
{
    public static Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>(); //where a key(string) returns a value(KeyCode)

    public Text up, down, left, right, jump; //creates a bunch of text variables, up is one down is another one etc

    private GameObject currentKey;
    //values between 0-255
    public Color32 changedKey = new Color32(39,171,249,255);//This one is blue
    public Color32 selectedKey = new Color32(239, 116, 36, 255);//this one is orange


    private void Start()
    {
        //keys.Add("Up", KeyCode.W);
        keys.Add("Up", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up", "W"))); //creating a keycode, by setting up a keycode value by converting Up value to string, if the value of Up doesn't exist, default to W
        keys.Add("Down", KeyCode.S);
        keys.Add("Left", KeyCode.A);
        keys.Add("Right", KeyCode.D);
        keys.Add("Jump", KeyCode.Space);


        //keys["Up"] = KeyCode.W; // to have a deafault/reset keys option

        //sets this up in the beginning
        up.text = keys["Up"].ToString();
        down.text = keys["Down"].ToString();
        left.text = keys["Left"].ToString();
        right.text = keys["Right"].ToString();
        jump.text = keys["Jump"].ToString();
    }

    private void Update()
    {
        //instead of Input.GetKeyDown(KeyCode.W), we use the dictonary 'keys' with value "Up" to call what KeyCode is required to be pressed for said action
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
    }

    private void OnGUI() //allows us to run Events
    {
        //e.keycode DOESNT store SHIFT KEYS
        string newKey = "";
        Event e = Event.current;
        //when currentKey us not nnunll, that means we want to reassign a key
        if (currentKey != null)//only when we are trying to reassign a key, so when we click a button, currentKey will NOT equal null
        {
            if (e.isKey)
            {
                //this is any key right now
                newKey = e.keyCode.ToString();
            }

            //There is an issue getting shift keys, the following will fix that
            if(Input.GetKey(KeyCode.LeftShift))
            {
                newKey = "LeftShift";
            }
            if (Input.GetKey(KeyCode.RightShift))
            {
                newKey = "RightShift";
            }
            if(newKey != "") //if we have a set key
            {
                //we change our dictionary (that means our keybind changes too)
                keys[currentKey.name] = (KeyCode)System.Enum.Parse(typeof(KeyCode), newKey);
                //getting the text component in our button and changing the text
                currentKey.GetComponentInChildren<Text>().text = newKey;
                currentKey.GetComponent<Image>().color = changedKey; //gives a color to notify user key ISNT the default
                currentKey = null;
            }
        }
    }
    public void ChangeKey(GameObject clickKey) // used as an onclick
    {
        //gets connected to the UI button so works on press
        currentKey = clickKey;
        if (clickKey != null)
        {
            currentKey.GetComponent<Image>().color = selectedKey;//gives us a visual update that this key is currently selected for changing
        }
    }

    public void SaveKeys()
    {

        foreach(var key in keys) //
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }

    }
}
