using UnityEngine;

[System.Serializable]
public class PlayerStats
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
    public float currentMana = 100f;
    public float maxMana = 100f;
    public float currentStamina = 100f;
    public float maxStamina = 100f;
    #endregion
    #region Property and QuaterHearts

    //field and property
    public float CurrentHealth //property
    {
        get
        {
            return currentHealth;
        }
        set
        {
            currentHealth = value;
            if (healthHearts != null)
            {
                healthHearts.UpdateHearts(value, maxHealth);
            }
        }
    }
    public QuarterHearts healthHearts;
    #endregion
}
