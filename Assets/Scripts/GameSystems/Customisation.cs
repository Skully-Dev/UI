using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Customisation : MonoBehaviour
{
    public bool showOnGUI = true;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Text characterName;

    public string inputName;

    [SerializeField]
    private string sceneToPlay = "GameScene";

    [SerializeField]
    private string TextureLocation = "Character/"; //location of warrior textures in project
    
    public enum CustomiseParts { Skin, Hair, Eyes, Mouth, Clothes, Armour };

    [SerializeField, Tooltip("The defaults for each profession coded in inspector")]
    PlayerProfession[] playerProfessions;

    [SerializeField, Tooltip("The defaults for each race coded in inspector")]
    PlayerRace[] playerRaces;

    //Renderer for our character mesh so we can reference materials list within script for  changinfg visuals
    public Renderer characterRenderer;

    //Enum.GetNames(typeof(CustomiseParts)).Length gets the number of customise parts we have (6) 
    //an array of List<Texture>
    //in other words 6 lists
    public List<Texture2D>[] partsTexture = new List<Texture2D>[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e. 6
    [SerializeField] private int[] currentPartsTextureIndex = new int[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e.6

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

    [Tooltip("Store the position of Professions scroll thingy")]
    public Vector2 scrollPositionProfession = Vector2.zero;

    [Tooltip("Store the position of Races scroll thingy")]
    public Vector2 scrollPositionRace = Vector2.zero;

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

        if (player == null)
        {
            Debug.LogError("player in Customisation is null");
        }
        else
        {
            if (player.customisationTextureIndex.Length != 0)
            {
                currentPartsTextureIndex = player.customisationTextureIndex;
            }
        }

        #region Assign first choice as default of Profession and Race
        if (playerProfessions != null
            && playerProfessions.Length > 0)
        {
            player.Profession = playerProfessions[0];
        }

        if (playerRaces != null
            && playerRaces.Length > 0)
        {
            player.Race = playerRaces[0];
        }
        #endregion

        //string[] of each body part = Enum.GetNames(typeof(CustomiseParts))
        //["Skin", "Hair", "Eyes", "Mouth", "Clothes", "Armour"]
        foreach (string part in Enum.GetNames(typeof(CustomiseParts))) //loop through our array of parts
        {
            SetTexture(part, 0);
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

    /// <summary>
    /// Save character settings to player prefs
    /// </summary>
    public void SaveCharacter()
    {
        player.customisationTextureIndex = currentPartsTextureIndex;
        player.playerStats.stats.name = inputName;
        PlayerBinarySave.SavePlayerData(player);


        /* WIP REDUNDANT
        //saves index of each
        PlayerPrefs.SetInt("Skin Index", currentPartsTextureIndex[0]);
        PlayerPrefs.SetInt("Hair Index", currentPartsTextureIndex[1]);
        PlayerPrefs.SetInt("Eyes Index", currentPartsTextureIndex[2]);
        PlayerPrefs.SetInt("Mouth Index", currentPartsTextureIndex[3]);
        PlayerPrefs.SetInt("Clothes Index", currentPartsTextureIndex[4]);
        PlayerPrefs.SetInt("Armour Index", currentPartsTextureIndex[5]);

        PlayerPrefs.SetString("Character Name", characterName.text);

        for (int i = 0; i < player.playerStats.baseStats.Length; i++)
        {
            PlayerPrefs.SetInt(player.playerStats.baseStats[i].baseStatName + " stat", player.playerStats.baseStats[i].defaultStat);
            PlayerPrefs.SetInt(player.playerStats.baseStats[i].baseStatName + " additionalStat", player.playerStats.baseStats[i].additionalStat);
            PlayerPrefs.SetInt(player.playerStats.baseStats[i].baseStatName + " levelUpStat", player.playerStats.baseStats[i].levelUpStat);
        }

        PlayerPrefs.SetString("Character Profession", player.Profession.ProfessionName);
        */
    }

    private void OnGUI() //like update but runs at a diffirent specific time for GUI
    {
        if (showOnGUI)
        {
            CustomiseOnGUI();
            StatsOnGUI();
            ProfessionsOnGUI();
            RacesOnGUI();
            NameOnGUI();

            if (GUI.Button(new Rect(10, 290, 120, 20), "Save & Play"))
            {
                SaveCharacter();
                SceneManager.LoadScene(sceneToPlay); //load gamescene
            }
        }
    }

    private void NameOnGUI()
    {
        GUI.Box(new Rect(10, 230, 120, 50), "Name");
        inputName = GUI.TextField(new Rect(20, 250, 100, 20), inputName, 15);

    }

    /// <summary>
    /// OnGUI Race scoll box w ALL available Races and their Abilities automatically added.
    /// Also displays info about current selected Race Ability in box near bottom screen.
    /// </summary>
    private void RacesOnGUI()
    {
        float curLoopHeight = 0;

        GUI.Box(new Rect(Screen.width - 170, 320, 155, 80), "Race");

        //On Screen scrollable box with enough space for all Races added in inspector
        scrollPositionRace = GUI.BeginScrollView(new Rect(Screen.width - 170, 340, 155, 50),
                                                      scrollPositionRace,
                                                      new Rect(0, 0, 100, 30 * playerRaces.Length));

        //Adds each of the Races to the scollable view as buttons, appropriately spaced out
        int i = 0;
        foreach (PlayerRace race in playerRaces)
        {
            if (GUI.Button(new Rect(0, curLoopHeight + i * 30, 100, 20), race.RaceName))
            {
                player.Race = race;
            }
            i++;
        }

        GUI.EndScrollView();

        #region Information displayed about Race and related ability
        GUI.Box(new Rect(Screen.width - 340, Screen.height - 90, 155, 80), "Race Ability");
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 30, 100, 20), player.Race.RaceName);
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 45, 100, 20), player.Race.AbilityName);
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 60, 100, 20), player.Race.AbilityDescription);
        #endregion
    }

    /// <summary>
    /// OnGUI Profession scoll box w ALL available Professions and their Abilities and BASESTATS automatically added.
    /// Also displays info about current selected Professsion Ability in box near bottom screen.
    /// </summary>
    private void ProfessionsOnGUI()
    {
        float curLoopHeight = 0;

        GUI.Box(new Rect(Screen.width - 170, 230, 155, 80), "Profession");

        //On screen scrollable box with enough space for all Professions listed in inspector
        scrollPositionProfession = GUI.BeginScrollView(new Rect(Screen.width - 170, 250, 155 , 50),
                                                      scrollPositionProfession,
                                                      new Rect(0,0,100,30 * playerProfessions.Length));

        //Adds each of the Professions to the scollable view as buttons, appropriately spaced out
        int i = 0;
        foreach (PlayerProfession profession in playerProfessions)
        {
            if (GUI.Button(new Rect(0, curLoopHeight + i * 30, 100, 20), profession.ProfessionName))
            {
                player.Profession = profession;
            }
            i++;
        }

        GUI.EndScrollView();

        #region Information displayed about Profession and related ability
        GUI.Box(new Rect(Screen.width - 170, Screen.height - 90, 155, 80), "Profession Ability");
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 30, 100, 20), player.Profession.ProfessionName);
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 45, 100, 20), player.Profession.AbilityName);
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 60, 100, 20), player.Profession.AbilityDescription);
        #endregion
    }

    private void StatsOnGUI()
    {
        float curLoopHeight = 40;
        GUI.Box(new Rect(Screen.width - 170, 10, 155, 210), "Stats : " + player.playerStats.stats.baseStatPoints);

        for (int i = 0; i < player.playerStats.stats.baseStats.Length; i++)
        {
            BaseStats stat = player.playerStats.stats.baseStats[i];

            if (GUI.Button(new Rect(Screen.width - 165, curLoopHeight + i * 30, 20, 20), "-")) 
            {
                player.playerStats.SetStats(i, -1);
            }
            
            GUI.Label(new Rect(Screen.width - 140, curLoopHeight + i * 30, 100, 20), 
                stat.baseStatName + ": " + stat.FinalStat);


            if (GUI.Button(new Rect(Screen.width - 40, curLoopHeight + i * 30, 20, 20), "+"))
            {
                player.playerStats.SetStats(i, 1);
            }
        }
    }

    private void CustomiseOnGUI()
    {

        //possible to set size as a ratio of screen size so it will scale with custom resolutions
        //GUI.Box(new Rect(Screen.width - 110, 10, 100, 90), "Top Right"); //This will make sure it is always on the right side

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
        /* WIP REDUNDANT
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
