using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Player player;

    //for testing
    public void Save()
    {
        SaveSystem.SavePlayer(player); //you need to pass it a reference of player to save
    }

    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.playerStats.level = data.level;
        player.playerStats.currentHealth = data.currentHealth;
        Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]); //transfers float array into Vector3(for unity)
        player.gameObject.transform.position = pos;
    }
}
