using UnityEngine;
using System.Collections;

public abstract class AbstractFish : MonoBehaviour 
{
    private bool isProximate;

    Transform player;           // Reference to player's position
    //  PlayerHealth playerHealth;
    //  EnemyHealth enemyHealth; 

    void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //      playerHealth = player.GetComponent <PlayerHealth> ();
        //      enemyHealth = GetComponent <EnemyHealth> ();
        isProximate = false;
    }

    void FixedUpdate() 
    {
        // if (playerHealth >= 0 && !isDead())
        // {
        if (isProximate) { React(player); }
        // if (player light == on) { Approach(player); }
        else { Move(player); }
        // }
        // else if (isDead()) { this.gameObject.SetActive(false); }
    }
    
    // Detects if fish is close to the player
    // ****** Change tag to refer to space around player, not player itself
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximate = true;
        }
    }
    
    // Detects if fish is no longer close to the player
    // ****** Change tag to refer to space around player, not player itself
    void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximate = false;
        }
    }
    
    public bool isDead()
    {
        // if (enemyHealth >= 0) { return true; }
        // else { return false; }
        return false;
    }

    // How the fish moves when it is not proximate to the player
    public abstract void Move(Transform player);
    
    // How the fish moves when it is proximate to the player
    public abstract void React(Transform player);

}
