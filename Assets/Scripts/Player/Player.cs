using UnityEngine;
using UnityEngine.SceneManagement;

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
    [Tooltip("The position at which the character ejects mass.")]
    private Transform massEjectionTransform;    
    
    [SerializeField]            
    [Tooltip("The light ball ejected by the player when thrusting")]
    private GameObject lightBallPrefab;

    [SerializeField]
    [Tooltip("The player's max speed in m/s")]
    private float maxSpeed = 100;

    [SerializeField]
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass.")]
    private float thrustForce = 0;

    [SerializeField]
    [Tooltip("The higher the value, the faster the propulsion when changing directions")]
    private float changeDirectionBoost = 0;

    [SerializeField]
    [Tooltip("The amount of light energy spent when ejecting one piece of mass.")]
    private float thrustEnergyCost = 1;
    
    [SerializeField]
    [Tooltip("The parent of the propulsion particle effects activated when the player is propulsing")]
    private GameObject jetFuelEffect;


    [Header("Player Lights")]
    [SerializeField]
    [Tooltip("If true, the lights are enabled on scene start.")]
    private bool defaultLightStatus = true;

    [SerializeField]
    [Tooltip("Time interval for lost of lights while lights are on")]
    private float lostOfLightTime = 0;

    [SerializeField]
    [Tooltip("Amount of light lost while lights are turned on")]
    private float energyCostLightToggle = 0;

    [SerializeField]
    [Tooltip("Energy needed to activate light and that light will turn off if reached")]
    private float minimalEnergyRestrictionToggleLights = 0;

    [SerializeField]
    [Tooltip("If true, checkpoints are not used and user is spawned at the initial position")]
    private bool disableCheckpoints = true;
        
        
    private PlayerMovement movement;
    private PlayerLightToggle lightToggle;
    private MaterialExtensions materials;
    private new Transform transform;
    private new Rigidbody rigidbody;    
    private int currentLevel;
    private bool isDead;
    
    /// <summary>
    /// Initializes Player components   
    /// </summary>
    public override void Awake()
    {
        // Call parent LightSource Awake() first
        base.Awake();
        
        this.isDead = false;
        this.currentLevel = SceneManager.GetActiveScene().buildIndex;        
        this.transform = GetComponent<Transform>();
        this.rigidbody = GetComponent<Rigidbody>();
        this.materials = new MaterialExtensions();
        
        this.ValidateInputs();               
        this.movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, changeDirectionBoost, thrustEnergyCost, transform, rigidbody, this.LightEnergy, this.jetFuelEffect);
        this.lightToggle = new PlayerLightToggle(transform.Find("LightsToToggle").gameObject, defaultLightStatus, this, minimalEnergyRestrictionToggleLights);    
              
        DontDestroyOnLoad(this.gameObject);                
        ChangeProbeColor(Color.black);
        LoadGame();
    }
    
    /// <summary>
    /// Listens for player states such as movement, light controls and death
    /// Called once per frame
    /// </summary>
    public override void Update()
    {
        base.Update(); 
                      
        if (this.isDead)
        {
            RestartGame();
        }
        else
        {
            Move();
            LightControl();
            
            // Clamp the player's velocity
            if (rigidbody.velocity.sqrMagnitude > this.maxSpeed * this.maxSpeed)
            {
                rigidbody.velocity = ((Vector2)rigidbody.velocity).SetMagnitude(this.maxSpeed);
            }
        }
    }
        
    /// <summary>
    /// Invoked when a new scene is loaded
    /// </summary>
    /// <param name="level">id of the scene that is loaded</param>
    protected void OnLevelWasLoaded(int level) 
    {
        Debug.Log("Scene " + level + " is loaded!");
        this.transform.position = new Vector3(0, 0, 0); 
    }
          
    /// <summary>
    /// Sets player state to 'dead' when LightDepleted event is triggered
    /// </summary>
    protected override void OnLightDepleted()
    {
        base.OnLightDepleted();         
        movement.OnPropulsionEnd();
        isDead = true;
        Debug.Log("Game OVER! Press 'R' to restart!");
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
    /// Smoothly changes color of the player avatar to the given one 
    /// </summary>
    /// <param name="color"></param>
    private void ChangeProbeColor(Color color)
    {
        foreach (GameObject probe in GameObject.FindGameObjectsWithTag("Probe"))
        {            
            Renderer renderer = probe.GetComponent<Renderer>();
            foreach (Material mat in renderer.materials)
            {
                StartCoroutine(materials.LerpColor(mat, color, 0.3f));
            }                    
        }                    
    }

    /// <summary>
    /// Listens for lights button clicks
    /// </summary>
    private void LightControl()
    {
        if (lightToggle != null)
        {
            if (Input.GetButtonDown("LightToggle") && minimalEnergyRestrictionToggleLights < this.LightEnergy.CurrentEnergy)
            {
                lightToggle.ToggleLights();
                if (lightToggle.LightsEnabled)
                {
                    this.ChangeProbeColor(new Color(1f, 0.3103448f, 0f, 1f));
                }
                else
                {
                    this.ChangeProbeColor(Color.black);
                }                                                 
            }
            lightToggle.LostOfLight(lostOfLightTime, energyCostLightToggle);   
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
    /// Listens for input related to movement of the player 
    /// </summary>
    private void Move()
    {        
        // Ensure that the rigidbody never spins
        rigidbody.angularVelocity = Vector3.zero;
        
        if (movement != null) 
        {         
            if (Input.GetButtonDown("Thrust")) { movement.OnPropulsionStart(); }        
            if (Input.GetButton("Thrust"))     { movement.Propulse(-massEjectionTransform.up); }       
            if (Input.GetButtonUp("Thrust"))   { movement.OnPropulsionEnd(); }
            
            movement.FollowLeftStickRotation();            
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
            this.LightEnergy.Add(this.DefaultEnergy);
            this.isDead = false;
            this.rigidbody.drag = 0; // reset drag
            LoadGame();
        }
    }
 
    /// <summary>
    /// Helper method to validate parameters passed through Unity EditorApplication
    /// In case of missing asset, shows debug error and halts the game
    /// </summary>
    private void ValidateInputs()
    {
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
    }
 
}