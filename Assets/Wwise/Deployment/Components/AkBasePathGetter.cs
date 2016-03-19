#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

public class AkBasePathGetter
{
	static public string GetPlatformName()
	{
        try
        {
#if UNITY_WSA && !UNITY_EDITOR
            TypeInfo CustomNameGetter = null;
            CustomNameGetter = Type.GetType("AkCustomPlatformNameGetter").GetTypeInfo();
#else
            Type CustomNameGetter = null;
            CustomNameGetter = Type.GetType("AkCustomPlatformNameGetter");
#endif
            if (CustomNameGetter != null)
            {
                MethodInfo mi = null;
#if UNITY_WSA && !UNITY_EDITOR
                mi = CustomNameGetter.GetDeclaredMethod("GetPlatformName");
#else
                mi = CustomNameGetter.GetMethod("GetPlatformName");
#endif
                if (mi != null)
                {
                    string PlatformName = (string)mi.Invoke(null, null);
					if( !string.IsNullOrEmpty(PlatformName) )
					{
						return PlatformName;
					}
                }
            }
        }
        catch
        {
            // If anything fails, return the default platform name.
        }
        
        string platformSubDir = "Undefined platform sub-folder";        
		
#if UNITY_EDITOR_WIN
        // NOTE: Due to a Unity3.5 bug, projects upgraded from 3.4 will have malfunctioning platform preprocessors
		// We have to use Path.DirectorySeparatorChar to know if we are in the Unity Editor for Windows or Mac.
        platformSubDir = "Windows";
#elif UNITY_EDITOR_OSX
		platformSubDir = "Mac";
#elif UNITY_STANDALONE_WIN
		platformSubDir = Path.DirectorySeparatorChar == '/' ? "Mac" : "Windows";
#elif UNITY_STANDALONE_LINUX
		platformSubDir = "Linux";
#elif UNITY_STANDALONE_OSX
		platformSubDir = Path.DirectorySeparatorChar == '/' ? "Mac" : "Windows";
#elif UNITY_XBOX360
		platformSubDir = "XBox360";
#elif UNITY_XBOXONE
		platformSubDir = "XBoxOne";
#elif UNITY_IOS
		platformSubDir = "iOS";
#elif UNITY_ANDROID
		platformSubDir = "Android";
#elif UNITY_PS3
		platformSubDir = "PS3";
#elif UNITY_PS4
		platformSubDir = "PS4";
#elif UNITY_WP_8_1
        platformSubDir = "WindowsPhone";
#elif UNITY_WSA
	    platformSubDir = "Windows";
#elif UNITY_WIIU
	    platformSubDir = "WiiUSW";
#elif UNITY_PSP2
#if AK_ARCH_VITA_SW
        platformSubDir = "VitaSW";
#elif AK_ARCH_VITA_HW
		platformSubDir = "VitaHW";
#else
		platformSubDir = "VitaSW";
#endif
#endif
        return platformSubDir;
	}

    /// Returns the full base path
    public static string GetPlatformBasePath()
    {
        string platformBasePath = string.Empty;
#if UNITY_EDITOR
        platformBasePath = GetPlatformBasePathEditor();
        if (!string.IsNullOrEmpty(platformBasePath))
        {
            return platformBasePath;
        }
#endif
        // Combine base path with platform sub-folder
        platformBasePath = Path.Combine(GetFullSoundBankPath(), GetPlatformName());

        FixSlashes(ref platformBasePath);

        return platformBasePath;
    }

    public static string GetFullSoundBankPath()
    {
        // Get full path of base path
#if UNITY_ANDROID && !UNITY_EDITOR
 		string fullBasePath = AkInitializer.GetBasePath();
#else
        string fullBasePath = Path.Combine(Application.streamingAssetsPath, AkInitializer.GetBasePath());
#endif
        FixSlashes(ref fullBasePath);
        return fullBasePath;
    }


#if UNITY_EDITOR
    static string GetPlatformBasePathEditor()
    {
        try
        {
            WwiseSettings Settings = WwiseSettings.LoadSettings();
            string platformSubDir = Path.DirectorySeparatorChar == '/' ? "Mac" : "Windows";
            string WwiseProjectFullPath = AkUtilities.GetFullPath(Application.dataPath, Settings.WwiseProjectPath);
            string SoundBankDest = AkUtilities.GetWwiseSoundBankDestinationFolder(platformSubDir, WwiseProjectFullPath);
            if (Path.GetPathRoot(SoundBankDest) == "")
            {
                // Path is relative, make it full
                SoundBankDest = AkUtilities.GetFullPath(Path.GetDirectoryName(WwiseProjectFullPath), SoundBankDest);
            }

            // Verify if there are banks in there
            DirectoryInfo di = new DirectoryInfo(SoundBankDest);
            FileInfo[] foundBanks = di.GetFiles("*.bnk", SearchOption.AllDirectories);
            if (foundBanks.Length == 0)
            {
                SoundBankDest = string.Empty;
            }

            if (!SoundBankDest.Contains(GetPlatformName()))
            {
                Debug.LogWarning("WwiseUnity: The platform SoundBank subfolder does not match your platform name. You will need to create a custom platform name getter for your game. See section \"Using Wwise Custom Platforms in Unity\" of the Wwise Unity integration documentation for more information");
            }

            return SoundBankDest;
        }
        catch
        {
            return string.Empty;
        }
    }
#endif

    public static void FixSlashes(ref string path)
    {
#if !UNITY_WSA
        string seperatorChar = Path.DirectorySeparatorChar.ToString();
        string badChar = string.Empty;
        if (Path.DirectorySeparatorChar == '/')
        {
            badChar = "\\";
        }
        else
        {
            badChar = "/";
        }
#else
        string seperatorChar = "\\";
        string badChar = "/";
#endif // #if !UNITY_WSA


        path.Trim();
        path = path.Replace(badChar, seperatorChar);
        path = path.TrimStart('\\');

        // Append a trailing slash to play nicely with Wwise
        if (!path.EndsWith(seperatorChar))
        {
            path += seperatorChar;
        }
    }


    // This looks for a valid SoundBank path for all Custom platforms for the current base platform.
    // If you are using custom platforms, you probably will need to modify the code to this function.
    public static string GetValidBasePath()
    {
        string basePathToSet = GetPlatformBasePath();
        bool InitBnkFound = true;
#if !(UNITY_ANDROID && !UNITY_EDITOR) // Can't use File.Exists on Android, assume banks are there
        string InitBankPath = Path.Combine(basePathToSet, "Init.bnk");
        if (!File.Exists(InitBankPath))
        {
            InitBnkFound = false;
        }
#endif

        if (basePathToSet == string.Empty || InitBnkFound == false)
        {
#if !UNITY_EDITOR
            Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to copy them to the StreamingAssets folder?");
#else
            Debug.LogError("WwiseUnity: Could not locate the SoundBanks. Did you make sure to generate them?");
#endif
            return string.Empty;
        }

        Debug.Log("WwiseUnity: Setting base SoundBank path to " + basePathToSet);

        return basePathToSet;
    }

}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.