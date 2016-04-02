#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.IO;

[InitializeOnLoad]
public class AkWwisePostImportCallback
{
    static AkWwisePostImportCallback()
    {
        EditorApplication.hierarchyWindowChanged += CheckWwiseGlobalExistance;
    }

	[DidReloadScripts(100000000)]
	static void RefreshCallback()
	{
        EditorApplication.delayCall += PostImportFunction;
        EditorApplication.delayCall += RefreshPlugins;
	}
	
	static void PostImportFunction()
    {
		EditorApplication.hierarchyWindowChanged += CheckWwiseGlobalExistance;
	
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return;
        }

		// Do nothing in batch mode
		string[] arguments = Environment.GetCommandLineArgs();
		if( Array.IndexOf(arguments, "-nographics") != -1 )
		{
			return;
		}
		
		try
		{            
			if (!File.Exists(Application.dataPath + Path.DirectorySeparatorChar + WwiseSettings.WwiseSettingsFilename))
			{
                WwiseSetupWizard.Init();
				return;
			}
			else
			{
				WwiseSetupWizard.Settings = WwiseSettings.LoadSettings();
				AkWwiseProjectInfo.GetData();
			}

			if( !string.IsNullOrEmpty(WwiseSetupWizard.Settings.WwiseProjectPath))
			{
				AkWwiseProjectInfo.Populate();
				AkWwisePicker.PopulateTreeview();
				if (AkWwiseProjectInfo.GetData().autoPopulateEnabled )
				{
                    AkWwiseWWUBuilder.StartWWUWatcher();
				}
			}
		}
		catch( Exception e)
		{
			Debug.Log(e.ToString());
		}

		//Check if a WwiseGlobal object exists in the current scene	
		CheckWwiseGlobalExistance();
		
