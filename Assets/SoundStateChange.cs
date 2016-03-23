﻿using UnityEngine;
using System.Collections;

public class SoundStateChange : MonoBehaviour
{

    [Tooltip("State of player before he enters")]
    [SerializeField]
    private int initialState = 0;
    [Tooltip("Whether the trigger is a safe zone.")]
    [SerializeField]
    private bool safeZone = false;
    protected static int currentState = 0;

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
                // Stop playing sound
            }
        }
    }

    private void SetState(int newState)
    {
        switch (newState)
        {
            case 0:
                AkSoundEngine.SetState("IMAmb1", "Amb1start");
                break;
            case 1:
                AkSoundEngine.SetState("IMAmb1", "Amb1end");
                break;
            case 2:
                AkSoundEngine.SetState("IMAmb2", "CP1");
                break;
            case 3:
                AkSoundEngine.SetState("IMAmb2", "CP2");
                break;
            case 4:
                AkSoundEngine.SetState("IMAmb2", "CP3");
                break;
            // case 5:
                // AkSoundEngine.SetState("IMAmb3", "");
            default:
                break;
            
        }
    }
    
    private void SafeZoneSound()
    {
        // Needs to change to safe zone music
        AkSoundEngine.SetState("IMAmb2", "CP2");
    }
}

