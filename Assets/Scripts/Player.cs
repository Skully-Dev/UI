using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Code relating to player
/// </summary>
public class Player : MonoBehaviour
{
    public PlayerStats playerStats;

    [Tooltip("Is the player dead. Stops pause from opening after death.")]
    public bool isDead;

    #region Other Player Stats (maybe relocate to player stats)
    [Header("Stats Bars Regeneration/debuff variables")]
    [Tooltip("The time of when player was last damaged + cooldown")]
    private float healthRegenStartTime;
    [Tooltip("Delay between health loss and regeneration")]
    public float HealthRegenCooldown = 5f;

    [Tooltip("The time of when mana was last used + cooldown")]
    public float manaRegenStartTime;
    [Tooltip("Delay between mana use and regeneration")]
    public float ManaRegenCooldown = 5f;

    [Tooltip("The time of when stamina was last used + cooldown")]
    public float staminaRegenStartTime;
    [Tooltip("Delay between stamina use and regeneration")]
    public float StaminaRegenCooldown = 1f;
    [Tooltip("Rate stamina drops when running")]
    public float StaminaDegen = 12f;
    #endregion

    [Tooltip("The index values of the players saved chosen textures")]
    public int[] customisationTextureIndex;

    #region Race/Profession Properties/References
    [SerializeField, Tooltip("What Profession the Player themselves are.")]
    private PlayerProfession profession;

    public PlayerProfession Profession
    {
        get
        {
            return profession;
        }
        set
        {
            ChangeProfession(value);
        }
    }

    [SerializeField, Tooltip("What Race the Player themselves are.")]
    private PlayerRace race;

    public PlayerRace Race
    {
        get
        {
            return race;
        }
        set
        {
            ChangeRace(value);
        }
    }
    #endregion

    #region Death/Respawn Reference Variables
    [Header("Death/Respawn references")]
    [SerializeField, Tooltip("Reference player damaged sound FX")]
    private AudioSource damageSound;
    [SerializeField, Tooltip("Reference on death sound FX")]
    private AudioSource deathSound;
    [SerializeField, Tooltip("Reference death screen black background")]
    private GameObject deathScreen;
    [SerializeField, Tooltip("Reference death screen title text (You died...)")]
    private GameObject deathTitle;
    [SerializeField, Tooltip("Reference death screen button group")]
    private GameObject deathMenu;
    [SerializeField, Tooltip("Reference game music, to start again after respawn")]
    private AudioSource backgroundMusic;
    [SerializeField, Tooltip("Reference sound to be played on respawn")]
    private AudioSource respawnSound;
    [SerializeField, Tooltip("Reference empty Game Object that represents the location to respawn after death")]
    private GameObject respawnLocation;
    #endregion

    #region Player Info References
    [Header("Player Info References")]
    [SerializeField]
    private Text nameText;
    [SerializeField]
    private Text classText;
    [SerializeField]
    private Text abilityText;
    #endregion

    //awake happens before start and some values are better initialized before others.
    private void Awake()
    {
        if (SceneManager.GetActiveScene().name != "Customise") //if not in Customise scene
        {
            LoadPlayer();
            UpdatePlayerCardInfo();
        }
    }

    private void Update()
    {
        #region Health Regen
        if (Time.time > healthRegenStartTime) //check if debuff time is over
        {
            if (playerStats.CurrentHealth < playerStats.MaxHealth) //if health is less than max
            {
                playerStats.CurrentHealth += playerStats.stats.regenHealth * Time.deltaTime; //Increases current health PROPERTY value (automates UI updates)
            }
        }
        #endregion

        #region Mana Regen
        if (Time.time > manaRegenStartTime) //check if debuff time is over
        {
            if (playerStats.CurrentMana < playerStats.MaxMana) //if mana not full
            {
                playerStats.CurrentMana += playerStats.stats.regenMana * Time.deltaTime; //Increases current mana PROPERTY value (automates UI updates)
            }
        }
        #endregion

        #region Stamina Regen
        if (Time.time > staminaRegenStartTime) //check if debuff time is over
        {
            if (playerStats.CurrentStamina < playerStats.MaxStamina) //if stamina not full
            {
                playerStats.CurrentStamina += playerStats.stats.regenStamina * Time.deltaTime; //Increases current stamina PROPERTY value (automates UI updates)
            }
        }
        #endregion
    }

