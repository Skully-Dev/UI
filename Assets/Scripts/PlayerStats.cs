using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStats
{
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

    //field and property
    private float _currentHealth = 100;//field
    public float CurrentHealth //property
    {
        get
        {
            return _currentHealth;
        }
        set
        {
            _currentHealth = value;
            if (healthHearts != null)
            {
                healthHearts.UpdateHearts(value, maxHealth);
            }
        }
    }
    public QuarterHearts healthHearts;
}
