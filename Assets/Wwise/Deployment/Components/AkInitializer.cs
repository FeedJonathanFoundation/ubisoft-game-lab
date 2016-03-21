#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

#pragma warning disable 0219, 0414

[AddComponentMenu("Wwise/AkInitializer")]
/// This script deals with initialization, and frame updates of the Wwise audio engine.  
/// It is marked as \c DontDestroyOnLoad so it stays active for the life of the game, 
/// not only one scene. You can, and probably should, modify this script to change the 
/// initialization parameters for the sound engine. A few are already exposed in the property inspector.
/// It must be present on one Game Object at the beginning of the game to initialize the audio properly.
/// It must be executed BEFORE any other MonoBehaviors that use AkSoundEngine.
/// \sa
/// - \ref workingwithsdks_initialization
/// - AK::SoundEngine::Init()
/// - AK::SoundEngine::Term()
/// - AkCallbackManager
[RequireComponent(typeof(AkTerminator))]
public class AkInitializer : MonoBehaviour
{
    public readonly static string c_DefaultBasePath = Path.Combine("Audio", "GeneratedSoundBanks");
    ///Path for the soundbanks. This must contain one sub folder per platform, with the same as in the Wwise project.
    public string basePath = c_DefaultBasePath;

    public const string c_Language = "English(US)";
    /// Language sub-folder. 
    public string language = c_Language;

    public const int c_DefaultPoolSize = 4096;
    ///Default Pool size.  This contains the meta data for your audio project.  Default size is 4 MB, but you should adjust for your needs.
    public int defaultPoolSize = c_DefaultPoolSize;

    public const int c_LowerPoolSize = 2048;
    ///Lower Pool size.  This contains the audio processing buffers and DSP data.  Default size is 2 MB, but you should adjust for your needs.
    public int lowerPoolSize = c_LowerPoolSize;

    public const int c_StreamingPoolSize = 1024;
    ///Streaming Pool size.  This contains the streaming buffers.  Default size is 1 MB, but you should adjust for your needs.
    public int streamingPoolSize = c_StreamingPoolSize;

	public const int c_PreparePoolSize = 0;
    ///Prepare Pool size.  This contains the banks loaded using PrepareBank (Banks decoded on load use this).  Default size is 0 MB, but you should adjust for your needs.
	public int preparePoolSize = c_PreparePoolSize;

    public const float c_MemoryCutoffThreshold = 0.95f;
    ///This setting will trigger the killing of sounds when the memory is reaching 95% of capacity.  Lowest priority sounds are killed.
    public float memoryCutoffThreshold = c_MemoryCutoffThreshold;
    
    public const bool c_EngineLogging = true;
    ///Enable Wwise engine logging. Option to turn on/off the logging of the Wwise engine.
    public bool engineLogging = c_EngineLogging;
    
    public static string GetBasePath()
    {
#if UNITY_EDITOR
        return WwiseSettings.LoadSettings().SoundbankPath;
#else
        return ms_Instance.basePath;
#endif
    }

    public static string GetCurrentLanguage()
    {
        return ms_Instance.language;
    }

    void Awake()
    {
		Initialize();
	}
	
	public void Initialize()
	{
        if (ms_Instance != null)
        {
            //Don't init twice
            //Check if there are 2 objects with this script.  If yes, remove this component.
            if (ms_Instance != this)
                UnityEngine.Object.DestroyImmediate(this.gameObject);
            return;
        }

        Debug.Log("WwiseUnity: Initialize sound engine ...");

        //Use default properties for most SoundEngine subsystem.  
        //The game programmer should modify these when needed.  See the Wwise SDK documentation for the initialization.
        //These settings may very well change for each target platform.
        AkMemSettings memSettings = new AkMemSettings();
        memSettings.uMaxNumPools = 20;

        AkDeviceSettings deviceSettings = new AkDeviceSettings();
        AkSoundEngine.GetDefaultDeviceSettings(deviceSettings);

        AkStreamMgrSettings streamingSettings = new AkStreamMgrSettings();
        streamingSettings.uMemorySize = (uint)streamingPoolSize * 1024;

        AkInitSettings initSettings = new AkInitSettings();
        AkSoundEngine.GetDefaultInitSettings(initSettings);
        initSettings.uDefaultPoolSize = (uint)defaultPoolSize * 1024;

        AkPlatformInitSettings platformSettings = new AkPlatformInitSettings();
        AkSoundEngine.GetDefaultPlatformInitSettings(platformSettings);
        platformSettings.uLEngineDefaultPoolSize = (uint)lowerPoolSize * 1024;
        platformSettings.fLEngineDefaultPoolRatioThreshold = memoryCutoffThreshold;

        AkMusicSettings musicSettings = new AkMusicSettings();
        AkSoundEngine.GetDefaultMusicSettings(musicSettings);

// Unity 5 only, UNity 4 doesn't provide a way to access the product name at runtime.
#if UNITY_5
#if UNITY_EDITOR
        AkSoundEngine.SetGameName(Application.productName + " (Editor)");
#else
		AkSoundEngine.SetGameName(Application.productName);
#endif
#endif

        AKRESULT result = AkSoundEngine.Init(memSettings, streamingSettings, deviceSettings, initSettings, platformSettings, musicSettings, (uint)preparePoolSize*1024);
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed to initialize the sound engine. Abort.");
            return; //AkSoundEngine.Init should have logged more details.
        }

        ms_Instance = this;

