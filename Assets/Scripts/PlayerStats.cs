using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BaseStats //like a class, but only stores variables, e.g. Vecotr 3 and color are structs
{
    [Tooltip("The Stat name, i.e. Strength, Dexterity, Constitution, Intelligence, Wisdom, Charisma")]
    public string baseStatName;
    [Tooltip("Locked Class base stat points")]
    public int defaultStat; //stat from the class
    [Tooltip("Locked level up stat points")]
    public int levelUpStat; //stats that you gain on level up, so you can't respec them.
    [Tooltip("Stat points assigned by user, RESPEC-ABLE")]
    public int additionalStat; //additional stat from initial stat pool AND from customisable level up bonus


    /// <summary>
    /// final stat is default + levelUp + additional
    /// </summary>
    public int FinalStat
    {
        get
        {
            return defaultStat + additionalStat + levelUpStat;
        }
    }
}

/// <summary>
/// Seperate into its own class as these are all savable
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
    [Tooltip("Players Walking Speed")]
    public float speed = 6f;
    [Tooltip("Players Running Speed")]
    public float sprintSpeed = 12f;
    [Tooltip("Players Movement while Crouched Speed")]
    public float crouchSpeed = 3f;
    [Tooltip("Players Jump Height")]
    public float jumpHeight = 1.0f;


    [Header("Current Stats")]
    [Tooltip("What is the players level")]
    public int level;
    [Tooltip("Scales stat points effectiveness")]
    public float levelModifier = 1f;
    [Tooltip("Additional health determined by stat points")]
    public float healthModifier;
    [Tooltip("Additional mana determined by stat points")]
    public float manaModifier;
    [Tooltip("Additional stamina determined by stat points")]
    public float staminaModifier;
    [Tooltip("health remaining")]
    public float currentHealth = 100f;
    [Tooltip("full health value")]
    public float maxHealth = 100f;
    [Tooltip("The rate of which health regenerates")]
    public float regenHealth = 5f;
    [Tooltip("mana remaining")]
    public float currentMana = 100f;
    [Tooltip("full mana value")]
    public float maxMana = 100f;
    [Tooltip("The rate of which mana regenerates")]
    public float regenMana = 5f;
    [Tooltip("stamina remaining")]
    public float currentStamina = 100f;
    [Tooltip("full stamina value")]
    public float maxStamina = 100f;
    [Tooltip("The rate of which stamina regenerates")]
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
    /// Property to clamp values and automate UI, also does quater hearts.
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
    /// Property to cap current value to max. Also automates regen value
    /// </summary>
    public float MaxHealth
    {
        get
        {
            return stats.maxHealth;
        }
        set
        {
            stats.maxHealth = value;
            stats.regenHealth = CalculateRegen(value, 0.05f);
            if (CurrentHealth > value)
            {
                CurrentHealth = value;
            }
        }
    }

    /// <summary>
    /// Property to clamp values and automate UI
    /// </summary>
    public float CurrentMana
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
    /// Property to cap current value to max. Also automates regen value
    /// </summary>
    public float MaxMana
    {
        get
        {
            return stats.maxMana;
        }
        set
        {
            stats.maxMana = value;
            stats.regenMana = CalculateRegen(value, 0.05f);
            if (CurrentMana > value)
            {
                CurrentMana = value;
            }
        }
    }

    /// <summary>
    /// Property to clamp values and automate UI.
    /// </summary>
    public float CurrentStamina
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

    /// <summary>
    /// Property to cap current value to max. Also automates regen value
    /// </summary>
    public float MaxStamina
    {
        get
        {
            return stats.maxStamina;
        }
        set
        {
            stats.maxStamina = value;
            stats.regenStamina = CalculateRegen(value, 0.05f);

            if (CurrentStamina > value)
            {
                CurrentStamina = value;
            }
        }
    }
    #endregion

    /// <summary>
    /// Sets the appropriate regen rate.
    /// </summary>
    /// <param name="statMaxValue"></param>
    /// <returns></returns>
    public float CalculateRegen(float statMaxValue, float regenRate)
    {
        return statMaxValue * regenRate;
    }


    /// <summary>
    /// Used to adjust any stat point value within its limits and stat points remaining accordingly.
    /// </summary>
    /// <param name="statIndex">What stat is being adjusted.</param>
    /// <param name="amount">By how much is it being adjusted.</param>
    /// <returns></returns>
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
    /// Updates stat bars values and visuals based on players stat points.
    /// </summary>
    public void UpdateStatBars()
    {
        UpdateModifiers();
        currentHealthText.text = stats.currentHealth.ToString();
        maxHealthText.text = stats.maxHealth.ToString();
        currentManaText.text = stats.currentMana.ToString();
        maxManaText.text = stats.maxMana.ToString();
        currentStaminaText.text = stats.currentStamina.ToString();
        maxStaminaText.text = stats.maxStamina.ToString();
    }

    /// <summary>
    /// Runs all 3 modifier updaters, determines: Max + level up bonus + Regeneration, values of Heath, Stamina and Mana.
    /// </summary>
    public void UpdateModifiers()
    {
        UpdateHealthModifier();
        UpdateManaModifier();
        UpdateStaminaModifier();
    }

    /// <summary>
    /// Based on stat point distribution, Sets Max + level up bonus + Regeneration for Health.
    /// </summary>
    public void UpdateHealthModifier()
    {
        //1 per strength
        stats.healthModifier = stats.baseStats[0].FinalStat;
        //0.5 per Constitution
        stats.healthModifier += stats.baseStats[2].FinalStat * 0.5f;
        //0.5 per Wisdom
        stats.healthModifier += stats.baseStats[4].FinalStat * 0.5f;

        //half the impact
        stats.healthModifier *= 0.5f;

        //scale by level
        stats.healthModifier *= stats.levelModifier;

        //Applies a whole value to Max which also then applies a value to regen
        MaxHealth = (int)(60 + stats.healthModifier);
    }

    /// <summary>
    /// Based on stat point distribution, Sets Max + level up bonus + Regeneration for Mana.
    /// </summary>
    public void UpdateManaModifier()
    {
        //1 per Intelligence
        stats.manaModifier = stats.baseStats[3].FinalStat;
        //0.5 per Wisdom
        stats.manaModifier += stats.baseStats[4].FinalStat * 0.5f;
        //0.5 per Charisma
        stats.manaModifier += stats.baseStats[5].FinalStat * 0.5f;

        //half the impact
        stats.manaModifier *= 0.5f;

        //scale by level
        stats.manaModifier *= stats.levelModifier;

        //Applies a whole value to Max which also then applies a value to regen
        MaxMana = (int)(60 + stats.manaModifier);
    }

    /// <summary>
    /// Based on stat point distribution, Sets Max + level up bonus + Regeneration for Stamina.
    /// </summary>
    public void UpdateStaminaModifier()
    {
        //1 per Dexterity
        stats.staminaModifier = stats.baseStats[1].FinalStat;
        //0.5 per Constitution
        stats.staminaModifier += stats.baseStats[2].FinalStat * 0.5f;
        //0.5 per Charisma
        stats.staminaModifier += stats.baseStats[5].FinalStat * 0.5f;

        //half the impact
        stats.staminaModifier *= 0.5f;

        //scale by level
        stats.staminaModifier *= stats.levelModifier;

        //Applies a whole value to Max which also then applies a value to regen
        MaxStamina = (int)(60 + stats.staminaModifier);
    }

    /// <summary>
    /// Sets Health, Stamina and Mana to max.
    /// </summary>
    public void RefillStatBars()
    {
        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
        CurrentStamina = MaxStamina;
    }
}
