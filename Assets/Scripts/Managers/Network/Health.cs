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

    private NetworkStartPosition[] spawnPoints;

    [SerializeField]
    private bool destroyOnDeath;

    void Start()
    {
        multiplier = healthBarWidth / maxHealth;
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }


    public void TakeDamage(int amount)
    {
        if (!isServer) { return; }
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Dead");
            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else //if (restartButtonPushed) // replace this pseudocode; 
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
            // spawns player at (0, 0, 0)
            // transform.position = Vector3.zero;
            Vector3 spawnPoint = Vector3.zero;
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position;
                // maybe have it iterate instead of being random
            }
            transform.position = spawnPoint;
        }
    }
    
}
