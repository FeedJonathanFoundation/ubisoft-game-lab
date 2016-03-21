#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;

public class AkBankHandle
{
    int m_RefCount = 0;
    uint m_BankID;

    public string bankName;

    public AkCallbackManager.BankCallback bankCallback;

    public AkBankHandle(string name)
    {
        bankName = name;
        bankCallback = null;
        m_BankID = AkSoundEngine.GetIDFromString(System.IO.Path.GetFileNameWithoutExtension(name));
    }

    public int RefCount
    {
        get
        {
            return m_RefCount;
        }
    }

    /// Loads a bank.  This version blocks until the bank is loaded.  See AK::SoundEngine::LoadBank for more information
    public void LoadBank()
    {
        if (m_RefCount == 0)
        {
            // There might be a case where we were asked to unload the SoundBank, but then asked immediately after to load that bank.
            // If that happens, there will be a short amount of time where the ref count will be 0, but the bank will still be in memory.
            // In that case, we do not want to unload the bank, so we have to remove it from the list of pending bank unloads.
            if (AkBankManager.BanksToUnload.Contains(m_BankID))
            {
                AkBankManager.BanksToUnload.Remove(m_BankID);
                IncRef();
                return;
            }

            AKRESULT res = AkSoundEngine.LoadBank(bankName, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
            if (res != AKRESULT.AK_Success)
            {
                Debug.LogWarning("WwiseUnity: Bank " + bankName + " failed to load (" + res.ToString() + ")");
            }
        }

        IncRef();
    }

    /// Loads a bank.  This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information
    public void LoadBankAsync(AkCallbackManager.BankCallback callback = null)
    {
        if (m_RefCount == 0)
        {
            // There might be a case where we were asked to unload the SoundBank, but then asked immediately after to load that bank.
            // If that happens, there will be a short amount of time where the ref count will be 0, but the bank will still be in memory.
            // In that case, we do not want to unload the bank, so we have to remove it from the list of pending bank unloads.
            if (AkBankManager.BanksToUnload.Contains(m_BankID))
            {
                AkBankManager.BanksToUnload.Remove(m_BankID);
                IncRef();
                return;
            }

            bankCallback = callback;
            AkSoundEngine.LoadBank(bankName, AkBankManager.GlobalBankCallback, this, AkSoundEngine.AK_DEFAULT_POOL_ID, out m_BankID);
        }
        IncRef();
    }

    public void IncRef()
    {
        m_RefCount++;
    }

    public void DecRef()
    {
        m_RefCount--;
        if (m_RefCount == 0)
        {
            AkBankManager.BanksToUnload.Add(m_BankID);
        }
    }
}

/// @brief Maintains the list of soundbanks loaded.  This is currently used only with AkAmbient objects.
public static class AkBankManager
{
    static Dictionary<string, AkBankHandle> m_BankHandles = new Dictionary<string, AkBankHandle>();
    static public List<uint> BanksToUnload = new List<uint>();

    static public void DoUnloadBanks()
    {
		foreach(uint bankID in BanksToUnload)
        {
            Debug.Log("WwiseUnity: Unloading bank");
            AkSoundEngine.UnloadBank(bankID, IntPtr.Zero, null, null);
        }

        BanksToUnload.Clear();
    }

    static public void Reset()
    {
		m_BankHandles.Clear ();
		BanksToUnload.Clear ();
    }

    static public void GlobalBankCallback(uint in_bankID, IntPtr in_pInMemoryBankPtr, AKRESULT in_eLoadResult, uint in_memPoolId, object in_Cookie)
    {
        m_Mutex.WaitOne();
		AkBankHandle handle = (AkBankHandle)in_Cookie ;
        AkCallbackManager.BankCallback cb = handle.bankCallback;
        if (in_eLoadResult != AKRESULT.AK_Success)
        {
            Debug.LogWarning("WwiseUnity: Bank " + handle.bankName + " failed to load (" + in_eLoadResult.ToString() + ")");
            m_BankHandles.Remove(handle.bankName);
        }
        m_Mutex.ReleaseMutex();

        if (cb != null)
            cb(in_bankID, in_pInMemoryBankPtr, in_eLoadResult, in_memPoolId, null);
    }

    /// Loads a bank.  This version blocks until the bank is loaded.  See AK::SoundEngine::LoadBank for more information
    public static void LoadBank(string name)
    {
        m_Mutex.WaitOne();
        AkBankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            handle = new AkBankHandle(name);
            m_BankHandles.Add(name, handle);
            m_Mutex.ReleaseMutex();
            handle.LoadBank();
        }
        else
        {
            // Bank already loaded, increment its ref count.
            handle.IncRef();
            m_Mutex.ReleaseMutex();
        }
    }

    /// Loads a bank.  This version returns right away and loads in background. See AK::SoundEngine::LoadBank for more information
    public static void LoadBankAsync(string name, AkCallbackManager.BankCallback callback = null)
    {
        m_Mutex.WaitOne();
        AkBankHandle handle = null;
        if (!m_BankHandles.TryGetValue(name, out handle))
        {
            handle = new AkBankHandle(name);
            m_BankHandles.Add(name, handle);
            m_Mutex.ReleaseMutex();
            handle.LoadBankAsync(callback);
        }
        else
        {
            // Bank already loaded, increment its ref count.
            handle.IncRef();
            m_Mutex.ReleaseMutex();
        }
    }

    /// Unloads a bank.  See AK::SoundEngine::UnloadBank for more information
    public static void UnloadBank(string name)
    {
        m_Mutex.WaitOne();
        AkBankHandle handle = null;
        if (m_BankHandles.TryGetValue(name, out handle))
        {
            handle.DecRef();
            if (handle.RefCount == 0)
                m_BankHandles.Remove(name);
        }
        m_Mutex.ReleaseMutex();
    }

    static System.Threading.Mutex m_Mutex = new System.Threading.Mutex();
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.