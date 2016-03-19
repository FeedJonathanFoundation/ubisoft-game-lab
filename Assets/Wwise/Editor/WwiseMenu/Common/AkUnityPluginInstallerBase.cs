#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;
using System.Net;


// This sets the order in which the menus appear.
public enum AkWwiseMenuOrder : int
{
    AndroidDebug = 100,
    AndroidProfile,
    AndroidRelease,
    IosDebug,
    IosProfile,
    IosRelease,
    Linux32Debug,
    Linux32Profile,
    Linux32Release,
    Linux64Debug,
    Linux64Profile,
    Linux64Release,
    MacDebug,
    MacProfile,
    MacRelease,
    WSAWin32Debug,
    WSAWin32Profile,
    WSAWin32Release,
    WSAArmDebug,
    WSAArmProfile,
    WSAArmRelease,
    PS3Debug,
    PS3Profile,
    PS3Release,
    PS4Debug,
    PS4Profile,
    PS4Release,
    VitaDebug,
    VitaProfile,
    VitaRelease,
    VitaHWDebug,
    VitaHWProfile,
    VitaHWRelease,
    WiiUDebug,
    WiiUProfile,
    WiiURelease,
    Win32Debug,
    Win32Profile,
    Win32Release,
    Win64Debug,
    Win64Profile,
    Win64Release,
    Xbox360Debug,
    Xbox360Profile,
    Xbox360Release,
    XboxOneDebug,
    XboxOneProfile,
    XboxOneRelease,

    ConvertIDs = 200,
    Reinstall,
    Uninstall
}

public enum AkWwiseWindowOrder : int
{
    WwiseSettings = 305,
    WwisePicker = 2300
}

public enum AkWwiseHelpOrder : int
{
    WwiseHelpOrder = 200
}

public class AkUnityAssetsInstaller
{
    protected string m_platform = "Undefined";
    public string[] m_arches = new string[] { };
    protected string m_assetsDir = Application.dataPath;
    protected string m_pluginDir = Path.Combine(Application.dataPath, "Plugins");
	protected List<string> m_excludes = new List<string>() {".meta"};

    // Copy file to destination directory and create the directory when none exists.
    public static bool CopyFileToDirectory(string srcFilePath, string destDir)
    {
        FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to copy. Source is missing: {0}.", srcFilePath));
            return false;
        }

        DirectoryInfo di = new DirectoryInfo(destDir);

		if ( ! di.Exists )
        {
            di.Create();
        }

        const bool IsToOverwrite = true;
        try
        {
            fi.CopyTo(Path.Combine(di.FullName, fi.Name), IsToOverwrite);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
            return false;
        }

        return true;
    }

    // Copy or overwrite destination file with source file.
    public static bool OverwriteFile(string srcFilePath, string destFilePath)
    {
        FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to overwrite. Source is missing: {0}.", srcFilePath));
            return false;
        }

        DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(destFilePath));

		if ( ! di.Exists )
        {
            di.Create();
        }

        const bool IsToOverwrite = true;
        try
        {
            fi.CopyTo(destFilePath, IsToOverwrite);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
            return false;
        }

        return true;
    }

    // Move file to destination directory and create the directory when none exists.
    public static void MoveFileToDirectory(string srcFilePath, string destDir)
    {
        FileInfo fi = new FileInfo(srcFilePath);
		if ( ! fi.Exists )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to move. Source is missing: {0}.", srcFilePath));
            return;
        }

        DirectoryInfo di = new DirectoryInfo(destDir);

		if ( ! di.Exists )
        {
            di.Create();
        }

        string destFilePath = Path.Combine(di.FullName, fi.Name);
        try
        {
            fi.MoveTo(destFilePath);
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
            return;
        }

        return;
    }

    // Recursively copy a directory to its destination.
    public static bool RecursiveCopyDirectory(DirectoryInfo srcDir, DirectoryInfo destDir, List<string> excludeExtensions = null)
    {
    	if ( ! srcDir.Exists )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to copy. Source is missing: {0}.", srcDir));
            return false;
        }

        if ( ! destDir.Exists )
        {
            destDir.Create();
        }

        // Copy all files.
        FileInfo[] files = srcDir.GetFiles();
        foreach (FileInfo file in files)
        {
            if (excludeExtensions != null)
            {
                string fileExt = Path.GetExtension(file.Name);
                bool isFileExcluded = false;
                foreach (string ext in excludeExtensions)
                {
                    if (fileExt.ToLower() == ext)
                    {
                        isFileExcluded = true;
                        break;
                    }
                }

                if (isFileExcluded)
                {
                    continue;
                }
            }

            const bool IsToOverwrite = true;
            try
            {
                file.CopyTo(Path.Combine(destDir.FullName, file.Name), IsToOverwrite);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("WwiseUnity: Error during installation: {0}.", ex.Message));
                return false;
            }
        }

        // Process subdirectories.
        DirectoryInfo[] dirs = srcDir.GetDirectories();
        foreach (DirectoryInfo dir in dirs)
        {
            // Get destination directory.
            string destFullPath = Path.Combine(destDir.FullName, dir.Name);

            // Recurse
            bool isSuccess = RecursiveCopyDirectory(dir, new DirectoryInfo(destFullPath), excludeExtensions);
            if ( ! isSuccess )
                return false;
        }

        return true;
    }

}

