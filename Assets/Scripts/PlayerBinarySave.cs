using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class PlayerBinarySave : MonoBehaviour
{
    public static void SavePlayerData(Player player)
    {
        //reference to a binary formatter
        BinaryFormatter formatter = new BinaryFormatter();
        //Loctaion to save //data path saves into the game folder //vs persistant the same location no matter where the game folder is located, also means uninstalls WONT delete save file.
        string path = Application.dataPath + "/playerSave.skully";
        //Create file at file path
        FileStream stream = new FileStream(path, FileMode.Create);

        //convert to a class we can actually save
        PlayerData data = new PlayerData(player);

        //Serialize player and save it to file
        formatter.Serialize(stream, data);
        //close the file
        stream.Close();
    }

    public static PlayerData LoadPlayerData()
    {
        //Location to load from
        string path = Application.dataPath + "/playerSave.skully";
        //if we have a file at that path
        if (File.Exists(path))
        {
            //get the binary formatter
            BinaryFormatter formatter = new BinaryFormatter();
            //read the data from the path
            FileStream stream = new FileStream(path, FileMode.Open);
            //Deserialize back to a usable variable
            PlayerData data = (PlayerData) formatter.Deserialize(stream);


            //close the file
            stream.Close();

            return data;
        }
        return null;
    }

}
