#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.Collections.Generic;
using System.IO;

public class WwiseUpdateSettings : WwiseSetupWindow 
{

    static bool m_oldCreateWwiseGlobal = true;
    static bool m_oldCreateWwiseListener = true;
    static string m_WwiseVersionString;

    [MenuItem("Edit/Wwise Settings...", false, (int)AkWwiseWindowOrder.WwiseSettings)]
	public static void Init()
	{
		// Get existing open window or if none, make a new one:
		EditorWindow window = EditorWindow.GetWindow(typeof (WwiseUpdateSettings));
		window.position = new Rect(100, 100, 850, 200);
#if !UNITY_5 || UNITY_5_0
		window.title ="Wwise Settings";
#else
		window.titleContent = new GUIContent("Wwise Settings");
#endif
        m_oldCreateWwiseGlobal = WwiseSetupWizard.Settings.CreateWwiseGlobal;
        m_oldCreateWwiseListener = WwiseSetupWizard.Settings.CreateWwiseListener;

        uint temp = AkSoundEngine.GetMajorMinorVersion();
        uint temp2 = AkSoundEngine.GetSubminorBuildVersion();
        m_WwiseVersionString = "Wwise v" + (temp >> 16) + "." + (temp & 0xFFFF);
        if ((temp2 >> 16) != 0)
        {
            m_WwiseVersionString += "." + (temp2 >> 16);
        }

        m_WwiseVersionString += " Build " + (temp2 & 0xFFFF) + " Settings.";
    }

    bool ApplyNewProject = false;
	void OnGUI() 
	{
        // Make sure everything is initialized
		// Use soundbank path, because Wwise project path can be empty.
        if (String.IsNullOrEmpty(WwiseSetupWizard.Settings.SoundbankPath) && WwiseSetupWizard.Settings.WwiseProjectPath == null)
        {
            WwiseSetupWizard.Settings = WwiseSettings.LoadSettings();
        }

        string initialProject = WwiseSetupWizard.Settings.WwiseProjectPath;

		if( VersionStyle == null )
		{
			InitGuiStyles();
		}
        GUILayout.Label(m_WwiseVersionString, VersionStyle);

		DrawSettingsPart();

        string newProject = WwiseSetupWizard.Settings.WwiseProjectPath; // DrawSettingsPart modifies WwiseSetupWizard.Settings.WwiseProjectPath directly.
        if (initialProject != newProject)
        {
            ApplyNewProject = true;
        }
		
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

		GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
		if( GUILayout.Button("OK", GUILayout.Width(60)) )
		{
			if( string.IsNullOrEmpty(WwiseSetupWizard.Settings.SoundbankPath) )
			{
				EditorUtility.DisplayDialog("Error", "Please fill in the required settings", "Ok");
			}

            if (WwiseUpdateSettings.m_oldCreateWwiseGlobal != WwiseSetupWizard.Settings.CreateWwiseGlobal)
            {
                AkInitializer[] AkInitializers = UnityEngine.Object.FindObjectsOfType(typeof(AkInitializer)) as AkInitializer[];
                if (WwiseSetupWizard.Settings.CreateWwiseGlobal == true)
                {
                    if (AkInitializers.Length == 0)
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
                    if (AkInitializers.Length != 0 && AkInitializers[0].gameObject.name == "WwiseGlobal")
                    {
                        GameObject.DestroyImmediate(AkInitializers[0].gameObject);
                    }
                }
            }

            if (WwiseUpdateSettings.m_oldCreateWwiseListener != WwiseSetupWizard.Settings.CreateWwiseListener)
            {
                if (Camera.main != null)
                {
                    AkAudioListener akListener = Camera.main.GetComponentInChildren<AkAudioListener>();

                    if (WwiseSetupWizard.Settings.CreateWwiseListener)
                    {
                        if (akListener == null)
                        {
                            Camera.main.gameObject.AddComponent<AkAudioListener>();
                        }

                        // If Unity had already an audio listener, we want to remove it when adding our own.
                        AudioListener unityListener = Camera.main.GetComponentInChildren<AudioListener>();

                        if (unityListener != null)
                        {
                            Component.DestroyImmediate(unityListener);
                        }
                    }
                    else
                    {
                        if (akListener != null)
                        {
                            Component.DestroyImmediate(akListener);
                        }
                    }
                }
            }

            WwiseSettings.SaveSettings(WwiseSetupWizard.Settings);
			
			CloseWindow();

            // Pop the Picker window so the user can start working right away
            AkWwiseProjectInfo.GetData(); // Load data
            if (ApplyNewProject)
            {
                //Clear the data, the project path changed.
                AkWwiseProjectInfo.GetData().Reset();
                ApplyNewProject = false;
                AkWwisePicker.WwiseProjectFound = true;
            }
            AkWwiseProjectInfo.Populate();
            AkWwisePicker.PopulateTreeview();
            AkWwisePicker.init();
		}
		
		if( GUILayout.Button("Cancel", GUILayout.Width(60)) )
		{
            WwiseSetupWizard.Settings = WwiseSettings.LoadSettings(true); 
            CloseWindow();
		}
        GUILayout.Space(5);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);
        GUILayout.EndVertical();

		
	}
	
	void CloseWindow()
	{
		EditorWindow SetupWindow = EditorWindow.GetWindow(typeof(WwiseUpdateSettings));
		SetupWindow.Close();
	}
}

#endif // UNITY_EDITOR