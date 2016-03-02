using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : LightSource
{
    public override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            int currentLevel = other.gameObject.GetComponent<Player>().CurrentLevel;
            PlayerData data = new PlayerData();
            data.playerPosition = DataManager.Vector3ToString(other.gameObject.transform.position);
            data.playerRotation = DataManager.Vector3ToString(other.gameObject.transform.localEulerAngles);
            data.playerScale = DataManager.Vector3ToString(other.gameObject.transform.localScale);
            data.playerEnergy = other.gameObject.GetComponent<Player>().LightEnergy.CurrentEnergy;
            data.levelID = currentLevel;
            DataManager.SaveFile(data);
            ChangeLevel(currentLevel + 1);            
        }
    }
    
    private void ChangeLevel(int levelID)
    {
        if (SceneManager.sceneCountInBuildSettings > levelID)
        {
            SceneManager.LoadScene(levelID, LoadSceneMode.Single);
        }
    }

}


