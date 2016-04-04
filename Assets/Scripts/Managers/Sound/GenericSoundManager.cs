using UnityEngine;
using System.Collections;

public class GenericSoundManager : MonoBehaviour
{
    
    public void CurrentSound(GameObject target)
    {
        AkSoundEngine.PostEvent("Current", target); 
    }
    
    public void JellyfishAttackSound(GameObject target)
    {
        AkSoundEngine.PostEvent("JellyfishAttack", this.gameObject);
    }
    
    public void StopJellyfishAttackSound(GameObject target)
    {
        AkSoundEngine.PostEvent("StopJellyfishAttack", target);
    }
    
    public void PulseSound(GameObject target)
    {
        AkSoundEngine.PostEvent("Pulse", target);
    }
}
