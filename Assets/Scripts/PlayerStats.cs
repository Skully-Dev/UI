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
    public float levelModifier = 1f;
    public float healthModifier;
    public float manaModifier;
    public float staminaModifier;
    public float currentHealth = 100f;
    public float maxHealth = 100f;
    public float regenHealth = 5f;
    public float currentMana = 100f;
    public float maxMana = 100f;
    public float regenMana = 5f;
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
    public float MaxHealth
    {
        get
        {
            return stats.maxHealth;
        }
        set
        {
            stats.maxHealth = value;
            stats.regenHealth = value * 0.05f;
            if (CurrentHealth > value)
            {
                CurrentHealth = value;
            }
        }
    }

    /// <summary>
    /// Current Mana Property to clamp values and automate UI
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

    public float MaxMana
    {
        get
        {
            return stats.maxMana;
        }
        set
        {
            stats.maxMana = value;
            stats.regenMana = value * 0.05f;
            if (CurrentMana > value)
            {
                CurrentMana = value;
            }
        }
    }

    /// <summary>
    /// Current Stamina Property to clamp values and automate UI
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

    public float MaxStamina
    {
        get
        {
            return stats.maxStamina;
        }
        set
        {
            stats.maxStamina = value;
            stats.regenStamina = value * 0.05f;

            if (CurrentStamina > value)
            {
                CurrentStamina = value;
            }
        }
    }
    #endregion

    public float CalculateRegen(float statMaxValue)
    {
        return statMaxValue * 0.05f;
    }



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

    public void UpdateModifiers()
    {
        UpdateHealthModifier();
        UpdateManaModifier();
        UpdateStaminaModifier();
    }

    public void UpdateHealthModifier()
    {
        //1 per strength
        stats.healthModifier = stats.baseStats[0].finalStat;
        //0.5 per Constitution
        stats.healthModifier += stats.baseStats[2].finalStat * 0.5f;
        //0.5 per Wisdom
        stats.healthModifier += stats.baseStats[4].finalStat * 0.5f;

        //half the impact
        stats.healthModifier *= 0.5f;

        //scale by level
        stats.healthModifier *= stats.levelModifier;

        MaxHealth = (int)(60 + stats.healthModifier);
    }

    public void UpdateManaModifier()
    {
        //1 per Intelligence
        stats.manaModifier = stats.baseStats[3].finalStat;
        //0.5 per Wisdom
        stats.manaModifier += stats.baseStats[4].finalStat * 0.5f;
        //0.5 per Charisma
        stats.manaModifier += stats.baseStats[5].finalStat * 0.5f;

        //half the impact
        stats.manaModifier *= 0.5f;

        //scale by level
        stats.manaModifier *= stats.levelModifier;

        MaxMana = (int)(60 + stats.manaModifier);
    }

    public void UpdateStaminaModifier()
    {
        //1 per Dexterity
        stats.staminaModifier = stats.baseStats[1].finalStat;
        //0.5 per Constitution
        stats.staminaModifier += stats.baseStats[2].finalStat * 0.5f;
        //0.5 per Charisma
        stats.staminaModifier += stats.baseStats[5].finalStat * 0.5f;

        //half the impact
        stats.staminaModifier *= 0.5f;

        //scale by level
        stats.staminaModifier *= stats.levelModifier;

        MaxStamina = (int)(60 + stats.staminaModifier);
    }

    public void RefillStatBars()
    {
        CurrentHealth = stats.maxHealth;
        CurrentMana = stats.maxMana;
        CurrentStamina = stats.maxStamina;
    }
}
