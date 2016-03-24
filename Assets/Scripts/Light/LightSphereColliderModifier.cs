using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class LightSphereColliderModifier : LightEnergyListener
{
    [Tooltip("The amount of light energy required to have a SphereCollider radius of 1")]
    public float lightToRadiusRatio;
    
    [Tooltip("The collider's radius is multiplied by this constant if the player is thrusting.")]
    public float thrustRadiusMultiplier;
    
    // Cache GameObject components
    private SphereCollider sphereCollider;
    private Player player;
    private PlayerLightToggle playerLightToggle;
    
    public override void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        // Move the collider somewhere no fish will ever see on game start
        sphereCollider.center = new Vector3(1000000,1000000,100000);
        
        base.Start();
        
        if (lightSource is Player) { player = (Player)lightSource; }
    }
    
    public override void OnLightChanged(float currentLight)
    {
        sphereCollider.radius = currentLight * lightToRadiusRatio;
    }
    
    void Update()
    {
        // Compute the sphere collider's radius based on the parent light source's energy
        float colliderRadius = lightSource.LightEnergy.CurrentEnergy * lightToRadiusRatio;
        Vector3 colliderCenter = Vector2.zero;
        
        // Debug.Log("Detectable radius: " + colliderRadius);
        
        if (player != null)
        {   
            // If the player's lights are turned off, disable the sphere. Fish should not be able to detect the player.
            if (!player.IsDetectable())
            {
                colliderRadius = 0;
                // Move the collider somewhere no fish will ever see
                colliderCenter = new Vector3(1000000,1000000,100000);
            }
            
            if (player.Movement.Thrusting)
            {
                colliderRadius *= thrustRadiusMultiplier;

                // Debug.Log("AFTER THRUSTING Detectable radius: " + colliderRadius);
                // Debug.Log("PLAYER THRUSTING");
            }
        }
        
        sphereCollider.radius = colliderRadius;
        sphereCollider.center = colliderCenter;
    }
}