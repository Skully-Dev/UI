using UnityEngine;

/// <summary>
/// Code relating to player
/// </summary>

public class Player : MonoBehaviour
{
    public PlayerStats playerStats;

    public float testHealth = 100;

    private void Update()
    {
        playerStats.CurrentHealth = testHealth;
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

}

/*
  public int level = 3;
    public int health = 55;
    //move(); //basically the actual character

    //for testing
    public void Save()
    {
        SaveSystem.SavePlayer(this); //you need to pass it a reference of player to save
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        level = data.level;
        health = data.health;
        Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.position = pos;
    }
 */
