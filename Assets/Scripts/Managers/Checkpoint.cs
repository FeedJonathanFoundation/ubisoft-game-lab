using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Parse;

public class Checkpoint : MonoBehaviour {

    // corresponds to player data
    public float playerLight;
    public int level;

	// Use this for initialization
	void Start() {
	   Save();
	}
	
	// Update is called once per frame
	void Update() {
	
	}
    
    /// <summary>
    /// TO REVIEW
    /// Executed before Start()
    /// Makes sure that there's only 1 instance of the gameObject
    /// across different scenes.
    /// </summary>
    void Awake() 
    {
        // if (control == null) 
        // {
        //     DontDestroyOnLoad(gameObject);
        //     control = this;
        // } 
        // else if (control != this) 
        // {
        //     Destroy(gameObject);
        // }    
    }
   
	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
		  Save();
		}		
		Destroy (gameObject); // destoy checkpoint once it's been collected
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
        gameControl["playerLight"] = 9999;
        gameControl["level"] = 5;
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

 /// <summary>
 /// Model holding player data
 /// </summary>
[Serializable]
class PlayerData
{
    public float playerLight;
    public int level;
}
