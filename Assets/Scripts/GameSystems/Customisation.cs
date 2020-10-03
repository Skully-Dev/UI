using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Customisation : MonoBehaviour
{
    [SerializeField]
    private string TextureLocation = "Character/"; //location of warrior textures in project
    
    public enum CustomiseParts { Skin, Hair, Eyes, Mouth, Clothes, Armour };

    //Renderer for our character mesh so we can reference materials list within script for  changinfg visuals
    public Renderer characterRenderer;

    //Enum.GetNames(typeof(CustomiseParts)).Length gets the number of customise parts we have (6) 
    //an array of List<Texture>
    //in other words 6 lists
    public List<Texture2D>[] partsTexture = new List<Texture2D>[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e. 6
    private int[] currentPartsTextureIndex = new int[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e.6

    //first number = which body part
    //second number = which version of that body part
    //partsTexture[0][0] = Skin_0
    //partsTexture[0][1] = Skin_1
    //partsTexture[0][2] = Skin_2
    //partsTexture[0][3] = Skin_3
    //partsTexture[0][3] = Skin_3
    //partsTexture[1][0] = Hair_0
    //partsTexture[1][1] = Hair_1 //etc
    //partsTexture[2][0] = Eyes_0 //etc
    private void Start()
    {
        int partCount = 0;
        foreach (string part in Enum.GetNames(typeof(CustomiseParts))) //loop through our array of parts
        {
            int textureCount = 0;
            Texture2D tempTexture;

            partsTexture[partCount] = new List<Texture2D>(); //create a list for the part skin/hair/eyes etc.
            do //loop through rach texture and add it to this list
            {
                //tempTexture = Resources.Load(TextureLocation + part + "_" + count) as Texture; //same thing
                tempTexture = (Texture2D)Resources.Load(TextureLocation + part + "_" + textureCount);

                if (tempTexture != null)
                {
                    partsTexture[partCount].Add(tempTexture);
                }
                textureCount++;
            } while (tempTexture != null);
            partCount++;
        }
    }

    /// <summary>
    /// Changes to next/prior texture available
    /// One Override for SetTexture, same name different parameters, meaning the one used depends on the arguments passed through.
    /// (Not using this one)
    /// </summary>
    /// <param name="part">the textures to change through, like Skin_0, Skin_1 etc, or is it eyes/mouth etc.</param>
    /// <param name="direction"> what direction button the user presses, left(-1) or right(+1)</param>
    void SetTexture(CustomiseParts part, int direction)
    {

        int partIndex = (int)part;

        int max = partsTexture[partIndex].Count;

        int currentTexture = currentPartsTextureIndex[partIndex];
        currentTexture += direction;
        if (currentTexture < 0)
        {
            currentTexture = max - 1;
        }
        else if (currentTexture > max - 1)
        {
            currentTexture = 0;
        }
        currentPartsTextureIndex[partIndex] = currentTexture;

        Material[] mats = characterRenderer.materials;
        mats[partIndex].mainTexture = partsTexture[partIndex][currentTexture];
        characterRenderer.materials = mats;
    }
    /// <summary>
    /// Changes to next/prior texture available
    /// </summary>
    /// <param name="type">What part of the character do you want to change the texture for, "Skin", "Hair" etc.</param>
    /// <param name="direction">What direction button the user presses, left(-1) or right(+1)</param>
    void SetTexture(string type, int direction)
    {
        int partIndex = 0; //just creating 2 int var at the same time

        switch (type) //used kinda like a dictionary, just looking at the type "string" passed in and getting the appropriate index number
        {
            case "Skin":
                partIndex = 0;
                break;
            case "Hair":
                partIndex = 1;
                break;
            case "Eyes":
                partIndex = 2;
                break;
            case "Mouth":
                partIndex = 3;
                break;
            case "Clothes":
                partIndex = 4;
                break;
            case "Armour":
                partIndex = 5;
                break;

        }

        int max = partsTexture[partIndex].Count; // determines however many textures in current part list.

        int currentTexture = currentPartsTextureIndex[partIndex];
        currentTexture += direction;
        if (currentTexture < 0)
        {
            currentTexture = max - 1;
        }
        else if (currentTexture > max -1)
        {
            currentTexture = 0;
        }
        currentPartsTextureIndex[partIndex] = currentTexture;


        Material[] mats = characterRenderer.materials;
        mats[partIndex].mainTexture = partsTexture[partIndex][currentTexture];
        characterRenderer.materials = mats;
    }

    private void OnGUI() //like update but runs at a diffirent specific time for GUI
    {

        //possible to set size as a ratio of screen size so it will scale with custom resolutions
        GUI.Box(new Rect(Screen.width - 110, 10, 100, 90), "Top Right"); //This will make sure it is always on the right side

        GUI.Box(new Rect(10, 10, 120, 210), "Visuals"); //a box/pannel for us to put GUI in, the first two numbers are pixels from the top left followed by width and height

        #region Loop create buttons
        string[] names = { "Skin", "Hair", "Eyes", "Mouth", "Clothes", "Armour" };

        float curLoopHeight = 40f; //height of first button

        for (int i = 0; i < names.Length; i++)
        {
            if (GUI.Button(new Rect(20, curLoopHeight + i * 30, 20, 20), "<")) //button in if statements, as it returns true if pressed
            {
                SetTexture(names[i], -1);
            }

            GUI.Label(new Rect(45, curLoopHeight + i * 30, 60, 20), names[i]);

            if (GUI.Button(new Rect(100, curLoopHeight + i * 30, 20, 20), ">")) //button in if statements, as it returns true if pressed
            {
                SetTexture(names[i], 1);
            }
        }
        #endregion

        #region Manual way to make buttons, easier to understand, more repetitive code
        /*
        //SKIN
        float currentHeight = 40f;

        if (GUI.Button(new Rect(20, currentHeight, 20 ,20),"<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(58, currentHeight, 60, 20), "Skin");

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        //HAIR
        currentHeight += 30f;

        if (GUI.Button(new Rect(20, currentHeight, 20, 20), "<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(45, currentHeight, 70, 20), "Hair"); //could use a texture instead of text using new GUIStyle(//position //contentIEtexture)

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        //EYES
        currentHeight += 30f;

        if (GUI.Button(new Rect(20, currentHeight, 20, 20), "<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(45, currentHeight, 70, 20), "Eyes"); //could use a texture instead of text using new GUIStyle(//position //contentIEtexture)

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        //MOUTH
        currentHeight += 30f;

        if (GUI.Button(new Rect(20, currentHeight, 20, 20), "<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(45, currentHeight, 70, 20), "Mouth"); //could use a texture instead of text using new GUIStyle(//position //contentIEtexture)

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        //CLOTHES
        currentHeight += 30f;

        if (GUI.Button(new Rect(20, currentHeight, 20, 20), "<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(45, currentHeight, 70, 20), "Clothes"); //could use a texture instead of text using new GUIStyle(//position //contentIEtexture)

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        //ARMOUR
        currentHeight += 30f;

        if (GUI.Button(new Rect(20, currentHeight, 20, 20), "<")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }

        GUI.Label(new Rect(45, currentHeight, 70, 20), "Armour"); //could use a texture instead of text using new GUIStyle(//position //contentIEtexture)

        if (GUI.Button(new Rect(100, currentHeight, 20, 20), ">")) //button in if statements, as it returns true if pressed
        {
            TestMethod();
        }
        */
        #endregion
    }
}
