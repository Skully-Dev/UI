using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region  Player Reference and Load On Load Conditional
    [SerializeField]
    [Tooltip("Reference Player so player values can be saved.")]
    private Player player;

    private void Start()
    {
        if (PlayerPrefs.GetInt("LoadOnLoad") == 1) //1=true, 0=false
        {
            Load();
            PlayerPrefs.SetInt("LoadOnLoad", 0);
        }
    }
    #endregion
    #region Save and Load Methods
    /// <summary>
    /// Sends player to be converted into PlayerData to be converted into binary and saved off in a file.
    /// </summary>
    public void Save()
    {
        SaveSystem.SavePlayer(player); //you need to pass it a reference of player to save
    }

    /// <summary>
    /// Loads save file converts it from binary into PlayerData then is applied to player.
    /// </summary>
    public void Load()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        player.playerStats.stats = data.stats;

        Vector3 pos = new Vector3(data.position[0], data.position[1], data.position[2]); //transfers float array into Vector3(for unity)
        //controller.enabled = false;
        player.gameObject.transform.position = pos;
        Physics.SyncTransforms();
        //controller.enabled = true;
    }
    #endregion
}
