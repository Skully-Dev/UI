using UnityEngine;

public class Player : MonoBehaviour
{
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
}
