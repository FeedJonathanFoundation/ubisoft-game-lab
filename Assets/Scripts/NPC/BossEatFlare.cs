using UnityEngine;
using System.Collections;

public class BossEatFlare : MonoBehaviour
{
    private GameObject player;
    [SerializeField]
    private GenericSoundManager soundManager;

    void Start()
    {
        player = GameObject.Find("Player");
        if (soundManager == null)
        {
            GameObject soundGameObject = GameObject.FindWithTag("SoundManager");
            if (soundGameObject != null)
            {
                soundManager = soundGameObject.GetComponent<GenericSoundManager>();
            }
        }
    }
    
    void OnTriggerEnter(Collider col)
    {

        if (col.CompareTag("Flare"))
        {
            Destroy(col.transform.parent.gameObject);
            
            player.GetComponent<FlareSpawner>().EatFlare();
        }
    }
    
    private void BossEatSound()
    {
        soundManager.BossEatSound(this.gameObject);
    }
}
