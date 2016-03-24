using UnityEngine;
using System.Collections;

public class JellyfishShock : MonoBehaviour 
{
    [Tooltip("Amount of energy sucked up but the jellyfish")]
    public float energyLost;
    [Tooltip("Interval at which the jellyfish sucks up player energy")]
    public float timeBeforeEnergyLost;
    private float timer = 0;
    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            timer += Time.deltaTime;
            if(timer > timeBeforeEnergyLost)
            {
                Player player = col.GetComponent<Player>();
                if(player)
                {
                    player.LightEnergy.Deplete(energyLost);
                }
                timer = 0;
            }
        }
    }
    
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Player")
        {
            AkSoundEngine.PostEvent("JellyfishAttack", this.gameObject);
        }
    }
    
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            AkSoundEngine.PostEvent("StopJellyfishAttack", this.gameObject);
        }
    }
}
