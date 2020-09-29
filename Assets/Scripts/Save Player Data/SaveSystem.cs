using UnityEngine;
using System.IO;//Allows us to work with files on our operating system, used when opening and saving file
using System.Runtime.Serialization.Formatters.Binary; //C# thing, this is why we need to use floats instead of vectors

public class SaveSystem : MonoBehaviour
{
    //Can be used when and how you want, save at end of level/every 10 mins/ on exit / on events like buttons
    public static void SavePlayer(Player player) //static allows saving from any point in our game without needing an instance
    {
        BinaryFormatter formatter = new BinaryFormatter();//creating a binary formatter, converts our classes to binary text

        //the way you save is character by character, so this, like a steam
        FileStream stream = new FileStream(Path(), FileMode.Create); //opens the file to be written to

        //Since we created the contructor  in PlayerData, and we are passing through player, it will set up itself
        PlayerData data = new PlayerData(player); //creates the data to be saved, converting to PlayerData as that can easily be saved

        //writes the data to the file(also converts the data to text)
        formatter.Serialize(stream, data); //converting class to characters, saving the file
        stream.Close();//makes sure to close off the file stream
    }

    public static PlayerData LoadPlayer()
    {
        string path = Path();

        if (File.Exists(path)) //if the file exists
        {
            //load data, a lot of similar steps to save
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open); //.Open as were loading, not saving

            PlayerData data = formatter.Deserialize(stream) as PlayerData; //using that open file, since we serialized(Binary), we deserialize (which makes an Object) and convert to player data
            stream.Close();//close the file

            return data;
        }
        else
        {
            //Couldn't load
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    /// <summary>
    /// A string of the path we want, where to save, currently unity assets folder
    /// </summary>
    /// <returns>path as string</returns>
    private static string Path()
    {
        //"c:/User/*user*/Documents/*Asssets location*/player.sav"
        //string path = Application.persistentDataPath + "/player.sav"; //where persistantDataPath saves to computer somewhere
        //extention DOESNT matter, can be .sav, or any other thing like .meme
        return Application.dataPath + "/player.skully"; // saves to unity assets folder // A string of the path we want, where to save

    }
}
