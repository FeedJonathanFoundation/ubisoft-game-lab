using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkHealthBar : NetworkBehaviour
{

    [SerializeField]
    float healthBarWidth = 200f;
    [SerializeField]
    RectTransform healthBar;
    private float multiplier;
    [SerializeField]
    private float maxHealth;
    [SyncVar(hook = "OnLightChanged")]
    private float currentHealth;
    private bool restartButtonPushed = false;
    private Player player;
    private NetworkStartPosition[] spawnPoints;
    
    void Start()
    {
        InitializePlayer(); 
        spawnPoints = FindObjectsOfType<NetworkStartPosition>();
    }
    void onDisable()
    {
        player.LightEnergy.LightChanged -= OnLightChanged;
    }
    
    void InitializePlayer()
    {
        if (player == null)
        {
            player = GetComponent<Player>();
        }
        player.LightEnergy.LightChanged += OnLightChanged;
            
        maxHealth = player.DefaultEnergy;
        currentHealth = maxHealth;
        
        multiplier = healthBarWidth / maxHealth;
    }
    
    void OnLightChanged(float currentEnergy)
    {
        if (maxHealth == 0) { InitializePlayer(); }

        // Debug.Log("currentEnergy: " + currentEnergy + "\n healthBar: " + healthBar.sizeDelta.x);
        currentHealth = currentEnergy;
        healthBar.sizeDelta = new Vector2(currentHealth * multiplier, healthBar.sizeDelta.y);
    }

    public void OnRespawn()
    {
        if (!isServer) { return; }
        RpcRespawn();
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
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