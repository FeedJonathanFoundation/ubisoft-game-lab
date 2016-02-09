using UnityEngine;
using System.Collections;

public abstract class AbstractFish : MonoBehaviour 
{
    public int movementSpeed;
    public int reactionSpeed;
    
    private bool isProximateToPlayer;
    private bool isProximateToNPC;
    Transform player;                           // Reference to player's position
    NPCAction movement = new NPCAction(new Wander());
    NPCAction escape = new NPCAction(new Escape());
    NPCAction attack = new NPCAction(new Attack());
    

    public AbstractFish() { }
    
    public AbstractFish(NPCAction reactionToPlayer) { }

    void Awake() 
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        isProximateToPlayer = false;
        isProximateToNPC = false;
    }

    void FixedUpdate() 
    {
        // if (playerHealth >= 0 && !IsDead())
        // {
        if (isProximateToPlayer) { ReactToPlayer(player); }
        // if (player light == on) { Approach(player); }
        // else if (isProximateToNPC) { ReactToNPC(other); }
        else { Move(); }
        // }
        // else if (isDead()) { this.gameObject.SetActive(false); }
    }
    
    // Detects if fish is close to the player
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximateToPlayer = true;
        }
        else if (other.gameObject.CompareTag("Fish")) 
        {
            isProximateToNPC = true;
        }
    }
    
    // Detects if fish is no longer close to the player
    void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            isProximateToPlayer = false;
        }
        else if (other.gameObject.CompareTag("Fish")) 
        {
            isProximateToNPC = false;
        }
    }
    
    public bool IsDead()
    {
        // if (enemyHealth >= 0) { return true; }
        // else { return false; }
        return false;
    }

    // How the fish moves when it is not proximate to the player
    public abstract void Move();
    
    // How the fish moves when it is proximate to the player
    public abstract void ReactToPlayer(Transform player);
    
    // How the fish moves when it is proximate to the player
    public abstract void ReactToNPC(Transform other);
    
    // Returns the height of a fish
    public virtual float GetHeight()
    {
        return transform.lossyScale.y;
    }
    
    // Returns the width of a fish
    public virtual float GetWidth()
    {
        return transform.lossyScale.x;
    }
    
    // Calculates the radius of a sphere around the fish
    public float CalculateRadius()
    {
        float height = GetHeight();
        float width = GetWidth();
        
        if (height > width) { return height / 2; }
        else { return width / 2; }
    }

}