public class AkUnityPluginInstallerBase : AkUnityAssetsInstaller
{
    private string m_progTitle = "WwiseUnity: Plugin Installation Progress";

    public bool InstallPluginByConfig(string config)
    {
        string pluginSrc = GetPluginSrcPathByConfig(config);
        string pluginDest = GetPluginDestPath("");

        string progMsg = string.Format("Installing plugin for {0} ({1}) from {2} to {3}.", m_platform, config, pluginSrc, pluginDest);
        EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

        bool isSuccess = RecursiveCopyDirectory(new DirectoryInfo(pluginSrc), new DirectoryInfo(pluginDest), m_excludes);
		if ( ! isSuccess )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to install plugin for {0} ({1}) from {2} to {3}.", m_platform, config, pluginSrc, pluginDest));
            EditorUtility.ClearProgressBar();
            return false;
        }

        EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log(string.Format("WwiseUnity: Plugin for {0} {1} installed from {2} to {3}.", m_platform, config, pluginSrc, pluginDest));

        return true;
    }

    public virtual bool InstallPluginByArchConfig(string arch, string config)
    {
        string pluginSrc = GetPluginSrcPathByArchConfig(arch, config);
        string pluginDest = GetPluginDestPath(arch);

        string progMsg = string.Format("Installing plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest);
        EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 0.5f);

        bool isSuccess = RecursiveCopyDirectory(new DirectoryInfo(pluginSrc), new DirectoryInfo(pluginDest), m_excludes);
		if ( ! isSuccess )
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to install plugin for {0} ({1}, {2}) from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));
            EditorUtility.ClearProgressBar();
            return false;
        }

        EditorUtility.DisplayProgressBar(m_progTitle, progMsg, 1.0f);
        AssetDatabase.Refresh();

        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log(string.Format("WwiseUnity: Plugin for {0} {1} {2} installed from {3} to {4}.", m_platform, arch, config, pluginSrc, pluginDest));

        return true;
    }

    protected string GetPluginSrcPathByConfig(string config)
    {
        return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(m_assetsDir, "Wwise"), "Deployment"), "Plugins"), m_platform), config);
    }

    protected string GetPluginSrcPathByArchConfig(string arch, string config)
    {
        return Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(m_assetsDir, "Wwise"), "Deployment"), "Plugins"), m_platform), arch), config);
    }

    protected virtual string GetPluginDestPath(string arch)
    {
        return m_pluginDir;
    }
}

public class AkUnityPluginInstallerMultiArchBase : AkUnityPluginInstallerBase
{
    protected override string GetPluginDestPath(string arch)
    {
        return Path.Combine(Path.Combine(m_pluginDir, m_platform), arch);
    }
}

