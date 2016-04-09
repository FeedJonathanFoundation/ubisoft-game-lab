using UnityEngine;
using System.Collections;

/// <summary>
/// PlayerSound is responsible for playing player-related sounds
/// 
///
/// @author - Stella L.
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerSound : MonoBehaviour
{

    GameObject target;
    void Start()
    {
        Player player = GetComponent<Player>();
        
        if (!player)
        {
            Debug.Log("Player not found - player sounds will not work.");
            return;
        }
        target = player.gameObject;
        AkSoundEngine.SetState("PlayerLife", "Alive");
        AkSoundEngine.PostEvent("Default", target); 
    }
    void OnCollisionEnter(Collision collision)
    {
        // AkSoundEngine.PostEvent("WallCrash", target);
    }
    
    public void ExplosionSound()
    {
        // AkSoundEngine.PostEvent("Explosion", target);
    }
    
    public void AttackSound()
    {
        // AkSoundEngine.PostEvent("Attack", target);
    }
    
    public void EatSound()
    {
        // AkSoundEngine.PostEvent("Eat", this.gameObject); 
    }
    
    public void PlayerDeathSound()
    {
        // AkSoundEngine.SetState("PlayerLife", "Dead");
        // AkSoundEngine.PostEvent("Die", target);
    }
    
    public void LightToggleSound()
    {
        // AkSoundEngine.PostEvent("LightsToToggle", target);
    }

    public void InsufficientEnergySound()
    {
        // AkSoundEngine.PostEvent("LowEnergy", target);
    }
    
    public void SetPlayerVelocity(float playerVelocity)
    {
        // AkSoundEngine.SetRTPCValue("playerVelocity", playerVelocity);
    }

}
