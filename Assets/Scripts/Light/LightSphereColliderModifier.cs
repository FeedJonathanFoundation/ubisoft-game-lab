using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class LightSphereColliderModifier : LightEnergyListener
{
    [Tooltip("The amount of light energy required to have a SphereCollider radius of 1")]
    public float lightToRadiusRatio;
    
    // Cache GameObject components
    private SphereCollider sphereCollider;
    
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    
    public override void OnLightChanged(float currentEnergy)
    {
        sphereCollider.radius = currentEnergy * lightToRadiusRatio;
    }
}