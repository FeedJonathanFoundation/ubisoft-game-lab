using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Player class is responsible for behaviour related to player's object
///
/// @author - Jonathan L.A
/// @author - Alex I.
/// @version - 1.0.0
///
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Transform))]
public class Player : LightSource
{
    [Header("Player Movement")]
    [SerializeField]
    [Tooltip("The position at which the character ejects mass")]
    private Transform massEjectionTransform;

    [SerializeField]
    [Tooltip("The light ball ejected by the player when thrusting")]
    private GameObject lightBallPrefab;

    [SerializeField]
    [Tooltip("The player's max speed in m/s")]
    private float maxSpeed = 100;

    [SerializeField]
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass")]
    private float thrustForce = 0;

    [SerializeField]
    [Tooltip("The higher the value, the faster the propulsion when changing directions")]
    private float changeDirectionBoost = 0;

    [SerializeField]
    [Tooltip("The amount of light energy spent when ejecting one piece of mass")]
    private float thrustEnergyCost = 1;

    [SerializeField]
    [Tooltip("The damping to apply when the brakes are on at full strength")]
    private float brakeDrag = 1;

    [SerializeField]
    [Tooltip("The speed of rotation of the player in response to user input")]
    private float rotationSpeed = 5;

    [SerializeField]
    [Tooltip("The parent of the propulsion particle effects activated when the player is propulsing")]
    private GameObject jetFuelEffect;

    [SerializeField]
    [Tooltip("Particle effect played when the player is hit by a fish")]
    private ParticleSystem fishHitParticles;

    [SerializeField]
    [Tooltip("Particle effect played when the player dies")]
    private ParticleSystem playerDeathParticles;

    [Header("Player Lights")]
    [SerializeField]
    [Tooltip("If true, the lights are enabled on scene start")]
    private bool defaultLightStatus = true;

    [SerializeField]
    [Tooltip("The amount of force applied on the player when hit by an enemy")]
    private float knockbackForce = 10;

    [SerializeField]
    [Tooltip("Amount of time invulnerable after being hit by an enemy")]
    private float invulnerabilityTime = 3;

    [SerializeField]
    [Tooltip("Linear drag applied when player is hit by enemy")]
    private float invulnerabilityDrag = 2;

    [SerializeField]
    [Tooltip("Time interval for energy depletion while lights are on")]
    private float timeToDeplete = 0;

    [SerializeField]
    [Tooltip("Amount of light lost while lights are turned on")]
    private float lightToggleEnergyCost = 0;

    [SerializeField]
    [Tooltip("Energy needed to activate light and that light will turn off if reached")]
    private float minimalEnergyRestrictionToggleLights = 0;
    
    [SerializeField]
    [Tooltip("The percent range of the players lights when propulsing with lights off")]
    private float propulsionLightRange = 0.3f;

    [SerializeField]
    [Tooltip("The amount of time it takes for the player's emissive light to toggle on/off")]
    private float lightToggleTime = 0.1f;

    [SerializeField]
    [Tooltip("If true, checkpoints are not used and user is spawned at the initial position")]
    private bool disableCheckpoints = true;
    private PlayerMovement movement;
    private PlayerLightToggle lightToggle;
    private float lastTimeHit = -100;  // The last time player was hit by an enemy
    private float defaultDrag;  // Default rigidbody drag
    private float previousThrustAxis; // Previous value of Input.GetAxis("ThrustAxis")
    private bool isDead; // determines is current player is dead
    public bool isSafe; // used for boss AI
    private bool deathParticlesPlayed;
    private MaterialExtensions materials;
    private ControllerRumble controllerRumble;  // Caches the controller rumble component
    private GameObject gameOverCanvas;
    private int currentLevel;
    public int playerVelocity;

    [Header("Emissive Colours")]
    [SerializeField]
    private Color probeColorOn = new Color(1f, 0.3103448f, 0f);
    
    [SerializeField]    
    private Color probeColorOff = new Color(0.3f,0.09310344f,0);
    
