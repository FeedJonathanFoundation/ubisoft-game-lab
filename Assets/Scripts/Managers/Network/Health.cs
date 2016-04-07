using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Health : NetworkBehaviour 
{

    [SerializeField] const int maxHealth = 100;
    [SyncVar(hook = "OnChangeHealth")][SerializeField] int currentHealth = maxHealth;
    [SerializeField] RectTransform healthBar;
    [SerializeField]
    int healthBarWidth = 200;
    private int multiplier;

    private bool restartButtonPushed = false;

    void Awake()
    {
        multiplier = healthBarWidth / maxHealth;
    }


    public void TakeDamage(int amount)
    {
        if (!isServer) { return; }
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead");
            if (restartButtonPushed) // replace this pseudocode; 
            // should probably be in update and check if isDead;
            {
                currentHealth = maxHealth;
                RpcRespawn();
            }
        }
    }
    
    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(currentHealth * multiplier, healthBar.sizeDelta.y);
    }
    
    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            transform.position = Vector3.zero;
        }
    }
    
}
