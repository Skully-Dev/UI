[System.Serializable]//makes it display in unity
public class PlayerData//load up PlayerData class with Data from our scene
{
    public int level;
    public int health;
    public float[] position;//doesnt save vector 3's so we write our own position array to save vector3's(UNITY SPECIFIC) as float[](C# friendly)
    // Start is called before the first frame update
    public PlayerData(Player player) // when method is same name as class, it is a constructor, shorthand ctor + TAB + TAB
    {
        /*
        level = player.level;
        health = player.health;

        position = new float[3]; // 3 floats initialized, so basically a vector 3
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        */
    }
}
