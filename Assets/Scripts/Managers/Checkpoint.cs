using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{

    private new Transform transform;

    void Awake()
    {
        transform = GetComponent<Transform>();
    }

	void Update()
    {

	}

	void OnTriggerEnter(Collider other)
    {
		if (other.tag == "Player")
        {
          PlayerData data = new PlayerData();
          data.playerPosition = DataManager.Vector3ToString(other.gameObject.transform.position);
          data.playerScale = DataManager.Vector3ToString(other.gameObject.transform.localScale);
          data.playerEnergy = other.gameObject.GetComponent<Player>().LightEnergy.CurrentEnergy;
		  DataManager.SaveFile(data);
		}

        Destroy(gameObject);
	}

    // private void MoveCheckpoint()
    // {
    //     int x = Random.Range(-20, 60);
    //     int y = Random.Range(0, -30);
    //     int z = 0;
    //     transform.position = new Vector3(x, y, z);
    // }

}


