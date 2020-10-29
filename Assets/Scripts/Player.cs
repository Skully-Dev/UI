using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Code relating to player
/// </summary>
public class Player : MonoBehaviour
{
    public PlayerStats playerStats;

    #region maybe relocate to player stats
    private bool disableRegen = false;
    private float disableRegenTime;
    public float RegenCooldown = 5f;

    public float disableStaminaRegenTime;
    public float StaminaRegenCooldown = 1f;
    public float StaminaDegen = 12f;
    #endregion

    public int[] customisationTextureIndex;

    [SerializeField]
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

    private void Awake()
    {
        //Load player data
        if (SceneManager.GetActiveScene().name != "Customise")
        {
            PlayerData loadedPlayer = PlayerBinarySave.LoadPlayerData();
            if (loadedPlayer != null)
            {
                playerStats.stats = loadedPlayer.stats;
                profession = loadedPlayer.profession;
                customisationTextureIndex = loadedPlayer.customisationTextureIndex;
            }
        }
    }

    public void ChangeProfession(PlayerProfession cProfession)
    {
        profession =  cProfession;
        SetUpProfession();
    }

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

    public void LevelUp()
    {
        //playerStats.
        //increase stats every level
        playerStats.stats.baseStatPoints += 3; //3 additional points in point poll to use

        for (int i = 0; i < playerStats.stats.baseStats.Length; i++)
        {
            playerStats.stats.baseStats[i].levelUpStat += 1; //adds 1 to each stat
        }
    }

    //public float testHealth = 100;

    private void Update()
    {
        #region Health Regen
        if (!disableRegen)
        {
            //playerStats.CurrentHealth = testHealth;
            if (playerStats.CurrentHealth < playerStats.stats.maxHealth)
            {
                playerStats.CurrentHealth += playerStats.stats.regenHealth * Time.deltaTime;
            }
        }
        else
        {
            if (Time.time > disableRegenTime + RegenCooldown)
            {
                disableRegen = false;
            }
        }
        #endregion

        #region Stamina Regen
        if (Time.time > disableStaminaRegenTime + StaminaRegenCooldown)
        {
            if (playerStats.stats.currentStamina < playerStats.stats.maxStamina)
            {
                playerStats.stats.currentStamina += playerStats.stats.regenStamina * Time.deltaTime;
            }
            else
            {
                playerStats.stats.currentStamina = playerStats.stats.maxStamina; //clamp to max like health is in Current Health property.
            }
        }
        #endregion
    }

    public void DealDamage(float damage)
    {
        playerStats.CurrentHealth -= damage;
        disableRegen = true;
        disableRegenTime = Time.time;
    }

    public void Heal(float health)
    {
        playerStats.CurrentHealth += health;
    }

    private void OnGUI()
    {
        //Display our health
        //display our current mana
        //display our current stamina
        if (GUI.Button(new Rect(150,0,100,20), "Level Up"))
        {
            LevelUp();
        }

        if (GUI.Button(new Rect(130, 40, 100, 20), "Do Damage: " + playerStats.CurrentHealth))
        {
            DealDamage(25f);
        }
    }

    //Save and Load moved to GameManager
}
