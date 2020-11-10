using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Customisation : MonoBehaviour
{
    public enum CustomiseParts { Skin, Hair, Eyes, Mouth, Clothes, Armour };

    #region General Variables and Player Reference
    public bool showOnGUI = true;

    [SerializeField]
    private Player player;

    [SerializeField]
    private string sceneToPlay = "GameScene";

    [SerializeField, Tooltip("Reference Button Sound, easier to add to on events via script")]
    private AudioSource buttonSound;
    #endregion

    #region Race and Profession Inspector Settings
    [Header("Race and Profession Settings")]
    [SerializeField, Tooltip("The defaults for each profession coded in inspector")]
    PlayerProfession[] playerProfessions;

    [SerializeField, Tooltip("The defaults for each race coded in inspector")]
    PlayerRace[] playerRaces;
    #endregion

    #region Appearance Settings
    [Header("Appearance Settings")]

    [SerializeField, Tooltip("Location of warrior textures in project")]
    private string TextureLocation = "Character/";

    [Tooltip("Renderer for our character mesh so we can reference materials list within script for changing visuals")]
    public Renderer characterRenderer;

    //Parts Texture List Array information
    //Enum.GetNames(typeof(CustomiseParts)).Length gets the number of customise parts we have (6) 
    //an array of List<Texture>
    //in other words 6 lists
    //
    //first number = which body part
    //second number = which version of that body part
    //partsTexture[0][0] = Skin_0
    //partsTexture[0][1] = Skin_1
    //partsTexture[0][2] = Skin_2
    //partsTexture[0][3] = Skin_3
    //partsTexture[1][0] = Hair_0
    //partsTexture[1][1] = Hair_1 //etc
    //partsTexture[2][0] = Eyes_0 //etc
    [Tooltip("An array of Lists storing available textures for each part you can change textures for.")]
    public List<Texture2D>[] partsTexture = new List<Texture2D>[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e. 6

    [SerializeField, Tooltip("Stores the texture index values for each part of the mesh for easy saving and reapplying")]
    private int[] currentPartsTextureIndex = new int[Enum.GetNames(typeof(CustomiseParts)).Length]; //i.e.6
    #endregion

    #region Customisation Scene UI References and Variables
    [Header("Customise Scene UI stuff")]
    [Tooltip("The character name input string")]
    public string inputName;

    [SerializeField, Tooltip("Reference TEXT where Profession Ability and Discription should be displayed")]
    private Text professionText;
    [SerializeField, Tooltip("Reference TEXT where Race Ability and Discription should be displayed")]
    private Text raceText;

    [SerializeField, Tooltip("Reference the remaining points pool text")]
    private Text baseStatPointsText;
    [SerializeField, Tooltip("Reference each of the Base Stat Point Text in order of appearance.")]
    private Text[] statsPointsText;

    [Header("IMGUI Scroll variables")]
    [Tooltip("Store the position of Professions scroll thingy")]
    private Vector2 scrollPositionProfession = Vector2.zero;

    [Tooltip("Store the position of Races scroll thingy")]
    private Vector2 scrollPositionRace = Vector2.zero;
    #endregion

    private void Start()
    {
        #region Initialize the array with a List for each texture part with all available textures added to the corrisponding lists.
        int partCount = 0;
        foreach (string part in Enum.GetNames(typeof(CustomiseParts))) //loop through our array of parts
        {
            int textureCount = 0;
            Texture2D tempTexture;

            partsTexture[partCount] = new List<Texture2D>(); //create a list for the part skin/hair/eyes etc.
            do //loop through rach texture and add it to this list
            {
                //tempTexture = Resources.Load(TextureLocation + part + "_" + count) as Texture; //same thing
                tempTexture = (Texture2D)Resources.Load(TextureLocation + part + "_" + textureCount); //gets the texture file from the computer.

                if (tempTexture != null) //if file by that texture name exists at that location
                {
                    partsTexture[partCount].Add(tempTexture); //add it to that parts list of texture options
                }
                textureCount++;
            } while (tempTexture != null); //until no textures for that part left
            partCount++;
        }
        #endregion

        #region Checks if player has Texture Index Values, if so, use them.
        if (player == null)
        {
            Debug.LogError("player in Customisation is null");
        }
        else
        {
            if (player.customisationTextureIndex.Length != 0) //if player has values for textures
            {
                currentPartsTextureIndex = player.customisationTextureIndex; //copy over those texture index values
            }
        }
        #endregion

        #region Assign first choice as default of Profession and Race if not already assigned.
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

        #region Apply the texture for each appearance part based on index values.
        //string[] of each body part = Enum.GetNames(typeof(CustomiseParts))
        //["Skin", "Hair", "Eyes", "Mouth", "Clothes", "Armour"]
        foreach (string part in Enum.GetNames(typeof(CustomiseParts))) //loop through our array of parts
        {
            SetTexture(part, 0); //using the players texture index values, Sets textures on chararcter
        }
        #endregion

        #region Initialize Customise Scene UI values
        if ("Customise" == SceneManager.GetActiveScene().name)
        {
            professionText.text = player.Profession.AbilityName + " - " + player.Profession.AbilityDescription;
            raceText.text = player.Race.AbilityName + " - " + player.Race.AbilityDescription;
            UpdateAllStatPointsValues();
            baseStatPointsText.text = player.playerStats.stats.baseStatPoints.ToString();
        }
        #endregion
    }

    #region IMGUI OnGUI - Runs Methods
    private void OnGUI() //like update but runs at a diffirent specific time for GUI
    {
        if (showOnGUI)
        {
            CustomiseOnGUI();
            NameOnGUI();
            ProfessionsOnGUI();
            RacesOnGUI();
            StatsOnGUI();

            if (GUI.Button(new Rect(10, 290, 120, 20), "Save & Play"))
            {
                SaveAndPlay();
            }
        }
    }
    #endregion

    #region Customise Appearance Methods

    #region Set Texture Method (2 Overrides)

    #region Set Texture (string type) Override (USING THIS ONE!)
    /// <summary>
    /// Changes to next/prior texture available
    /// One Override for SetTexture, same name different parameters, meaning the one used depends on the arguments passed through.
    /// (USING THIS ONE)
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

        int max = partsTexture[partIndex].Count; // determines how many textures in current part list.

        //Shifts current index based on texture count
        int textureIndex = currentPartsTextureIndex[partIndex];
        textureIndex += direction;
        if (textureIndex < 0)
        {
            textureIndex = max - 1;
        }
        else if (textureIndex > max - 1)
        {
            textureIndex = 0;
        }
        currentPartsTextureIndex[partIndex] = textureIndex; //stores new index

        //applies to player mesh material main textures.
        Material[] mats = characterRenderer.materials;
        mats[partIndex].mainTexture = partsTexture[partIndex][textureIndex];
        characterRenderer.materials = mats;
    }
    #endregion

    #region Set Texture CustomiseParts Override (Currently NOT USED)
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

        int textureIndex = currentPartsTextureIndex[partIndex];
        textureIndex += direction;
        if (textureIndex < 0)
        {
            textureIndex = max - 1;
        }
        else if (textureIndex > max - 1)
        {
            textureIndex = 0;
        }
        currentPartsTextureIndex[partIndex] = textureIndex;

        Material[] mats = characterRenderer.materials;
        mats[partIndex].mainTexture = partsTexture[partIndex][textureIndex];
        characterRenderer.materials = mats;
    }
    #endregion
   
    #endregion

    /// <summary>
    /// Swaps though to the next available texture for this layer.
    /// </summary>
    /// <param name="nameIndex">0 = skin, 1 = hair, 2 = eyes, 3 = mouth, 4 = clothes, 5 = armour</param>
    public void NextTexture(int nameIndex)
    {
        string[] names = { "Skin", "Hair", "Eyes", "Mouth", "Clothes", "Armour" };
        SetTexture(names[nameIndex], 1);
        buttonSound.Play();
    }

    /// <summary>
    /// Swaps though to the previous available texture for this layer.
    /// </summary>
    /// <param name="nameIndex">0 = skin, 1 = hair, 2 = eyes, 3 = mouth, 4 = clothes, 5 = armour</param>
    public void PreviousTexture(int nameIndex)
    {
        string[] names = { "Skin", "Hair", "Eyes", "Mouth", "Clothes", "Armour" };
        SetTexture(names[nameIndex], -1);
        buttonSound.Play();
    }

    /// <summary>
    /// Chooses a random texture from available textures for each texture slot.
    /// </summary>
    public void RandomiseTextures()
    {
        Material[] mats = characterRenderer.materials; //makes a copy of the materials of the characters mesh

        for (int i = 0; i < currentPartsTextureIndex.Length; i++)
        {
            int textureCount = partsTexture[i].Count;
            int randomTextureIndex = UnityEngine.Random.Range(0, textureCount);

            currentPartsTextureIndex[i] = randomTextureIndex; //The simplified storage index variable.

            mats[i].mainTexture = partsTexture[i][randomTextureIndex]; //Changes the copy of the actual texture settings.
        }

        characterRenderer.materials = mats; //apply the changes to the characters mesh for each materials main texture.

        buttonSound.Play();
    }

    /// <summary>
    /// Chooses the first texture for each customisable part.
    /// </summary>
    public void ResetTextures()
    {
        Material[] mats = characterRenderer.materials; //makes a copy of the materials of the characters mesh
        //sets them all to the default 0 texture
        for (int i = 0; i < currentPartsTextureIndex.Length; i++)
        {
            currentPartsTextureIndex[i] = 0; //The simplified storage index variable.

            mats[i].mainTexture = partsTexture[i][0];  //Changes the copy of the actual texture settings.
        }
        characterRenderer.materials = mats; //apply the changes to the characters mesh for each materials main texture.

        buttonSound.Play();
    }

    /// <summary>
    /// Creates an IMGUI Box w Buttons for each of the customisable appearance options of the character.
    /// That then swap through the available textures.
    /// </summary>
    private void CustomiseOnGUI()
    {

        //possible to set size as a ratio of screen size so it will scale with custom resolutions
        //GUI.Box(new Rect(Screen.width - 110, 10, 100, 90), "Top Right"); //This will make sure it is always on the right side

        GUI.Box(new Rect(10, 10, 120, 210), "Visuals"); //a box/pannel for us to put GUI in, the first two numbers are pixels from the top left followed by width and height

        #region Loop create buttons for Customise Appearacne
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
    #endregion

    #region Change Name Methods
    /// <summary>
    /// On InputField value change of Chatacter Name, stores value to be saved.
    /// </summary>
    /// <param name="_name">The value of the input field, passed dynamically</param>
    public void ChangeName(string _name)
    {
        inputName = _name;
    }

    /// <summary>
    /// IM GUI text box to enter character name, dynamic.
    /// </summary>
    private void NameOnGUI()
    {
        GUI.Box(new Rect(10, 230, 120, 50), "Name");
        inputName = GUI.TextField(new Rect(20, 250, 100, 20), inputName, 20);
    }
    #endregion

    #region Change Race AND Profession Methods
    /// <summary>
    /// On Dropdown value change of Profession, applies chosen value 
    /// w the Abilities and BASESTATS automatically applied.
    /// Also displays info about selected Professions Ability.
    /// </summary>
    /// <param name="professionIndex">The index of dropdown chosen value, passed dynamically</param>
    public void ChangeProfession(int professionIndex)
    {
        player.Profession = playerProfessions[professionIndex];
        professionText.text = player.Profession.AbilityName + " - " + player.Profession.AbilityDescription;
        UpdateAllStatPointsValues();

        buttonSound.Play();
    }

    /// <summary>
    /// OnGUI Profession scoll box w ALL available Professions and their Abilities and BASESTATS automatically added.
    /// Also displays info about current selected Professsion Ability in box near bottom screen.
    /// </summary>
    private void ProfessionsOnGUI()
    {
        #region Profession Change scroll box and buttons
        float curLoopHeight = 0;

        GUI.Box(new Rect(Screen.width - 170, 230, 155, 80), "Profession");

        //On screen scrollable box with enough space for all Professions listed in inspector
        scrollPositionProfession = GUI.BeginScrollView(new Rect(Screen.width - 170, 250, 155, 50),
                                                      scrollPositionProfession,
                                                      new Rect(0, 0, 100, 30 * playerProfessions.Length));

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
        #endregion

        #region Information displayed about Profession and related ability
        GUI.Box(new Rect(Screen.width - 170, Screen.height - 90, 155, 80), "Profession Ability");
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 30, 100, 20), player.Profession.ProfessionName);
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 45, 100, 20), player.Profession.AbilityName);
        GUI.Label(new Rect(Screen.width - 140, Screen.height - 100 + 60, 100, 20), player.Profession.AbilityDescription);
        #endregion
    }

    /// <summary>
    /// On Dropdown value change of Race, Ability automatically applied.
    /// Also displays info about Ability.
    /// </summary>
    /// <param name="raceIndex">The index of dropdown chosen value, passed dynamically</param>
    public void ChangeRace(int raceIndex)
    {
        player.Race = playerRaces[raceIndex];
        raceText.text = player.Race.AbilityName + " - " + player.Race.AbilityDescription;

        buttonSound.Play();
    }

    /// <summary>
    /// OnGUI Race scoll box w ALL available Races and their Abilities automatically added.
    /// Also displays info about current selected Race Ability in box near bottom screen.
    /// </summary>
    private void RacesOnGUI()
    {
        #region Race Change scroll box and buttons
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

        #endregion

        #region Information displayed about Race and related ability
        GUI.Box(new Rect(Screen.width - 340, Screen.height - 90, 155, 80), "Race Ability");
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 30, 100, 20), player.Race.RaceName);
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 45, 100, 20), player.Race.AbilityName);
        GUI.Label(new Rect(Screen.width - 310, Screen.height - 100 + 60, 100, 20), player.Race.AbilityDescription);
        #endregion
    }
    #endregion

    #region Player Stat Points Methods
    /* Not needed
    public enum StatNames { Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma };
    public string[] statNames = Enum.GetNames(typeof(StatNames));
    */

    /// <summary>
    /// Updates all base stats point values
    /// </summary>
    private void UpdateAllStatPointsValues()
    {
        for (int i = 0; i < statsPointsText.Length; i++)
        {
            statsPointsText[i].text = player.playerStats.stats.baseStats[i].FinalStat.ToString();
        }
    }

    /// <summary>
    /// Increase specified player stat by 1 point.
    /// Also updates points UI for stat and remaining point pool.
    /// </summary>
    /// <param name="statIndex">stat index in relation to order of appearance</param>
    public void IncreaseStat(int statIndex)
    {
        player.playerStats.SetStats(statIndex, 1);
        statsPointsText[statIndex].text = player.playerStats.stats.baseStats[statIndex].FinalStat.ToString();
        baseStatPointsText.text = player.playerStats.stats.baseStatPoints.ToString();

        buttonSound.Play();
    }

    /// <summary>
    /// Decrease specified player stat by 1 point.
    /// Also updates points UI for stat and remaining point pool.
    /// </summary>
    /// <param name="statIndex">stat index in relation to order of appearance</param>
    public void DecreaseStat(int statIndex)
    {
        player.playerStats.SetStats(statIndex, -1);
        statsPointsText[statIndex].text = player.playerStats.stats.baseStats[statIndex].FinalStat.ToString();
        baseStatPointsText.text = player.playerStats.stats.baseStatPoints.ToString();

        buttonSound.Play();
    }

    /// <summary>
    /// Creates an IMGUI Box w Buttons to increase/decrease each of the Base Stats.
    /// Automatimated point limits and updated point pool.
    /// </summary>
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
    #endregion

    #region Save Methods
    /// <summary>
    /// Save player and load game scene.
    /// </summary>
    public void SaveAndPlay()
    {
        SaveCharacter();
        SceneManager.LoadScene(sceneToPlay); //load gamescene

        buttonSound.Play();
    }

    /// <summary>
    /// Save character settings to player prefs
    /// </summary>
    public void SaveCharacter()
    {
        player.customisationTextureIndex = currentPartsTextureIndex;
        player.playerStats.stats.name = inputName;
        PlayerBinarySave.SavePlayerData(player);

        #region Save to PlayerPrefs (REDUNDANT)
        /*
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
        #endregion
    }
    #endregion
}
