using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : LightSource
{
    [SerializeField]
    [Tooltip("If set to true, this checkpoint will teleport player to the next scene")]
    private bool changeScene = false;
    
    protected override void Awake()
    {       
        base.Awake(); // call parent LightSource Awake() first
        this.InfiniteEnergy = true; // override default LightSource value
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.gameObject.GetComponent<Player>();
            
            PlayerData data = new PlayerData();
            
            if (changeScene)
            {
                // if checkpoint changes scene, save values for the new scene
                data.playerPosition = DataManager.Vector3ToString(new Vector3(0, 0, 0));
                data.levelID = player.CurrentLevel + 1;                    
            } 
            else
            {
                data.playerPosition = DataManager.Vector3ToString(other.gameObject.transform.position);
                data.levelID = player.CurrentLevel;    
            }            
            
            data.playerRotation = DataManager.Vector3ToString(other.gameObject.transform.localEulerAngles);
            data.playerScale = DataManager.Vector3ToString(other.gameObject.transform.localScale);
            data.playerEnergy = other.gameObject.GetComponent<Player>().LightEnergy.CurrentEnergy;            
            DataManager.SaveFile(data);    
            
            if (changeScene)
            {
                ChangeLevel(player.CurrentLevel + 1);
                player.CurrentLevel = player.CurrentLevel + 1;
            }        
        }
    }
    
    private void ChangeLevel(int levelID)
    {
        if (SceneManager.sceneCountInBuildSettings > levelID)
        {
            SceneManager.LoadScene(levelID, LoadSceneMode.Single);
            Destroy(this.gameObject); // destroys checkpoint to prevent it from appearing in the following scene
        }
    }

}


