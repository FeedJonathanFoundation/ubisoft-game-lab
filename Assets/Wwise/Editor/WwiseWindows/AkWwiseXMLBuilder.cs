#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Collections.Generic;

public class AkWwiseXMLBuilder
{
    public static bool Populate()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
        {
            return false;
        }

        // Try getting the SoundbanksInfo.xml file for Windows or Mac first, then try to find any other available platform.
        string FullSoundbankPath = AkBasePathGetter.GetPlatformBasePath();
        string filename = Path.Combine(FullSoundbankPath, "SoundbanksInfo.xml");

        if (!File.Exists(filename))
        {
            FullSoundbankPath = Path.Combine(Application.streamingAssetsPath, WwiseSetupWizard.Settings.SoundbankPath);
#if UNITY_EDITOR_OSX
			FullSoundbankPath = FullSoundbankPath.Replace('\\', '/');
#endif
            string[] foundFiles = Directory.GetFiles(FullSoundbankPath, "SoundbanksInfo.xml", SearchOption.AllDirectories);
            if (foundFiles.Length > 0)
            {
                // We just want any file, doesn't matter which one.
                filename = foundFiles[0];
            }
        }

        bool bChanged = false;
        if (File.Exists(filename))
        {
            DateTime time = File.GetLastWriteTime(filename);
            if (time <= s_LastParsed)
                return false;
            
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            XmlNodeList soundBanks = doc.GetElementsByTagName("SoundBanks");
            for (int i = 0; i < soundBanks.Count; i++)
            {
                XmlNodeList soundBank = soundBanks[i].SelectNodes("SoundBank");
                for (int j = 0; j < soundBank.Count; j++)
                {
                    bChanged = bChanged || SerialiseSoundBank(soundBank[j]);
                }
            }
        }
        return bChanged;
    }

    static bool SerialiseSoundBank(XmlNode node)
    {
        bool bChanged = false;
        XmlNodeList includedEvents = node.SelectNodes("IncludedEvents");
        for (int i = 0; i < includedEvents.Count; i++)
        {
            XmlNodeList events = includedEvents[i].SelectNodes("Event");
            for (int j = 0; j < events.Count; j++)
            {
                bChanged = bChanged || SerialiseMaxAttenuation(events[j]);
            }
        }
        return bChanged;
    }

    static bool SerialiseMaxAttenuation(XmlNode node)
    {
        bool bChanged = false;
        for (int i = 0; i < AkWwiseProjectInfo.GetData().EventWwu.Count; i++)
        {
			for(int j = 0; j < AkWwiseProjectInfo.GetData().EventWwu[i].List.Count; j++)
			{
				if (node.Attributes["MaxAttenuation"] != null && node.Attributes["Name"].InnerText == AkWwiseProjectInfo.GetData().EventWwu[i].List[j].Name)
            	{
                    float radius = float.Parse(node.Attributes["MaxAttenuation"].InnerText);
                    if (AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxAttenuation != radius)
                    {
                        AkWwiseProjectInfo.GetData().EventWwu[i].List[j].maxAttenuation = radius;
                        bChanged = true;
                    }
            	    break;
            	}
			}
        }
        return bChanged;
    }

    static DateTime s_LastParsed = DateTime.MinValue;
}
#endif