		// If demo scene, remove file that should only be there on import
		string filename = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Wwise"), "Editor"), "WwiseSetupWizard"), "AkWwisePopPicker.cs");
		if( File.Exists(filename) )
		{
			EditorApplication.delayCall += DeletePopPicker;
		}
	}
	
	static void RefreshPlugins()
	{
#if !UNITY_5
		// Check if there are some new platforms to install.
		InstallNewPlatforms();
#else
		if( string.IsNullOrEmpty(AkWwiseProjectInfo.GetData().CurrentPluginConfig) )
		{
			AkWwiseProjectInfo.GetData().CurrentPluginConfig = AkPluginActivator.CONFIG_PROFILE;
		}
		AkPluginActivator.RefreshPlugins();
#endif
	}
	
	static void DeletePopPicker()
	{
		string basePath = Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Wwise"), "Editor"), "WwiseSetupWizard");
		string filename = Path.Combine(basePath, "AkWwisePopPicker.cs");
		File.Delete(filename);	
		AssetDatabase.Refresh();
	}
	
    static void InstallNewPlatforms()
    {
        try
        {
            // Get a list of installed plugins
            string pluginsPath = Path.Combine(Application.dataPath, "plugins");

            if (!Directory.Exists(pluginsPath))
            {
                Directory.CreateDirectory(pluginsPath);
            }

            DirectoryInfo pluginsDirectory = new DirectoryInfo(pluginsPath);
            FileInfo[] foundPlugins = pluginsDirectory.GetFiles("*AkSoundEngine*", SearchOption.AllDirectories);
            List<string> installedPlugins = new List<string>();
            string ToAdd = "";
            foreach (FileInfo plugin in foundPlugins)
            {
                if (plugin.DirectoryName.Contains("x86") && plugin.FullName.Contains(".dll"))
                {
                    // x86 and x86_64 plus .dll mean Windows
					ToAdd = "Windows";
                }
				else if (plugin.DirectoryName.Contains("x86") && plugin.FullName.Contains(".so"))
				{
					// x86 and x86_64 plus .so mean Windows
					ToAdd = "Linux";
				}
				else if (plugin.DirectoryName.Contains("Metro"))
				{
					ToAdd = "Metro";
				}
				else if (plugin.DirectoryName.Contains("PSVita"))
				{
					ToAdd = "Vita";
				}
				else if (plugin.FullName.Contains(".bundle"))
				{
					ToAdd = "Mac";
				}
				else if (plugin.FullName.Contains(".xex"))
				{
					ToAdd = "XBox360";
				}
				else if (plugin.FullName.Contains (".meta") || plugin.FullName.Contains (".def") || plugin.FullName.Contains (".pdb") )
				{
					continue;
				}
				else
                {
                    ToAdd = Path.GetFileName(plugin.DirectoryName);
                }
                
                if( !installedPlugins.Contains(ToAdd))
                {
                	installedPlugins.Add (ToAdd);
                }
            }

            // Get the available plugins, and mark the new ones for install
            pluginsPath = Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Wwise"), "Deployment"), "Plugins");
            pluginsDirectory = new DirectoryInfo(pluginsPath);
            DirectoryInfo[] availablePlatforms = pluginsDirectory.GetDirectories();



            foreach (DirectoryInfo platform in availablePlatforms)
            {
                if (platform.Name == "Common")
                {
                    continue;
                }

                if (!installedPlugins.Contains(platform.Name))
                {
					WwiseSetupWizard.InstallPlugin(platform);
                }
            }
        }
        catch
        {
            Debug.Log("WwiseUnity: Unable to install new platform plugins. Please copy them manually to Assets/Plugins");
        }
    }
	
	static string s_CurrentScene = null;
	static void CheckWwiseGlobalExistance()
	{
        WwiseSettings settings = WwiseSettings.LoadSettings();
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_4_6 || UNITY_4_7
        if (!settings.OldProject && (String.IsNullOrEmpty(EditorApplication.currentScene) || s_CurrentScene != EditorApplication.currentScene))
#else
        string activeSceneName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
        if (!settings.OldProject && s_CurrentScene != activeSceneName)
#endif
		{			
			// Look for a game object which has the initializer component
			AkInitializer[] AkInitializers = UnityEngine.Object.FindObjectsOfType(typeof(AkInitializer)) as AkInitializer[];
			if (AkInitializers.Length == 0)
			{
                if (settings.CreateWwiseGlobal == true)
                {
                    //No Wwise object in this scene, create one so that the sound engine is initialized and terminated properly even if the scenes are loaded
                    //in the wrong order.
                    GameObject objWwise = new GameObject("WwiseGlobal");

                    //Attach initializer and terminator components
                    AkInitializer init = objWwise.AddComponent<AkInitializer>();
                    AkWwiseProjectInfo.GetData().CopyInitSettings(init);
                }
			}
			else
			{
                if (settings.CreateWwiseGlobal == false && AkInitializers[0].gameObject.name == "WwiseGlobal")
                {
                    GameObject.DestroyImmediate(AkInitializers[0].gameObject);
                }
				//All scenes will share the same initializer.  So expose the init settings consistently across scenes.
				AkWwiseProjectInfo.GetData().CopyInitSettings(AkInitializers[0]);
			}

			AkAudioListener[] akAudioListeners = UnityEngine.Object.FindObjectsOfType(typeof(AkAudioListener)) as AkAudioListener[];
            if (akAudioListeners.Length == 0)
            {
                // Remove the audio listener script
                if (Camera.main != null && settings.CreateWwiseListener == true)
                {
                    AudioListener listener = Camera.main.gameObject.GetComponent<AudioListener>();
                    if (listener != null)
                    {
                        Component.DestroyImmediate(listener);
                    }

                    // Add the AkAudioListener script
                    if (Camera.main.gameObject.GetComponent<AkAudioListener>() == null)
                    {
                        Camera.main.gameObject.AddComponent<AkAudioListener>();
                    }
                }
            }
            else
            {
                foreach (AkAudioListener akListener in akAudioListeners)
                {
                    if (settings.CreateWwiseListener == false && akListener.gameObject == Camera.main.gameObject)
                    {
                        Component.DestroyImmediate(akListener);
                    }
                }
            }


#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_4_6 || UNITY_4_7
			s_CurrentScene = EditorApplication.currentScene;
#else
			s_CurrentScene = activeSceneName;
#endif
		}
	}
}

#endif // UNITY_EDITOR