public class AkDocHelper
{
    static string m_WwiseVersionString = string.Empty;
    public static void OpenDoc(string platform)
    {
        if (m_WwiseVersionString == string.Empty)
        {
            uint temp = AkSoundEngine.GetMajorMinorVersion();
            uint temp2 = AkSoundEngine.GetSubminorBuildVersion();
            m_WwiseVersionString = (temp >> 16) + "." + (temp & 0xFFFF);
            if ((temp2 >> 16) != 0)
            {
                m_WwiseVersionString += "." + (temp2 >> 16);
            }

            m_WwiseVersionString += "_" + (temp2 & 0xFFFF);
        }

        string docUrl = "http://www.audiokinetic.com/library/" + m_WwiseVersionString + "/?source=Unity&id=main.html";
        bool isConnected = false;
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.audiokinetic.com/robots.txt");
            request.Timeout = 1000;
            request.Credentials = CredentialCache.DefaultNetworkCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                isConnected = true;
            }
            else
            {
                isConnected = false;
            }
        }
        catch (Exception)
        {
            isConnected = false;
        }

        if (!isConnected)
        {
            // Can't access audiokinetic.com, open local doc.
            docUrl = GetLocalDocUrl(platform);
            if (string.IsNullOrEmpty(docUrl))
            {
                return;
            }
        }

        Application.OpenURL(docUrl);
    }

    static string GetLocalDocUrl(string platform)
    {
        string docUrl = string.Empty;
        string docPath = string.Empty;

#if UNITY_EDITOR_WIN
        if (platform == "Windows")
        {
            docPath = string.Format("{0}/Wwise/Documentation/{1}/en/WwiseUnityIntegrationHelp_en.chm", Application.dataPath, platform);
        }
        else
        {
            docPath = string.Format("{0}/Wwise/Documentation/{1}/en/WwiseUnityIntegrationHelp_{1}_en.chm", Application.dataPath, platform);
        }
#else
		string DestPath = AkUtilities.GetFullPath(Application.dataPath, "../WwiseUnityIntegrationHelp_en");
		docPath = string.Format ("{0}/html/index.html", DestPath);
		if (!File.Exists (docPath))
		{
			UnzipHelp(DestPath);
		}
		
		if( !File.Exists(docPath))
        {
        	UnityEngine.Debug.Log("WwiseUnity: Unable to show documentation. Please unzip WwiseUnityIntegrationHelp_AppleCommon_en.zip manually.");
			return string.Empty;
        }
#endif

        FileInfo fi = new FileInfo(docPath);
        if (!fi.Exists)
        {
            UnityEngine.Debug.LogError(string.Format("WwiseUnity: Failed to find documentation: {0}. Aborted.", docPath));
            return string.Empty;
        }
        docUrl = string.Format("file:///{0}", docPath.Replace(" ", "%20"));

        return docUrl;
    }

    public static void UnzipHelp(string DestPath)
    {
        // Start by extracting the zip, if it exists
        string ZipPath = Path.Combine(Path.Combine(Path.Combine(Path.Combine(Path.Combine(Application.dataPath, "Wwise"), "Documentation"), "AppleCommon"), "en"), "WwiseUnityIntegrationHelp_en.zip");

        if (File.Exists(ZipPath))
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            start.FileName = "unzip";

            start.Arguments = "\"" + ZipPath + "\" -d \"" + DestPath + "\"";

            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;

            string progMsg = "WwiseUnity: Unzipping documentation...";
            string progTitle = "Unzipping Wwise documentation";
            EditorUtility.DisplayProgressBar(progTitle, progMsg, 0.5f);

            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                while (!process.WaitForExit(1000))
                {
                    System.Threading.Thread.Sleep(100);
                }
                try
                {
                    //ExitCode throws InvalidOperationException if the process is hanging
                    int returnCode = process.ExitCode;

                    bool isBuildSucceeded = (returnCode == 0);
                    if (isBuildSucceeded)
                    {
                        EditorUtility.DisplayProgressBar(progTitle, progMsg, 1.0f);
                        UnityEngine.Debug.Log("WwiseUnity: Documentation extraction succeeded. ");
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("WwiseUnity: Extraction failed.");
                    }

                    EditorUtility.ClearProgressBar();
                }
                catch (Exception ex)
                {
                    EditorUtility.ClearProgressBar();
                    UnityEngine.Debug.LogError(ex.ToString());
                    EditorUtility.ClearProgressBar();
                }
            }
        }
    }

}

#endif // #if UNITY_EDITOR