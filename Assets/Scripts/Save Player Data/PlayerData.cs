[System.Serializable]//makes it display in unity, + we can save it in a file.
public class PlayerData//load up PlayerData class with Data from our scene
{
    #region Variables
    /// These are all the variables that will be saved, this is also where unity specific variables
    /// are converted into C# standard variables so they may be translated into binary
    public Stats stats;
    public PlayerProfession profession;
    public PlayerRace race;
    public int[] customisationTextureIndex;

    public float[] position;//doesnt save vector 3's so we write our own position array to save vector3's(UNITY SPECIFIC) as float[](C# friendly)
    #endregion
    #region PlayerData Constructor
    /// <summary>
    /// Contructor(Set up for our class), in here we are telling the class how to get the data from the player
    /// Stores the player data in the above 3 variables
    /// </summary>
    /// <param name="player">Takes in data from our player, using the Player script</param>
    public PlayerData(Player player) // when method is same name as class, it is a constructor, shorthand ctor + TAB + TAB
    {
        stats = player.playerStats.stats;
        profession = player.Profession; //May need to change to profession not Profession
        race = player.Race;
        customisationTextureIndex = player.customisationTextureIndex;

        position = new float[3]; // 3 floats instanced, so basically a vector 3
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
    #endregion
    /*
    public Player GetPlayer()
    {
        Player player = new Player();

        player.playerStats.stats = stats;
        player.Profession = profession;

        return player;
    }
    */
}
