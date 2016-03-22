#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;


public class AkWwiseMenu_Mac : MonoBehaviour {
#if !UNITY_5
	private static AkUnityPluginInstaller_Mac m_installer = new AkUnityPluginInstaller_Mac();

	// private static AkUnityIntegrationBuilder_Mac m_builder = new AkUnityIntegrationBuilder_Mac();

	[MenuItem("Assets/Wwise/Install Plugins/Mac/Debug", false, (int)AkWwiseMenuOrder.MacDebug)]
	public static void InstallPlugin_Debug () {
		m_installer.InstallPluginByConfig("Debug");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Mac/Profile", false, (int)AkWwiseMenuOrder.MacProfile)]
	public static void InstallPlugin_Profile () {
		m_installer.InstallPluginByConfig("Profile");
	}

	[MenuItem("Assets/Wwise/Install Plugins/Mac/Release", false, (int)AkWwiseMenuOrder.MacRelease)]
	public static void InstallPlugin_Release () {
		m_installer.InstallPluginByConfig("Release");
	}
#endif

    [MenuItem("Help/Wwise Help/Mac Common", false, (int)AkWwiseHelpOrder.WwiseHelpOrder)]
    public static void OpenDocMac () 
    {
       	AkDocHelper.OpenDoc("Mac");
    }
    
//	[MenuItem("Assets/Wwise/Rebuild Integration/Mac/Debug")]
//	public static void RebuildIntegration_Debug () {
//		m_builder.BuildByConfig("Debug", null);
//	}
//
//	[MenuItem("Assets/Wwise/Rebuild Integration/Mac/Profile")]
//	public static void RebuildIntegration_Profile () {
//		m_builder.BuildByConfig("Profile", null);
//	}
//
//	[MenuItem("Assets/Wwise/Rebuild Integration/Mac/Release")]
//	public static void RebuildIntegration_Release () {
//		m_builder.BuildByConfig("Release", null);
//	}
}


public class AkUnityPluginInstaller_Mac : AkUnityPluginInstallerBase
{
	public AkUnityPluginInstaller_Mac()
	{
		m_platform = "Mac";
	}
}


public class AkUnityIntegrationBuilder_Mac : AkUnityIntegrationBuilderBase
{
	public AkUnityIntegrationBuilder_Mac()
	{
		m_platform = "Mac";
	}
}

#endif // #if UNITY_EDITOR