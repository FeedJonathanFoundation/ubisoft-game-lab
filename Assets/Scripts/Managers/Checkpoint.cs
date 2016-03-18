using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Checkpoint class is responsible for behaviour related to Checkpoint object. 
/// It recharges player object with energy on collision and saves current progress.
///
/// Optionally, it switches scenes, to bring the player to the next level.
///
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
public class Checkpoint : LightSource
{
    [SerializeField]
    [Tooltip("If set to true, this checkpoint will teleport player to the next scene")]
    private bool changeScene = false;
    
    protected override void Awake()
    {       
        base.Awake();
        this.InfiniteEnergy = true; // override default LightSource value
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.name == "Player")
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
                StartCoroutine(ChangeLevel(player.CurrentLevel + 1));
                player.CurrentLevel = player.CurrentLevel + 1;                                                
            }        
        }
    }
    
    private IEnumerator ChangeLevel(int levelID)
    {
        if (SceneManager.sceneCountInBuildSettings > levelID)
        {
            // Apply camera fade while loading the next scene
            float fadeTime = GameObject.Find("Main Camera").GetComponent<Fade>().BeginFade(1);                        
            yield return new WaitForSeconds(fadeTime);
            SceneManager.LoadScene(levelID, LoadSceneMode.Single);            
            // Destroy checkpoint to prevent it from appearing in the next scene
            Destroy(this.gameObject); 
        }
    }

}


