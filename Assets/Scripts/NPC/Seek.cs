using UnityEngine;
using System.Collections;

[System.Serializable]
public class Seek : MonoBehaviour, NPCActionable
{
    
    public Transform targetTransform;
    
    public float strengthMultiplier = 9.9f;
    
	public void Execute(Steerable steerable) 
    {
        targetTransform = GameObject.FindGameObjectWithTag("Player").transform;
        steerable.AddSeekForce (targetTransform.position, strengthMultiplier);
    }
}
