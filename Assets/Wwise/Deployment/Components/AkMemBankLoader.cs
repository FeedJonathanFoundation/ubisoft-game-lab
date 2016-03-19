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
using System.Collections;
using System.Runtime.InteropServices;

/// This class is an example of how to load banks in Wwise, if the bank data was preloaded in memory.  
/// This would be useful for situations where you use the WWW class
public class AkMemBankLoader : MonoBehaviour
{
	/// Name of the bank to load
	public string bankName = "";
	
	/// Is the bank localized (situated in the language specific folders)
	public bool isLocalizedBank = false;
	
	private WWW ms_www;
	private GCHandle ms_pinnedArray;
	private IntPtr ms_pInMemoryBankPtr = IntPtr.Zero;
	[HideInInspector]
	public uint ms_bankID = AkSoundEngine.AK_INVALID_BANK_ID;
	
	private const int WaitMs = 50;
	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT = (long)AkSoundEngine.AK_BANK_PLATFORM_DATA_ALIGNMENT;
	private const long AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK = AK_BANK_PLATFORM_DATA_ALIGNMENT - 1;
	
	void Start()
	{
		if (isLocalizedBank)
		{
			LoadLocalizedBank(bankName);
		}
		else
		{
			LoadNonLocalizedBank(bankName);
		}
	}
	
	/// Load a sound bank from WWW object 
	public void LoadNonLocalizedBank(string in_bankFilename)
	{
        string bankPath = "file://" + Path.Combine(AkBasePathGetter.GetPlatformBasePath(), in_bankFilename);
		DoLoadBank(bankPath);
	}
	
	/// Load a language-specific bank from WWW object
	public void LoadLocalizedBank(string in_bankFilename)
	{
        string bankPath = "file://" + Path.Combine(Path.Combine(AkBasePathGetter.GetPlatformBasePath(), AkInitializer.GetCurrentLanguage()), in_bankFilename);
		DoLoadBank(bankPath);
	}

	IEnumerator LoadFile()
	{
		ms_www = new WWW(m_bankPath);

		yield return ms_www;

		uint in_uInMemoryBankSize = 0;

		// Allocate an aligned buffer
		try
		{
			ms_pinnedArray = GCHandle.Alloc(ms_www.bytes, GCHandleType.Pinned);
			ms_pInMemoryBankPtr = ms_pinnedArray.AddrOfPinnedObject();
			in_uInMemoryBankSize = (uint)ms_www.bytes.Length;	
			
			// Array inside the WWW object is not aligned. Allocate a new array for which we can guarantee the alignment.
			if( (ms_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0 )
			{
				byte[] alignedBytes = new byte[ms_www.bytes.Length + AK_BANK_PLATFORM_DATA_ALIGNMENT];
				GCHandle new_pinnedArray = GCHandle.Alloc(alignedBytes, GCHandleType.Pinned);
				IntPtr new_pInMemoryBankPtr = new_pinnedArray.AddrOfPinnedObject();
				int alignedOffset = 0;
				
				// New array is not aligned, so we will need to use an offset inside it to align our data.
				if( (new_pInMemoryBankPtr.ToInt64() & AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) != 0 )
				{
					Int64 alignedPtr = (new_pInMemoryBankPtr.ToInt64() + AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK) & ~AK_BANK_PLATFORM_DATA_ALIGNMENT_MASK;
					alignedOffset = (int)(alignedPtr - new_pInMemoryBankPtr.ToInt64());
					new_pInMemoryBankPtr = new IntPtr(alignedPtr);
				}
				
				// Copy the bank's bytes in our new array, at the correct aligned offset.
				Array.Copy (ms_www.bytes, 0, alignedBytes, alignedOffset, ms_www.bytes.Length);
				
				ms_pInMemoryBankPtr = new_pInMemoryBankPtr;
				ms_pinnedArray.Free();
				ms_pinnedArray = new_pinnedArray;
			}
		}
		catch
		{
			yield break;
		}
		
		AKRESULT result = AkSoundEngine.LoadBank(ms_pInMemoryBankPtr, in_uInMemoryBankSize, out ms_bankID);
		if( result != AKRESULT.AK_Success )
		{
			Debug.LogError("WwiseUnity: AkMemBankLoader: bank loading failed with result " + result.ToString ());
		}
	}
	
	private string m_bankPath;

	private void DoLoadBank(string in_bankPath)
	{
		m_bankPath = in_bankPath;
		StartCoroutine (LoadFile ());
	}
	
	void OnDestroy()
	{
		if (ms_pInMemoryBankPtr != IntPtr.Zero)
		{
			AKRESULT result = AkSoundEngine.UnloadBank(ms_bankID, ms_pInMemoryBankPtr);
			if (result == AKRESULT.AK_Success)
			{
				ms_pinnedArray.Free();	
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.