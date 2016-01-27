using UnityEngine;
using System.Collections;

public abstract class AbstractFish : MonoBehaviour 
{

    Transform player;           // Reference to player's position
    //  PlayerHealth playerHealth;
    //  EnemyHealth enemyHealth; 

    void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        //      playerHealth = player.GetComponent <PlayerHealth> ();
        //      enemyHealth = GetComponent <EnemyHealth> ();
    }

    void Update() 
    {
        if (isProximate()) 
        {
            React(player);
        }
        // if (player light == on) { Approach(player); }
        else 
        {
            Move(player);
        }
    }

    public bool isProximate()
    {
        // on trigger collision with sphere collider around player
        if (1>0) { return true; }
        else { return false; }
    }

    public abstract void Move(Transform player);
    
    public abstract void React(Transform player);

}
