using UnityEngine;

/// <summary>
/// Code relating to player
/// </summary>

public class Player : MonoBehaviour
{
    public PlayerStats playerStats;

    //public float testHealth = 100;

    private void Update()
    {
        //playerStats.CurrentHealth = testHealth;
    }

    public void DealDamage(float damage)
    {
        playerStats.CurrentHealth -= damage;
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
    }

    //Save and Load moved to GameManager
}
