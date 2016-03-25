using UnityEngine;
using System.Collections;

public class BossThemeTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.PostEvent("BossTheme", this.gameObject);
            AkSoundEngine.PostEvent("Ambient2Stop", this.gameObject);
        }
    }
}