    #region Load + Save Methods
    /// <summary>
    /// Loads save file converts it from binary into PlayerData then is applied to player.
    /// </summary>
    public void LoadPlayer()
    {
        PlayerData loadedPlayer = PlayerBinarySave.LoadPlayerData();
        if (loadedPlayer != null)
        {
            playerStats.stats = loadedPlayer.stats;
            profession = loadedPlayer.profession;
            race = loadedPlayer.race;
            customisationTextureIndex = loadedPlayer.customisationTextureIndex;

            Vector3 pos = new Vector3(loadedPlayer.position[0], loadedPlayer.position[1], loadedPlayer.position[2]); //transfers float array into Vector3(for unity)
            
            //controller.enabled = false;
            transform.position = pos;
            Physics.SyncTransforms();
            //controller.enabled = true;

            playerStats.UpdateStatBars();
        }
    }

    /// <summary>
    /// Sends player to be converted into PlayerData to be converted into binary and saved off in a file.
    /// </summary>
    public void SavePlayer()
    {
        PlayerBinarySave.SavePlayerData(this); //you need to pass it a reference of player to save
    }
    #endregion

    #region Change Race/Profession Methods
    /// <summary>
    /// Method run by property when profession is changed.
    /// This enables the code to make the appropriate changes to the Player Attributes with a Method Stack.
    /// Sets chosen profession to current profession.
    /// Applies the default stat points based on that profession
    /// </summary>
    /// <param name="cProfession">The profession being changed to.</param>
    public void ChangeProfession(PlayerProfession cProfession)
    {
        profession =  cProfession;
        SetUpProfession();
        //May still need to do something for ability
    }

    /// <summary>
    /// Method run by property when Race is changed.
    /// This enables the code to make the appropriate changes to the Player Attributes with a Method Stack.
    /// Sets chosen race to current race.
    /// </summary>
    /// <param name="cRace"></param>
    public void ChangeRace(PlayerRace cRace)
    {
        race = cRace;
        //may still need to do something for ability
    }

    /// <summary>
    /// Applies the Default Base Stats of the chosen profession.
    /// </summary>
    public void SetUpProfession()
    {
        for (int i = 0; i < playerStats.stats.baseStats.Length; i++)
        {
            if (i < profession.defaultStats.Length) //check if i exists in profession
            {
                playerStats.stats.baseStats[i].defaultStat = profession.defaultStats[i].defaultStat;
            }
        }
    }
    #endregion

    /// <summary>
    /// Increases all stat values by 1.
    /// Get 3 additional points to spend.
    /// One incrament of exponential level modifier.
    /// Updates stat bars.
    /// </summary>
    public void LevelUp()
    {
        //playerStats.
        //increase stats every level
        playerStats.stats.baseStatPoints += 3; //3 additional points in point poll to use

        for (int i = 0; i < playerStats.stats.baseStats.Length; i++)
        {
            playerStats.stats.baseStats[i].levelUpStat += 1; //adds 1 to each stat
        }
        //increase stat bonuses modifier
        playerStats.stats.levelModifier *= 1.2f;

        playerStats.UpdateStatBars();
    }

