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
    
    public void BossEatSound(GameObject target)
    {
        AkSoundEngine.PostEvent("BossEat", target);
    }
    
    public void SeekSound(GameObject target)
    {
        AkSoundEngine.PostEvent("Fish_Detection", target);
    }
    
    public void CriticalHealthSound(GameObject target)
    {
        AkSoundEngine.PostEvent("CriticalHealth", target);
    }
    
    public void IntroSound(GameObject target)
    {
        AkSoundEngine.PostEvent("Ambient2", target);
        AkSoundEngine.SetState("IMAmb2", "CP2");
    }
    
    public void StopIntroSound(GameObject target)
    {
        AkSoundEngine.PostEvent("Ambient2Stop", target);
    }
    
    // if (soundManager == null)
    //     {
    //         GameObject soundGameObject = GameObject.FindWithTag("SoundManager");
    //         if (soundGameObject !=null)
    //         {
    //             soundManager = soundGameObject.GetComponent<GenericSoundManager>();
    //         }
    //     }
    
    
}
