using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Health : NetworkBehaviour 
{

    [SerializeField] const float maxHealth = 100;
    // [SyncVar(hook = "OnLightChanged")]
    [SerializeField] float currentHealth = maxHealth;
    [SerializeField] Slider healthBar;

    private bool restartButtonPushed = false;
    private Player player;
    private NetworkStartPosition[] spawnPoints;
    

    [SerializeField]
    private bool destroyOnDeath;

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        // player.LightEnergy.LightChanged += OnLightChanged;
        if (isLocalPlayer)
        {
            spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        }
    }
    
    void onDisable()
    {
        // player.LightEnergy.LightChanged -= OnLightChanged;
    }
    
    float OnLightChanged(float currentEnergy)
    {
        healthBar.value = currentEnergy * 100;
        return healthBar.value;
    }
    
    // void OnChangeHealth(int health)
    // {
    //     healthBar.value = health * 100;
    // }
    
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