    [SerializeField]
    private Color probeColorHit = new Color(1, 0.067f, 0.067f);
           
    [SerializeField]
    private Color probeColorEatFish = new Color(0, 0.875f, 1);
    
    [SerializeField]
    private Color probeColorEatPickup = new Color(0.82f, 0.82f, 0.596f);
    
    [SerializeField]
    [Tooltip("The amount of time the player flashes when eating a fish")]
    private float eatFlashDuration = 0.5f;
    
    [SerializeField]
    [Tooltip("The amount of time the player flashes when hit")]
    private float hitFlashDuration = 2.0f;
    
    private IEnumerator changeColorCoroutine;
    private IEnumerator flashColorCoroutine;
    private IEnumerator changeIntensityCoroutine;
    
    private static Player playerInstance;

    private GameObject UI;

    [SerializeField]
    private GameObject gameOverCanvasPrefab;

    /// <summary>
    /// Initializes Player components
    /// </summary>
    protected override void Awake()
    {
        base.Awake(); // call parent LightSource Awake() first
        if (playerInstance != null && playerInstance != this)
        {
            GameObject.Destroy(this.gameObject);   
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            playerInstance =  this;
        }
                       
        this.movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, changeDirectionBoost, thrustEnergyCost, brakeDrag, this.Transform, this.Rigidbody, this.LightEnergy, this.jetFuelEffect, this.rotationSpeed);
        this.lightToggle = new PlayerLightToggle(this.Transform.Find("LightsToToggle").gameObject, defaultLightStatus, this, minimalEnergyRestrictionToggleLights, propulsionLightRange);
        this.materials = new MaterialExtensions();

        this.defaultDrag = Rigidbody.drag;
        this.isDead = false;
        this.isSafe = true;
        this.controllerRumble = GetComponent<ControllerRumble>();
        AkSoundEngine.SetState("PlayerLife", "Alive");
        this.currentLevel = SceneManager.GetActiveScene().buildIndex;
        ChangeColor(probeColorOff, false, 0);
        LoadGame();
        ResetPlayerState();
        
        gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
        
        UI = GameObject.FindWithTag("UI");

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }
        else
        {
            Canvas[] canvases = UI.GetComponentsInChildren<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (canvas.name == "GameOverCanvas")
                {
                    gameOverCanvas = canvas.gameObject;
                    gameOverCanvas.SetActive(false);
                    break;
                }
            }
        }