        string basePathToSet = AkBasePathGetter.GetValidBasePath();
        if (string.IsNullOrEmpty(basePathToSet))
        {
            return;
        }

		result = AkSoundEngine.SetBasePath(basePathToSet);
		if (result != AKRESULT.AK_Success)
		{
			return;
		}
		
        AkSoundEngine.SetCurrentLanguage(language);

        result = AkCallbackManager.Init();
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed to initialize Callback Manager. Terminate sound engine.");
            AkSoundEngine.Term();
            ms_Instance = null;
            return;
        }

        AkBankManager.Reset ();

        Debug.Log("WwiseUnity: Sound engine initialized.");

        //The sound engine should not be destroyed once it is initialized.
        DontDestroyOnLoad(this);

#if UNITY_EDITOR
        //Redirect Wwise error messages into Unity console.
        AkCallbackManager.SetMonitoringCallback(ErrorLevel.ErrorLevel_All, CopyMonitoringInConsole);
#endif

        //Load the init bank right away.  Errors will be logged automatically.
        uint BankID;
        result = AkSoundEngine.LoadBank("Init.bnk", AkSoundEngine.AK_DEFAULT_POOL_ID, out BankID);
        if (result != AKRESULT.AK_Success)
        {
            Debug.LogError("WwiseUnity: Failed load Init.bnk with result: " + result.ToString());
        }

#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged += OnEditorPlaymodeStateChanged;
#endif
    }

    void OnDestroy()
    {
        if (ms_Instance == this)
        {
#if UNITY_EDITOR
            EditorApplication.playmodeStateChanged -= OnEditorPlaymodeStateChanged;
#endif

            AkCallbackManager.SetMonitoringCallback(0, null);
            ms_Instance = null;
        }
        // Do nothing. AkTerminator handles sound engine termination.

    }

    void OnEnable()
    {
        //The sound engine was not terminated normally.  Make this instance the one that will manage
        //the updates and termination.
        //This happen when Unity resets everything when a script changes.
        if (ms_Instance == null && AkSoundEngine.IsInitialized())
        {
            ms_Instance = this;
#if UNITY_EDITOR
            //Redirect Wwise error messages into Unity console.
            AkCallbackManager.SetMonitoringCallback(ErrorLevel.ErrorLevel_All, CopyMonitoringInConsole);
#endif
        }
    }

    //Use LateUpdate instead of Update() to ensure all gameobjects positions, listener positions, environements, RTPC, etc are set before finishing the audio frame.
    void LateUpdate()
    {
        //Execute callbacks that occured in last frame (not the current update)     
        if (ms_Instance != null)
        {
            AkCallbackManager.PostCallbacks();
            AkBankManager.DoUnloadBanks();
            AkSoundEngine.RenderAudio();
        }
    }

#if UNITY_EDITOR

    void OnDisable()
    {
        // Unregister the callback that redirects the output to the Unity console.  If not done early enough (meaning, later than Disable), AkInitializer will leak.
        if (ms_Instance != null && AkSoundEngine.IsInitialized())
            AkCallbackManager.SetMonitoringCallback(0, null);
    }

    
    void CopyMonitoringInConsole(ErrorCode in_errorCode, ErrorLevel in_errorLevel, uint in_playingID, IntPtr in_gameObjID, string in_msg)
    {
        // Only log when logging from the engine is enabled. The callback remains active when the flag is disabled to ensure
        // it can be toggled on and off in a lively manner in Unity.
        if (engineLogging)
        {
            string msg = "Wwise: " + in_msg;
            if (in_gameObjID != (IntPtr)AkSoundEngine.AK_INVALID_GAME_OBJECT)
            {
                GameObject obj = EditorUtility.InstanceIDToObject((int)in_gameObjID) as GameObject;
                string name = obj != null ? obj.ToString() : in_gameObjID.ToString();
                msg += "(Object: " + name + ")";
            }

            if (in_errorLevel == ErrorLevel.ErrorLevel_Error)
                Debug.LogError(msg);
            else
                Debug.Log(msg);
        }
    }
#endif

    //
    // Private members
    //

    private static AkInitializer ms_Instance;

#if !UNITY_EDITOR && !UNITY_WIIU
	//On the WiiU, it seems Unity has a bug and never calls OnApplicationFocus(true).  This leaves us in "suspended mode".  So commented out for now.
	void OnApplicationPause(bool pauseStatus) 
	{
		if (ms_Instance != null)
		{
			if ( !pauseStatus )
			{
				AkSoundEngine.WakeupFromSuspend();
			}
			else
			{
				AkSoundEngine.Suspend();             
			}
			AkSoundEngine.RenderAudio();
		}        
	}
	
    void OnApplicationFocus(bool focus)
    {
        if (ms_Instance != null)
        {
            if ( focus )
            {
                AkSoundEngine.WakeupFromSuspend();
            }
            else
            {
				AkSoundEngine.Suspend();             
            }
            AkSoundEngine.RenderAudio();
        }
    }
#endif

#if UNITY_EDITOR
    // Enable/Disable the audio when pressing play/pause in the editor.
    private static void OnEditorPlaymodeStateChanged()
    {
        if (ms_Instance != null)
        {
            if (EditorApplication.isPaused)
            {
                AkSoundEngine.Suspend();
            }
            else
            {
                AkSoundEngine.WakeupFromSuspend();
            }

            AkSoundEngine.RenderAudio();
        }
    }
#endif

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.