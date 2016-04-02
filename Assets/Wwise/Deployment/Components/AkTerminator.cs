#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading;
#pragma warning disable 0219, 0414


[AddComponentMenu("Wwise/AkTerminator")]
/// This script deals with termination of the Wwise audio engine.  
/// It must be present on one Game Object that gets destroyed last in the game.
/// It must be executed AFTER any other monoBehaviors that use AkSoundEngine.
/// \sa
/// - \ref workingwithsdks_termination
/// - AK::SoundEngine::Term()
public class AkTerminator : MonoBehaviour
{
	static private AkTerminator ms_Instance = null;

	void Awake()
	{
		if (ms_Instance != null)
		{			
			//Check if there are 2 objects with this script.  If yes, remove this component.
			if (ms_Instance != this)
				Object.DestroyImmediate(this);
            return; 
		}

		DontDestroyOnLoad(this);
		ms_Instance = this;		
	}	
	
	void OnApplicationQuit() 
	{
		//This happens before OnDestroy.  Stop the sound engine now.
		Terminate();
		
		// NOTE: AkCallbackManager needs to handle last few events after sound engine terminates
		// So it has to terminate after sound engine does.  See OnDestroy.
	}
	
	void OnDestroy()
    {   
		if (ms_Instance == this)
			ms_Instance = null;        
    }
	
	void Terminate()
	{

		if (ms_Instance == null || ms_Instance != this || !AkSoundEngine.IsInitialized())
			return; //Don't term twice        
				
				
		// Stop everything, and make sure the callback buffer is empty. We try emptying as much as possible, and wait 10 ms before retrying.
		// Callbacks can take a long time to be posted after the call to RenderAudio().
        AkCallbackManager.SetMonitoringCallback(0, null);
		AkSoundEngine.StopAll();
        AkSoundEngine.ClearBanks();
		AkSoundEngine.RenderAudio();
        int retry = 5;
        do
        {
            int numCB = 0;
			do
			{
				numCB = AkCallbackManager.PostCallbacks();
				
				// This is a WSA-friendly sleep
	            using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
	            {
	                tmpEvent.WaitOne(System.TimeSpan.FromMilliseconds(1));
	            }
			}
			while(numCB > 0);

			// This is a WSA-friendly sleep
            using (EventWaitHandle tmpEvent = new ManualResetEvent(false))
            {
                tmpEvent.WaitOne(System.TimeSpan.FromMilliseconds(10));
            }
            retry--;
        }
        while (retry > 0);

		AkSoundEngine.Term();

		// Make sure we have no callbacks left after Term. Some might be posted during termination.
        AkCallbackManager.PostCallbacks();
        ms_Instance = null;

		AkCallbackManager.Term();
		AkBankManager.Reset ();
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.