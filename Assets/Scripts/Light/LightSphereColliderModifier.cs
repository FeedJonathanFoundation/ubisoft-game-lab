using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class LightSphereColliderModifier : LightEnergyListener
{
    [Tooltip("The amount of light energy required to have a SphereCollider radius of 1")]
    public float lightToRadiusRatio;
    
    // Cache GameObject components
    private SphereCollider sphereCollider;
    
    public override void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        // Move the collider somewhere no fish will ever see on game start
        sphereCollider.center = new Vector3(1000000,1000000,100000);
        
        base.Start();
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
        
        if (lightSource is Player)
        {
            Player player = (Player)lightSource;
            
            // If the player's lights are turned off, disable the sphere. Fish should not be able to detect the player.
            if (!player.IsDetectable())
            {
                colliderRadius = 0;
                // Move the collider somewhere no fish will ever see
                colliderCenter = new Vector3(1000000,1000000,100000);
            }
        }
        
        sphereCollider.radius = colliderRadius;
        sphereCollider.center = colliderCenter;
    }
}