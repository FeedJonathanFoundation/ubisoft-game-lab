using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Player : LightSource
{

    /// <summary>
    /// The position at which the character ejects mass.
    /// </summary>
    public Transform massEjectionTransform;

    /// <summary>
    /// The light ball ejected by the player when thrustingz\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    /// </summary>
    public GameObject lightBallPrefab;

    [Tooltip("The player's max speed in m/s")]
    public float maxSpeed;

    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass.")]
    public float thrustForce;

    [Tooltip("The higher the value, the faster the propulsion when changing directions")]
    public float changeDirectionBoost;

    /// <summary>
    /// The amount of light energy spent when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of light energy spent when ejecting one piece of mass.")]
    public float thrustEnergyCost = 1;
    
    /// <summary>
    /// The propulsion effect activated when the player is propulsing
    /// </summary>
    [Tooltip("The parent of the propulsion particle effects activated when the player is propulsing")]
    public GameObject jetFuelEffect;

    [Tooltip("If true, the lights are enabled on scene start.")]
    public bool defaultLightStatus = true;
    
    [Tooltip("The amount of force applied on the player when hit by an enemy")]
    public float knockbackForce = 10;
    
    [Tooltip("Amount of time invulnerable after being hit by an enemy")]
    public float invulnerabilityTime = 3;
    
    [Tooltip("Linear drag applied when player is hit by enemy")]
    public float invulnerabilityDrag = 2;

    [Tooltip("Time interval for lost of lights while lights are on")]
    public float lostOfLightTime;

    [Tooltip("Amount of light lost while lights are turned on")]
    public float energyCostLightToggle;

    [Tooltip("Energy needed to activate light and that light will turn off if reached")]
    public float minimalEnergyRestrictionToggleLights;

    [Tooltip("If true, checkpoints are not used and user is spawned at the initial position")]
    public bool disableCheckpoints;

    /** Caches the player's components */
    private PlayerMovement movement;
    private PlayerLightToggle lightToggle;
    private float lastTimeHit = -100;  // The last time player was hit by an enemy
    private float defaultDrag;  // Default rigidbody drag
    private bool isDead; // determines is current player is dead
    private new Transform transform;
    private new Rigidbody rigidbody;
    private int currentLevel;

    // Use this for initialization
    public override void Awake()
    {
       base.Awake(); // call parent LightSource Awake() first
       
       transform = GetComponent<Transform>();
       rigidbody = GetComponent<Rigidbody>();
       
       movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, changeDirectionBoost, thrustEnergyCost, transform, rigidbody, this.LightEnergy, this.jetFuelEffect);
       lightToggle = new PlayerLightToggle(transform.Find("LightsToToggle").gameObject, defaultLightStatus, this, minimalEnergyRestrictionToggleLights);
       
       this.defaultDrag = Rigidbody.drag;
       this.isDead = false;
       this.currentLevel = SceneManager.GetActiveScene().buildIndex;
       DontDestroyOnLoad(this.gameObject);
       
       LoadGame();
    }
    
    void OnLevelWasLoaded(int level) 
    {
        Debug.Log("Scene " + level + " is loaded!");
        this.transform.position = new Vector3(0, 0, 0); 
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update(); 
        
        // Clamp the player's velocity
        if (rigidbody.velocity.sqrMagnitude > maxSpeed*maxSpeed)
        {
            rigidbody.velocity = ((Vector2)rigidbody.velocity).SetMagnitude(maxSpeed);
        }
        
        // Modify player drag if invulnerable
        if (IsInvulnerable()) 
        { 
            // 0 = just became invulnerable
            // 1 = not invulnerable anymore
            float invulnerabilityPercent = (Time.time-lastTimeHit)/invulnerabilityTime;
            Rigidbody.drag = (invulnerabilityDrag-defaultDrag) * (1-invulnerabilityPercent) + defaultDrag; 
        }
        else { Rigidbody.drag = defaultDrag; }
        
        if (isDead)
        {
            Restart();
        }
        else
        {
            Move();
            LightControl();
        }
    }

    /// <summary>
    /// Player movements responding to user input
    /// </summary>
    private void Move()
    {
        if (Input.GetButtonDown("Thrust"))
        {
            movement.OnPropulsionStart();
        }
        
        if (Input.GetButton("Thrust"))
        {
            // Propulse in the direction of the left stick (opposite to the rear of the probe)
            movement.Propulse(-massEjectionTransform.up);
        }
        
        if (Input.GetButtonUp("Thrust"))
        {
            movement.OnPropulsionEnd();
        }

        // Makes the character follow the left stick's rotation
        movement.FollowLeftStickRotation();

        // Ensure that the rigidbody never spins
        rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// Player lights responding to user input
    /// </summary>
    private void LightControl()
    {
        if (Input.GetButtonDown("LightToggle") && minimalEnergyRestrictionToggleLights < this.LightEnergy.CurrentEnergy)
        {
            lightToggle.ToggleLights();
        }
        lightToggle.LostOfLight(lostOfLightTime, energyCostLightToggle);
    }

    private void Restart()
    {
        if (Input.GetButtonDown("Restart"))
        {
            Debug.Log("Game Restarted");
            this.LightEnergy.Add(this.defaultEnergy);
            this.isDead = false;
            this.rigidbody.drag = 0; // reset drag
            LoadGame();
        }
    }

    private void LoadGame()
    {              
        PlayerData data = DataManager.LoadFile();
        
        if (data != null && !disableCheckpoints)
        {
            if (data.levelID != this.currentLevel) 
            {
                if (SceneManager.sceneCountInBuildSettings > data.levelID)
                {
                    SceneManager.LoadScene(data.levelID, LoadSceneMode.Single);
                }                    
            }
            transform.position = DataManager.Vector3FromString(data.playerPosition);
            transform.localEulerAngles = DataManager.Vector3FromString(data.playerRotation);
        } 
        else 
        {
            transform.position = new Vector3(0, 0, 0);
            transform.localEulerAngles = new Vector3(0, 0, 0);                                
        }
    }
    
    public override void Knockback(LightSource enemyLightSource)
    {        
        // Calculate a knockback force pushing the player away from the enemy fish
        Vector2 distance = (Transform.position - enemyLightSource.Transform.position);
        Vector2 knockback = distance.normalized * knockbackForce;
        
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.AddForce(knockback, ForceMode.Impulse);
        
        // The player was just hit
        lastTimeHit = Time.time;
    }
    
    /// <summary>
    /// If true, the player has been hit and is temporarily
    /// invulnerable
    /// </summary>
    public bool IsInvulnerable()
    {
        return (Time.time - lastTimeHit) < invulnerabilityTime;
    }
    
    public override bool CanBeAbsorbed()
    {
        // The player cannot be absorbed if invulnerable
        if (IsInvulnerable())
        {
            Debug.Log("PLAYER CAN'T BE ABSORBED: " + (Time.deltaTime - lastTimeHit));
            return false;
        }
        
        return true;
    }

    /// <summary>
    /// Listens for LightDepleted event from LightEnergy
    /// Sets player to dead when it occurs
    /// </summary>
    protected override void OnLightDepleted()
    {
        base.OnLightDepleted(); 
        
        movement.OnPropulsionEnd();
        isDead = true;
        Debug.Log("Game OVER! Press 'R' to restart!");
    }

    public int CurrentLevel
    {
        get { return this.currentLevel; }
    }
    
    /// <summary>
    /// If player lights are on, player is visible
    /// </summary>
    public bool IsDetectable()
    {
        return lightToggle.LightsEnabled;
    }


}