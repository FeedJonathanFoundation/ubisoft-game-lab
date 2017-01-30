using UnityEngine;
using System.Collections;

public class JellyfishShock : MonoBehaviour 
{
    [SerializeField]
    [Tooltip("Amount of energy sucked up but the jellyfish")]
    private float energyLost;
    [Tooltip("Interval at which the jellyfish sucks up player energy")]
    public float timeBeforeEnergyLost;
    private float timer = 0;
    
    [SerializeField]
    [Tooltip("Particle effect played when the player is hit by a fish")]
    private ParticleSystem hitParticles;
    
    [SerializeField]
    [Tooltip("The amount of force applied on the player when hit by a jellyfish")]
    private float knockbackForce = 10;
    
    [SerializeField]
    [Tooltip("The amount of time the player flashes when hit")]
    private float hitFlashDuration = 2.0f;
    
     [Header("Emissive Colours")]
    [SerializeField]
    private Color probeColorOn = new Color(1f, 0.3103448f, 0f);
    
    [SerializeField]    
    private Color probeColorOff = new Color(0.3f,0.09310344f,0);
    
    
    [SerializeField]
    private Color probeColorHit = new Color(1, 0.067f, 0.067f);
    
    private ControllerRumble controllerRumble;  // Caches the controller rumble component

    [SerializeField]
    private GenericSoundManager soundManager;

    void Start()
    {
        if (soundManager == null)
        {
            GameObject soundGameObject = GameObject.FindWithTag("SoundManager");
            if (soundGameObject !=null)
            {
                soundManager = soundGameObject.GetComponent<GenericSoundManager>();
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "Player")
        {
            Player player = col.GetComponent<Player>();
            if(player)
            {
                Knockback(player);
                player.LightEnergy.Deplete(energyLost);
                if (soundManager)
                {
                    soundManager.JellyfishAttackSound(this.gameObject);
                }
            }
        }
    }
    
    public void Knockback(LightSource lightSource)
    {
        if (lightSource is Player)
        {

            Transform player = lightSource.transform;
            Rigidbody playerRigidbody = player.GetComponent<Rigidbody>();
            this.controllerRumble = player.GetComponent<ControllerRumble>();
            // Calculate a knockback force pushing the player away from the enemy fish
            Vector2 distance = (player.position - gameObject.transform.position);
            Vector2 knockback = distance.normalized * knockbackForce;

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.AddForce(knockback, ForceMode.Impulse);


            // Instantiate hit particles
            GameObject.Instantiate(hitParticles, player.position, Quaternion.Euler(0, 0, 0));
            FlashColor(probeColorHit, hitFlashDuration);

            // Rumble the controller when the player hits a fish.
            controllerRumble.PlayerHitByFish();

            // The player was just hit
            // lastTimeHit = Time.time;
        }

    }
    
    /// <summary>
    /// Flashes the probe's emissive color in the specified time
    /// </summary>
    private void FlashColor(Color color, float seconds)
    {
        // if (this.lightToggle != null)
        // {
        //     // If the lights are enabled, flash back to the probe's 'on' color
        //     if (this.lightToggle.LightsEnabled)
        //     {
        //         FlashColor(color, probeColorOn, seconds);
        //     }
        //     // If the lights are disabled, flash back to the probe's 'off' color
        //     else
        //     {
        //         FlashColor(color, probeColorOff, seconds);
        //     }
        // }
    }
   
    
    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            if (soundManager)
            {
                soundManager.StopJellyfishAttackSound(this.gameObject);
            }
            
        }
    }

}
