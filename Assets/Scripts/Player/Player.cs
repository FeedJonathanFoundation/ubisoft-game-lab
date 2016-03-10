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
    /// The light ball ejected by the player when thrusting
    /// </summary>
    public GameObject lightBallPrefab;

    /// <summary>
    /// The amount of force applied on the player when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of force applied on the player when ejecting one piece of mass.")]
    public float thrustForce;

    /// <summary>
    /// The amount of light energy spent when ejecting one piece of mass.
    /// </summary>
    [Tooltip("The amount of light energy spent when ejecting one piece of mass.")]
    public float thrustEnergyCost = 1;

    [Tooltip("If true, the lights are enabled on scene start.")]
    public bool defaultLightStatus = true;

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
    private bool isDead; // determines is current player is dead
    private new Transform transform;
    private new Rigidbody rigidbody;
    private int currentLevel;

    // Use this for initialization
    public override void Awake()
    {
       base.Awake(); // call parent LightSource Awake() first
       this.transform = GetComponent<Transform>();
       this.rigidbody = GetComponent<Rigidbody>();       
       this.movement = new PlayerMovement(massEjectionTransform, lightBallPrefab, thrustForce, thrustEnergyCost, transform, rigidbody, this.LightEnergy);
       this.lightToggle = new PlayerLightToggle(transform.Find("LightsToToggle").gameObject, defaultLightStatus, this, minimalEnergyRestrictionToggleLights);
       this.LightEnergy.LightDepleted += OnLightDepleted;
       this.isDead = false;
       this.currentLevel = SceneManager.GetActiveScene().buildIndex;
       DontDestroyOnLoad(this.transform.gameObject);
       LoadGame();
    }
    
    void OnLevelWasLoaded(int level) 
    {
        Debug.Log("Scene " + level + " is loaded!");
        this.transform.position = new Vector3(0, 0, 0); 
    }

    // Update is called once per frame
    void Update()
    {
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
            // Eject mass in the direction of the left stick
            movement.EjectMass(massEjectionTransform.up);
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
        lightToggle.UpdateLossOfLight(lostOfLightTime, energyCostLightToggle);
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

    /// <summary>
    /// Listens for LightDepleted event from LightEnergy
    /// Sets player to dead when it occurs
    /// </summary>
    public void OnLightDepleted()
    {
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