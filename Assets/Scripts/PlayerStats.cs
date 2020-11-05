using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BaseStats //like a class, but only stores variables, e.g. Vecotr 3 and color are structs
{
    public string baseStatName;
    public int defaultStat; //stat from the class
    public int levelUpStat; //stats that you gain on level up, so you can't respec them.
    public int additionalStat; //additional stat from

    //final stats will be
    //default + additional
    public int finalStat
    {
        get
        {
            return defaultStat + additionalStat + levelUpStat;
        }
    }
}

/// <summary>
/// Seperate into its own class as these are all savable unlike quater heart
/// which means we can just save Stats instead of saving each and every stat individually.
/// </summary>
[System.Serializable]
public class Stats
{
    #region Float Variables
    /// <summary>
    /// Stores the player stats
    /// </summary>

    [Header("Player Movement")]
    public float speed = 6f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 1.0f;


    [Header("Current Stats")]
    public int level;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float regenHealth = 5f;
    public float currentMana = 100f;
    public float maxMana = 100f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    public float regenStamina = 5f;
    #endregion

    [Header("Base Stats")]
    public int baseStatPoints = 10;
    public BaseStats[] baseStats;
}

[System.Serializable]
public class PlayerStats
{
    public Stats stats; //all the stats in the class above

    #region Stats Bars References
    [Header("Stats Bars References")]
    [SerializeField]
    private Text currentHealthText;
    [SerializeField]
    private Text maxHealthText;
    [SerializeField] 
    private Image healthFill;
    [SerializeField]
    private Text currentManaText;
    [SerializeField] 
    private Text maxManaText;
    [SerializeField] 
    private Image manaFill;
    [SerializeField] 
    private Text currentStaminaText;
    [SerializeField] 
    private Text maxStaminaText;
    [SerializeField] 
    private Image staminaFill;
    #endregion

    #region Star Bar Properties and QuaterHearts
    public QuarterHearts healthHearts;

    //field and property
    /// <summary>
    /// Current Health Property to clamp values and automate UI
    /// </summary>
    public float CurrentHealth //property
    {
        get
        {
            return stats.currentHealth;
        }
        set
        {
            stats.currentHealth = Mathf.Clamp(value,0,stats.maxHealth); //replaces the two if conditions below.
            /*if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
            if (currentHealth < 0)
            {
                currentHealth = 0;
                //die
            }*/
            

            currentHealthText.text = ((int)stats.currentHealth).ToString(); //round down to int for health text
            healthFill.fillAmount = stats.currentHealth / stats.maxHealth;

            #region quaterHearts
            if (healthHearts != null)
            {
                healthHearts.UpdateHearts(value, stats.maxHealth);
            }
            #endregion
        }
    }

    /// <summary>
    /// Current Mana Property to clamp values and automate UI
    /// </summary>
    public float CurrrentMana
    {
        get
        {
            return stats.currentMana;
        }
        set
        {
            stats.currentMana = Mathf.Clamp(value, 0, stats.maxMana);
            currentManaText.text = ((int)stats.currentMana).ToString(); //round down to int for health text
            manaFill.fillAmount = stats.currentMana / stats.maxMana;
        }
    }

    /// <summary>
    /// Current Stamina Property to clamp values and automate UI
    /// </summary>
    public float CurrrentStamina
    {
        get
        {
            return stats.currentStamina;
        }
        set
        {
            stats.currentStamina = Mathf.Clamp(value, 0, stats.maxStamina);
            currentStaminaText.text = ((int)stats.currentStamina).ToString(); //round down to int for health text
            staminaFill.fillAmount = stats.currentStamina / stats.maxStamina;
        }
    }
    #endregion

    public bool SetStats(int statIndex, int amount)
    {
        //increasing
        if (amount > 0 && stats.baseStatPoints - amount < 0)
        {
            //we can't add points if there are none left
            return false;
        }
        else if (amount < 0 && stats.baseStats[statIndex].additionalStat + amount < 0) //decreasing
        {
            //additionalStat must be 0 or positive int
            return false;
        }

        stats.baseStats[statIndex].additionalStat += amount;
        stats.baseStatPoints -= amount;
        return true;
    }

    /// <summary>
    /// At start of scene, stat bar values are updated to relevant values.
    /// </summary>
    public void InitializeStatBarsText()
    {
        currentHealthText.text = stats.currentHealth.ToString();
        maxHealthText.text = stats.maxHealth.ToString();
        currentManaText.text = stats.currentMana.ToString();
        maxManaText.text = stats.maxMana.ToString();
        currentStaminaText.text = stats.currentStamina.ToString();
        maxStaminaText.text = stats.maxStamina.ToString();
    }
}
