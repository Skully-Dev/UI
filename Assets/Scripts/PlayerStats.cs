using UnityEngine;

[System.Serializable]
public struct BaseStats //like a class, but only stores variables, Vecotr 3 and color are structs
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

    #region Property and QuaterHearts

    //field and property
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
            if (healthHearts != null)
            {
                healthHearts.UpdateHearts(value, stats.maxHealth);
            }
        }
    }
    public QuarterHearts healthHearts;
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
}
