using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Parse;

 /// <summary>
 /// Data Manager contains static methods to read and write PlayerData to disk
 /// TODO extend beyond player data and implement loading from Parse
 /// </summary>
public static class DataManager
{
    private static String fileName = ".playerInfo.dat";

    public static void SaveFile(PlayerData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Create);
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Saved!");
    }

    public static PlayerData LoadFile()
    {
        if (File.Exists(Application.persistentDataPath + fileName))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + fileName, FileMode.Open);
            PlayerData data = (PlayerData) bf.Deserialize(file);
            file.Close();
            Debug.Log("Loaded");
            return data;
        }
        else
        {
            Debug.Log("No saved progress! File does not exist :(");
            return null;
        }
    }


    /// <summary>
    /// Utility method - converts Vector3 objects to strings.
    /// Useful to allow serialization Vector3 objects.
    /// </summary>
    public static string Vector3ToString(Vector3 v)
    {
        return string.Format("{0:0.00},{1:0.00},{2:0.00}", v.x, v.y, v.z);
    }

    /// <summary>
    /// Utility method - converts strings to Vector3 objects.
    /// Useful to allow DEserialization of Vector3 objects.
    /// </summary>
    public static Vector3 Vector3FromString(String s)
    {
        string[] parts = s.Split(new string[] { "," }, StringSplitOptions.None);
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    /// <summary>
    /// Save to Parse
    /// NOTE: currently not used
    /// </summary>
    public static void Save(PlayerData data)
    {
        ParseObject gameControl = new ParseObject("Player");
        gameControl["playerID"] = data.playerID;
        gameControl["playerEnergy"] = data.playerEnergy;
        gameControl["playerPosition"] = data.playerPosition;
        gameControl["playerScale"] = data.playerScale;
        gameControl.SaveAsync();
        Debug.Log("Game Saved!");
    }

    /// <summary>
    /// Load from Parse
    /// NOTE: currently not used
    /// </summary>
    public static PlayerData Load()
    {
        ParseQuery<ParseObject> query = ParseObject.GetQuery("Player");
        query.GetAsync("ZOPe3i9KbM").ContinueWith(t =>
        {
            ParseObject gameScore = t.Result;
            PlayerData data = new PlayerData();
            data.playerPosition = gameScore["playerPosition"].ToString();
            data.playerScale = gameScore["playerScale"].ToString();
            data.playerEnergy = float.Parse(gameScore["playerEnergy"].ToString());
            return data;
        });
        return null;
    }

}

 /// <summary>
 /// Model holding player data
 /// DATA to be changed
 /// </summary>
[Serializable]
public class PlayerData
{
    public float playerID;
    public float playerEnergy;
    public String playerPosition;
    public String playerScale;
    public String levelName;
}