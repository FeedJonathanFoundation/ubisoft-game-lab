using UnityEngine;
using System.Collections;

public class SoundStateChange : MonoBehaviour
{

    [Tooltip("State of player before he enters")]
    [SerializeField]
    private int initialState = 0;
    [Tooltip("Scene Number, from 0.")]
    [SerializeField]
    private int scene = 0;
    [Tooltip("Whether the trigger is a safe zone.")]
    [SerializeField]
    private bool safeZone = false;
    protected static int currentState = 0;

    void Start()
    {

        switch (scene)
        {
            case 0:
                currentState = scene;
                break;
            case 1:
                currentState = 1;
                break;
            case 2:
                currentState = 3;
                break;
            default:
                break;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!safeZone)
            {
                if (currentState == initialState)
                {
                    currentState++;
                    SetState(currentState);
                }
                else if (currentState == (initialState + 1))
                {
                    currentState--;
                    SetState(currentState);
                }
            }
            else
            {
                SafeZoneSound();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (safeZone)
        {
            if (other.CompareTag("Player"))
            {
                AkSoundEngine.SetState("IMAmb3", "Hunted");
            }
        }
    }

    private void SetState(int newState)
    {
        // Debug.Log("new state: " + newState);
        switch (newState)
        {
            case 0:// Field scene
                AkSoundEngine.SetState("IMAmb1", "Amb1start");
                break;
            case 1:
                AkSoundEngine.SetState("IMAmb1", "Amb1end");
                break;
            case 2: // Volcano scene
                AkSoundEngine.SetState("IMAmb2", "CP2");
                break;
            case 3:
                AkSoundEngine.SetState("IMAmb2", "CP3");
                break;
            case 4: // Beginning cave scene
                //AkSoundEngine.PostEvent("Ambient2Stop", this.gameObject);
                //AkSoundEngine.PostEvent("Ambient3", this.gameObject);
                AkSoundEngine.SetState("IMAmb3", "Hunted");
                break;
            case 5: // boss theme
                AkSoundEngine.SetState("IMAmb3", "Over");
                break;
            default:
                break;
            
        }
    }
    
    private void SafeZoneSound()
    {
        AkSoundEngine.SetState("IMAmb3", "Hiding");
    }
}