    #region Use Stat Bars Methods
    /// <summary>
    /// damage player, regen debuff, sound FX and if 0hp Die();
    /// </summary>
    /// <param name="damage">Amount of damage</param>
    public void DealDamage(float damage)
    {
        playerStats.CurrentHealth -= damage;
        healthRegenStartTime = Time.time + HealthRegenCooldown;
        damageSound.Play();
        if (playerStats.CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// increase health and any related effects
    /// </summary>
    /// <param name="health">How much hp to regain</param>
    public void Heal(float health)
    {
        playerStats.CurrentHealth += health;
    }

    /// <summary>
    /// use mana and any related effects code
    /// </summary>
    /// <param name="cost">mana to be spent</param>
    public void UseMana(float cost)
    {
        playerStats.CurrentMana -= cost;
        manaRegenStartTime = Time.time + ManaRegenCooldown;
    }
    #endregion

    #region Death + Respawn Methods
    /// <summary>
    /// Player death. isDead true, stops main music and plays sound FX, freeze gameplay and run death screen coroutine 
    /// </summary>
    public void Die()
    {
        isDead = true;
        Debug.Log("Oops, guess I died :O");
        Time.timeScale = 0f; //freeze gameplay

        deathSound.Play();
        backgroundMusic.Stop();
        StartCoroutine(DeathScreen());
    }
    
    /// <summary>
    /// Load death screen piece by piece at timed intervials.
    /// </summary>
    /// <returns></returns>
    IEnumerator DeathScreen()
    {
        deathScreen.SetActive(true);//fade screen to black
        yield return new WaitForSecondsRealtime(2.5f);
        deathTitle.SetActive(true); //you died... text fade-drops in.
        yield return new WaitForSecondsRealtime(3);
        deathMenu.SetActive(true); //buttons fade-drop in.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Player to respawn transforms with clean FULL stat bars, sound FX, hides cursor, toggles isDead.
    /// </summary>
    public void Respawn()
    {
        isDead = false;//toggle isDead to false (e.g. pause now works)
        Time.timeScale = 1f; //unfreeze gameplay
        Cursor.visible = false; //hide cursor

        #region Sounds
        deathSound.Stop(); //stop death sound FX
        backgroundMusic.Play(); //restart game music
        respawnSound.Play(); //respawn sound FX
        #endregion

        #region Respawn Position
        //set player transform to respawns
        transform.position = respawnLocation.transform.position;
        transform.rotation = respawnLocation.transform.rotation;
        Physics.SyncTransforms(); //apply those values
        #endregion

        #region Fresh start with full stat bars.
        playerStats.RefillStatBars();
        #endregion
    }
    #endregion

    /// <summary>
    /// Updates the Player Card Info displayed in the bottom right corner.
    /// With player customisation info.
    /// </summary>
    public void UpdatePlayerCardInfo()
    {
        nameText.text = playerStats.stats.name;
        classText.text = profession.ProfessionName + "-" + race.RaceName;
        abilityText.text = profession.AbilityName + " " + race.AbilityName;
    }

    private void OnGUI()
    {
        #region Test Buttons LevelUp UseMana DealDamage
        //level player up for testing
        if (GUI.Button(new Rect(150, 0, 100, 20), "Level Up"))
        {
            LevelUp();
        }

        //use 25 mana for testing
        if (GUI.Button(new Rect(150, 20, 100, 20), "Use Mana: " + playerStats.CurrentMana))
        {
            UseMana(25f);
        }

        //damage player 25hp for testing
        if (GUI.Button(new Rect(150, 40, 100, 20), "Do Damage: " + playerStats.CurrentHealth))
        {
            DealDamage(25f);
        }
        #endregion

        if (nearShop)
        {
            if (!alreadyOpen)
            {
                if (GUI.Button(new Rect(30, 1020, 120, 60), "Open Shop"))
                {
                    shop.OpenShopToggle();
                    alreadyOpen = true;
                    Time.timeScale = 0;
                }
            }
            else
            {
                if (GUI.Button(new Rect(30, 1020, 120, 60), "Exit Shop"))
                {
                    shop.OpenShopToggle();
                    alreadyOpen = false;
                    Time.timeScale = 1;
                }
            }
        }

        if (nearChest)
        {
            if (!alreadyOpen)
            {
                if (GUI.Button(new Rect(30, 1020, 120, 60), "Open Chest"))
                {
                    chest.OpenChestToggle();
                    alreadyOpen = true;
                    Time.timeScale = 0;
                }
            }
            else
            {
                if (GUI.Button(new Rect(30, 1020, 120, 60), "Exit Chest"))
                {
                    chest.OpenChestToggle();
                    alreadyOpen = false;
                    Time.timeScale = 1;
                }
            }
        }
    }

    Chest chest;
    Shop shop;
    bool nearShop = false;
    bool nearChest = false;
    bool alreadyOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Chest")
        {
            nearChest = true;
            chest = other.gameObject.GetComponent<Chest>();
        }

        if (other.gameObject.tag == "Shop")
        {
            nearShop = true;
            shop = other.gameObject.GetComponent<Shop>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        nearChest = false;
        nearShop = false;
    }
}
