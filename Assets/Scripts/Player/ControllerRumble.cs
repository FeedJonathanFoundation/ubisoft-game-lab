using UnityEngine;
using System.Collections;
using XInputDotNetPure;

/// <summary>
/// Most of the code was copied from the example XInputDotNet script
/// </summary>

public class ControllerRumble : MonoBehaviour
{
    [Tooltip("The rumble intensity when a flare is shot")]
    [SerializeField]
    private float flareRumbleIntensity = 0.7f;
    [Tooltip("The duration of rumbling when a flare is shot")]
    [SerializeField]
    private float flareRumbleTime = 0.2f;

    [Tooltip("The rumble intensity when the player eats a fish")]
    [SerializeField]
    private float eatFishRumbleIntensity = 0.5f;
    [Tooltip("The duration of rumbling when the player eats a fish")]
    [SerializeField]
    private float eatFishRumbleTime = 0.1f;

    [Tooltip("The rumble intensity when the player is hit by a fish")]
    [SerializeField]
    private float hitFishRumbleIntensity = 1.0f;
    [Tooltip("The duration of rumbling when the player is hit by a fish")]
    [SerializeField]
    private float hitFishRumbleTime = 0.2f;

    [Tooltip("The rumble intensity when the player explodes upon death")]
    [SerializeField]
    private float playerDeadRumbleIntensity = 1.0f;
    [Tooltip("The duration of rumbling when the player explodes upon death")]
    [SerializeField]
    private float playerDeadRumbleTime = 0.3f;

    private bool playerIndexSet = false;
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamePadState prevState;
    private Player player;
    private bool isJoystickConnected;

    void Awake()
    {
        this.player = GetComponent<Player>();
        this.isJoystickConnected = (Input.GetJoystickNames().Length > 0);
    }

    void OnEnable()
    {
        // Subscribe to the player's events to listen for rumble opportunities
        player.ConsumedLightSource += PlayerAteFish;
    }

    void OnDisable()
    {
        // Unsubscribe to the player's events which listen for rumble opportunities
        player.ConsumedLightSource -= PlayerAteFish;
    }

    void Update()
    {
        if (this.isJoystickConnected)
        {
            // Find a PlayerIndex, for a single player game
            // Will find the first controller that is connected ans use it
            if (!playerIndexSet || !prevState.IsConnected)
            {
                for (int i = 0; i < 4; ++i)
                {
                    PlayerIndex testPlayerIndex = (PlayerIndex)i;
                    GamePadState testState = GamePad.GetState(testPlayerIndex);
                    if (testState.IsConnected)
                    {
                        Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
                        playerIndex = testPlayerIndex;
                        playerIndexSet = true;
                    }
                }
            }

            prevState = state;
            state = GamePad.GetState(playerIndex);

            // Detect if a button was pressed this frame
            if (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed)
            {

            }
            // Detect if a button was released this frame
            if (prevState.Buttons.A == ButtonState.Pressed && state.Buttons.A == ButtonState.Released)
            {
                //GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }

            // Set vibration according to triggers
            //GamePad.SetVibration(playerIndex, state.Triggers.Left, state.Triggers.Right);
        }
    }

    /// <summary>
    /// Rumbles the GamePad when the player shoots a flare
    /// </summary>
    public void ShotFlare()
    {
        if (this.isJoystickConnected)
        {
            StartCoroutine(Rumble(playerIndex, flareRumbleIntensity, flareRumbleIntensity, flareRumbleTime));
        }
    }

    /// <summary>
    /// Rumbles the GamePad when the player is hit by a fish
    /// </summary>
    public void PlayerHitByFish()
    {
        StartCoroutine(Rumble(playerIndex, hitFishRumbleIntensity, hitFishRumbleIntensity, hitFishRumbleTime));
    }

    /// <summary>
    /// Rumbles the controller when the player eats a fish
    /// </summary>
    public void PlayerAteFish(LightSource fish)
    {
        StartCoroutine(Rumble(playerIndex, eatFishRumbleIntensity, eatFishRumbleIntensity, eatFishRumbleTime));
    }

    /// <summary>
    /// Rumbles the controller when the player explodes upon death
    /// </summary>
    public void PlayerDied()
    {
        StartCoroutine(Rumble(playerIndex, playerDeadRumbleIntensity, playerDeadRumbleIntensity, playerDeadRumbleTime));
    }

    private IEnumerator Rumble(PlayerIndex playerIndex, float leftRumbleIntensity, float rightRumbleIntensity, float duration)
    {
        if (this.isJoystickConnected)
        {
            while (duration >= 0)
            {
                // Vibrate the gamepad at for the given duration
                GamePad.SetVibration(playerIndex, leftRumbleIntensity, rightRumbleIntensity);
                duration -= Time.deltaTime;
                yield return null;
            }

            // Stop vibrating once the duration is complete.
            GamePad.SetVibration(playerIndex, 0, 0);
        }
    }
}