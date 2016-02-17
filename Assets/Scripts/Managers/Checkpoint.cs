using UnityEngine;
using System.Collections;

public class Checkpoint : LightSource
{

    private new Transform transform;

    public override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
        transform = GetComponent<Transform>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PlayerData data = new PlayerData();
            data.playerPosition = DataManager.Vector3ToString(other.gameObject.transform.position);
            data.playerScale = DataManager.Vector3ToString(other.gameObject.transform.localScale);
            data.playerEnergy = other.gameObject.GetComponent<Player>().LightEnergy.CurrentEnergy;
            data.levelName = other.gameObject.GetComponent<Player>().LevelName;
            DataManager.SaveFile(data);
        }
        //Destroy(gameObject);
    }

}