#if UNITY_EDITOR
        this.ValidateInputs();
        #endif
    }

    public override void OnEnable()
    {
        base.OnEnable();
        ConsumedLightSource += OnConsumedLightSource;
    }

    public override void OnDisable()
    {
        base.OnEnable();
        ConsumedLightSource -= OnConsumedLightSource;
    }


    /// <summary>
    /// Listens for player states such as movement, light controls and death
    /// Called once per frame
    /// </summary>
    protected override void Update()
    {
        base.Update();

        // if (gameOverCanvas == null)
        // {
        //     gameOverCanvas = GameObject.Instantiate(gameOverCanvasPrefab);
        //     gameOverCanvas.SetActive(false);
        // }
        if (gameOverCanvas == null)
        {
            gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
        }
        if (gameOverCanvas.activeSelf == true && !isDead)
        {
            gameOverCanvas.SetActive(false);
        }
        

        playerVelocity = (int)this.Rigidbody.velocity.magnitude;
        AkSoundEngine.SetRTPCValue("playerVelocity", playerVelocity);
        
        // Modify player drag if invulnerable
        if (IsInvulnerable())
        {
            // 0 = just became invulnerable
            // 1 = not invulnerable anymore
            float invulnerabilityPercent = (Time.time - lastTimeHit) / invulnerabilityTime;
            Rigidbody.drag = (invulnerabilityDrag - defaultDrag) * (1 - invulnerabilityPercent) + defaultDrag;
        }
        else
        {
            Rigidbody.drag = defaultDrag;
        }

        if (isDead)
        {
            this.isSafe = true;
            gameOverCanvas.SetActive(true);
            RestartGame();
        }
        else
        {
            Move();
            LightControl();

            // Clamp the player's velocity
            if (this.Rigidbody.velocity.sqrMagnitude > this.maxSpeed * this.maxSpeed)
            {
                this.Rigidbody.velocity = ((Vector2)this.Rigidbody.velocity).SetMagnitude(this.maxSpeed);
            }
        }
    }

    /// <summary>
    /// Invoked when a new scene is loaded
    /// </summary>
    protected void OnLevelWasLoaded(int level)
    {
        Debug.Log("Scene " + level + " is loaded!");
        ResetPlayerState();
    }

    /// <summary>
    /// Sets player state to 'dead' when LightDepleted event is triggered
    /// </summary>
    protected override void OnLightDepleted()
    {
        base.OnLightDepleted();

        // If the player just died
        if (!isDead)
        {
            movement.OnPropulsionEnd();
            Rigidbody.useGravity = true;
        }

        isDead = true;
        AkSoundEngine.SetState("PlayerLife", "Dead");
        AkSoundEngine.PostEvent("Die", this.gameObject);

        Debug.Log("Game OVER! Press 'R' to restart!");
    }

    public void ResetPlayerState()
    {
        this.Rigidbody.velocity = Vector3.zero;
        this.Transform.position = Vector3.zero;
        this.Transform.localEulerAngles = new Vector3(0, 0, -90);
    }

    /// <summary>
    /// If player lights are on, player is visible
    /// </summary>
    public bool IsDetectable()
    {
        if (lightToggle != null)
        {
            return lightToggle.LightsEnabled;
        }
        else
        {
            return false;
        }
    }

    public int CurrentLevel
    {
        get { return this.currentLevel; }
        set { this.currentLevel = value; }
    }

    /// <summary>
    /// Changes the color of the player avatar to the given one
    ///
    /// </summary>
    /// <param name="color">target color</param>
    /// <param name="isSmooth">if true, the color change will follow a smooth gradient</param>
    protected override void ChangeColor(Color color, bool isSmooth, float seconds)
    {
        if (changeColorCoroutine != null) { StopCoroutine(changeColorCoroutine); }
        
        foreach (GameObject probe in GameObject.FindGameObjectsWithTag("Probe"))
        {
            Renderer renderer = probe.GetComponent<Renderer>();
            foreach (Material material in renderer.materials)
            {
                if (isSmooth)
                {
                   changeColorCoroutine = materials.ChangeColor(material, color, seconds, 0f);
                   StartCoroutine(changeColorCoroutine);
                }
                else
                {
                    changeColorCoroutine = materials.ChangeColor(material, color, seconds);
                    StartCoroutine(changeColorCoroutine);
                }
            }
        }
    }

    /// <summary>
    /// Flashes the probe's emissive color in the specified time
    /// </summary>
    private void FlashColor(Color color, float seconds)
    {
        if (this.lightToggle != null)
        {
            // If the lights are enabled, flash back to the probe's 'on' color
            if (this.lightToggle.LightsEnabled)
            {
                FlashColor(color, probeColorOn, seconds);
            }
            // If the lights are disabled, flash back to the probe's 'off' color
            else
            {
                FlashColor(color, probeColorOff, seconds);
            }
        }
    }

    /// <summary>
    /// Flashes the probe's emissive color from start color to end color in the specified time
    /// </summary>
    private void FlashColor(Color startColor, Color endColor, float seconds)
    {
        if (flashColorCoroutine != null) { StopCoroutine(flashColorCoroutine); }
        
        foreach (GameObject probe in GameObject.FindGameObjectsWithTag("Probe"))
        {
            Renderer renderer = probe.GetComponent<Renderer>();
            foreach (Material material in renderer.materials)
            {
                flashColorCoroutine = materials.FlashColor(material, startColor, endColor, seconds); 
                StartCoroutine(flashColorCoroutine);
            }
        }
    }
    
    
    /// <summary>
    /// Listens for Lights button click
    /// When Lights button is clicked toggle the lights ON or OFF
    /// </summary>
    private void LightControl()
    {
        if (this.lightToggle != null)
        {
            if (Input.GetButtonDown("LightToggle"))
            {
                if (minimalEnergyRestrictionToggleLights < this.LightEnergy.CurrentEnergy)
                {
                    this.lightToggle.ToggleLights();
                    AkSoundEngine.PostEvent("LightsToToggle", this.gameObject);
                    
                    if (changeIntensityCoroutine != null) { StopCoroutine(changeIntensityCoroutine); }
                    
                    if (this.lightToggle.LightsEnabled)
                    {
                        this.ChangeColor(probeColorOn, true, 0);
                        changeIntensityCoroutine = materials.ChangeLightIntensity(this.lightToggle, 0.3f);
                        StartCoroutine(changeIntensityCoroutine);
                    }
                    else
                    {
                        this.ChangeColor(probeColorOff, true, 0);
                        changeIntensityCoroutine = materials.ChangeLightIntensity(this.lightToggle, 0f); 
                        StartCoroutine(changeIntensityCoroutine);
                    }
                }
                else
                {
                    // If the player isn't thrusting, turn off his emissive lights
                    if (!movement.Thrusting)
                    {
                        this.ChangeColor(probeColorOff, true, 0);
                    }
                    
                    AkSoundEngine.PostEvent("LowEnergy", this.gameObject);
                }
            }

            this.lightToggle.DepleteLight(timeToDeplete, lightToggleEnergyCost);
        }
    }

    /// <summary>
    /// Loads the last saved game state on the scene or places player at the origin
    /// </summary>
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
            DataManager.ClearSavedData();
        }
    }

    /// <summary>
    /// Called when the player is hit by a light source that is stronger than him
    /// </summary>
    public override void Knockback(LightSource enemyLightSource)
    {
        // Calculate a knockback force pushing the player away from the enemy fish
        Vector2 distance = (Transform.position - enemyLightSource.Transform.position);
        Vector2 knockback = distance.normalized * knockbackForce;

        Rigidbody.velocity = Vector3.zero;
        Rigidbody.AddForce(knockback, ForceMode.Impulse);

        // If the player was hit by a fish
        if (enemyLightSource.CompareTag("Fish"))
        {
            // Instantiate hit particles
            GameObject.Instantiate(fishHitParticles, transform.position, Quaternion.Euler(0, 0, 0));
            FlashColor(probeColorHit, hitFlashDuration);

            // Rumble the controller when the player hits a fish.
            controllerRumble.PlayerHitByFish();
        }

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
            // Debug.Log("PLAYER CAN'T BE ABSORBED: " + (Time.deltaTime - lastTimeHit));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Listens for input related to movement of the player
    /// </summary>
    private void Move()
    {
        // Ensure that the rigidbody never spins
        this.Rigidbody.angularVelocity = Vector3.zero;

        float thrustAxis = Input.GetAxis("ThrustAxis");
        float brakeAxis = Input.GetAxis("BrakeAxis");

        if (Input.GetButtonDown("Thrust") || (previousThrustAxis == 0 && thrustAxis > 0))
        {
            movement.OnPropulsionStart();
            lightToggle.OnPropulsionStart();
            this.ChangeColor(probeColorOn, true, 0);
        }

        if (Input.GetButton("Thrust"))
        {
            movement.Propulse(-massEjectionTransform.up);
        }

        if (thrustAxis != 0)
        {
            // Propulse in the direction of the left stick (opposite to the rear of the probe)
            movement.Propulse(-massEjectionTransform.up, thrustAxis);
        }

        if (Input.GetButtonUp("Thrust") || (previousThrustAxis > 0 && thrustAxis == 0))
        {
            movement.OnPropulsionEnd();
            lightToggle.OnPropulsionEnd();
            if (!lightToggle.LightButtonPressed)
                this.ChangeColor(probeColorOff, true, 0);
        }

        // Brake
        if (Input.GetButton("Brake"))
        {
            movement.Brake(1);
        }
        if (brakeAxis != 0)
        {
            movement.Brake(brakeAxis);
        }

        if (isDead)
        {
            // Slow down gravity;
            Rigidbody.AddForce(Vector3.up * 20, ForceMode.Force);
        }

        // Makes the character follow the left stick's rotation
        movement.FollowLeftStickRotation();

        // Ensure that the rigidbody never spins
        this.Rigidbody.angularVelocity = Vector3.zero;

        previousThrustAxis = thrustAxis;
    }

    private void OnConsumedLightSource(LightSource consumedLightSource)
    {
        // If the player ate a fish        
        if (consumedLightSource.CompareTag("Fish"))
        {
            FlashColor(probeColorEatFish, eatFlashDuration);
        }
        
        // If the player collected a pickup (yellow ball)       
        if (consumedLightSource.CompareTag("Pickup"))
        {
            FlashColor(probeColorEatPickup, eatFlashDuration);
        }
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Player has collided upon death
        if (isDead && !deathParticlesPlayed && playerDeathParticles != null)
        {
            // Calculate the angle of the player's velocity upon impact
            float crashAngle = Mathf.Rad2Deg * Mathf.Atan2(Rigidbody.velocity.y, Rigidbody.velocity.x);
            // Orient the explosion opposite to the player's velocity
            float explosionAngle = crashAngle + 180;
            // Spawn the explosion
            ParticleSystem explosion = GameObject.Instantiate(playerDeathParticles,
                                        Transform.position, Quaternion.Euler(-90, explosionAngle, 0)) as ParticleSystem;
            // Rumble the controller
            controllerRumble.PlayerDied();

            Transform.localScale = Vector3.zero;
            Rigidbody.isKinematic = true;

            // Only play the death particles the first time the player crashes on an obstacle
            deathParticlesPlayed = true;
            
            this.transform.FindChild("ProbeModel").gameObject.SetActive(false); //remove bubbles on death
        }
    }

    /// <summary>
    /// Listens for restart button clicks
    /// </summary>
    private void RestartGame()
    {
        if (Input.GetButtonDown("Restart"))
        {
            
            Debug.Log("Game Restarted");
            gameOverCanvas.SetActive(false);
            Transform.localScale = new Vector3(1, 1, 1);
            Rigidbody.isKinematic = false;
            Rigidbody.useGravity = false;

            this.LightEnergy.Add(this.DefaultEnergy);
            this.isDead = false;
            this.deathParticlesPlayed = false;
            this.Rigidbody.drag = defaultDrag; // reset drag
            this.transform.FindChild("ProbeModel").gameObject.SetActive(true); //reactivate bubbles
            ReactivateObjects();
            
            LoadGame();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    
    private void ReactivateObjects()
    {
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (GameObject pickup in pickups)
        {
            Debug.Log(pickup);
            LightSource light = pickup.GetComponent<LightSource>();
            if (light != null)
            {
                light.LightEnergy.Add(light.DefaultEnergy);
            }
            pickup.SetActive(true);
        }
        ObjectPooler.current.ResetPool();
    }

    /// <summary>
    /// Helper method to validate parameters passed through Unity EditorApplication
    /// In case of missing asset, shows debug error and halts the game
    /// </summary>
    private void ValidateInputs()
    {
        #if UNITY_EDITOR
            if (massEjectionTransform == null || lightBallPrefab == null || jetFuelEffect == null)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogError("Missing prefab on Player object!");
            }

            if (this.transform.Find("LightsToToggle").gameObject == null)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                Debug.LogError("Could not find LightsToToggle object!");
            }
        #endif
    }
    
    public PlayerMovement Movement
    {
        get { return movement; }
        set { movement = value; }
    }

    public void MaxSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }
}