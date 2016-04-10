using UnityEngine;
using System.Collections;

public class FlareSound : MonoBehaviour
{

    GameObject target;

    public FlareSound(GameObject target)
    {
        this.target = target;
    }

    public void ShootFlareSound()
    {
        AkSoundEngine.PostEvent("Flare", target);
    }
    
    public void SetFlareDistance(float flareDistance)
    {
        AkSoundEngine.SetRTPCValue("flareDistance", flareDistance);
    }
    
    // need to pass boss fish object
    public void EatFlareSound()
    {
        AkSoundEngine.PostEvent("FlareEat", this.gameObject);
    }
    
}
