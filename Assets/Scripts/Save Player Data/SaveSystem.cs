using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary; //C# thing, this is why we need to use floats instead of vectors

public class SaveSystem : MonoBehaviour
{
    public static void SavePlayer(Player player) //static allows saving from any point in our game
    {
        BinaryFormatter formatter = new BinaryFormatter();//creating a binary formatter, converts our classes to binary text

        //"c:/User/*user*/Documents/*Asssets location*/player.sav"
        //string path = Application.persistentDataPath + "/player.sav"; //where persistantDataPath saves to computer somewhere
        string path = Application.dataPath + "/player.meme"; // saves to unity assets folder // A string of the path we want, where to save

        //the way you save is character by character, so this, like a steam
        FileStream stream = new FileStream(path, FileMode.Create); //opens the file to be written to

        PlayerData data = new PlayerData(player); //creates the data to be saved, converting to PlayerData as that can easily be saved

        //writes the data to the file(also converts the data to text)
        formatter.Serialize(stream, data); //converting class to characters, saving the file
        stream.Close();//makes sure to close off the file stream
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.dataPath + "/player.meme"; //extention DOESNT matter, can be .sav, or any other thing like .meme

        if (File.Exists(path))
        {
            //load data
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open); //as were loading, not saving

            PlayerData data = formatter.Deserialize(stream) as PlayerData; //using that open file, since we serialized, we deserialize (which makes an Object) and convert to player data
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
}
