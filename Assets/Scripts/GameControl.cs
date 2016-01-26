using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Parse;

/// <summary>
///  This class keeps track of the game in progress.
/// </summary>
public class GameControl : MonoBehaviour
{

    public static GameControl control;

    public float playerLight;
    public int level;

    
    /// <summary>
    ///  Executed before Start()
    /// Makes sure that there's only 1 instance of the gameObject
    /// across different scenes.
    /// </summary>
    void Awake() 
    {
        if (control == null) 
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        } 
        else if (control != this) 
        {
            Destroy(gameObject);
        }    
    }
    
    void Start()
    {
        this.Save();

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void Save()
    {
        // save locally
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + ".playerInfo.dat");
        
        PlayerData data = new PlayerData();
        data.playerLight = playerLight;
        data.level = level;
        bf.Serialize(file, data);
        file.Close();
        
        // save online
        ParseObject gameControl = new ParseObject("GameControl");
        gameControl["playerLight"] = 90;
        gameControl["level"] = 1;
        gameControl.SaveAsync();
        
    }
    
    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "playerInfo.dat")) 
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + ".playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData) bf.Deserialize(file);
            file.Close();
            
            playerLight = data.playerLight; 
            level = data.level;          
        }
    }
    
}

[Serializable]
class PlayerData
{
    public float playerLight;
    public int level;
}
