using UnityEngine;
using System.Collections;

/// <summary>
/// BossThemeTrigger is responsible for stopping sounds from scene 2
/// and loading scene 3 sounds using a trigger
///
/// @author - Stella L.
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(Collider))]
public class BossThemeTrigger : MonoBehaviour
{
    /// <summary>
    /// Once the player enters, stop ambient 2 sounds and
    /// load ambient 3 sounds
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AkSoundEngine.PostEvent("Ambient3", this.gameObject);
            AkSoundEngine.PostEvent("Ambient2Stop", this.gameObject);
        }
    }
}
