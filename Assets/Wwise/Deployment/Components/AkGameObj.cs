#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;

[AddComponentMenu("Wwise/AkGameObj")]
///@brief This component represents a sound emitter in your scene.  It will track its position and other game syncs such as Switches, RTPC and environment values.  You can add this to any object that will emit sound.  Note that if it is not present, Wwise will add it automatically, with the default values, to any Unity Game Object that is passed to Wwise API (see AkSoundEngine.cs).  
/// \sa
/// - \ref soundengine_gameobj
/// - \ref soundengine_events
/// - \ref soundengine_switch
/// - \ref soundengine_states
/// - \ref soundengine_environments
[ExecuteInEditMode] //ExecuteInEditMode necessary to maintain proper state of isStaticObject.
public class AkGameObj : MonoBehaviour 
{
	const int ALL_LISTENER_MASK = (1<<AkSoundEngine.AK_NUM_LISTENERS)-1;

	/// When not set to null, the emitter position will be offset relative to the Game Object position by the Position Offset
	public AkGameObjPositionOffsetData m_positionOffsetData = null;
	
	/// Is this object affected by Environment changes?  Set to false if not affected in order to save some useless calls.  Default is true.
    public bool isEnvironmentAware = true;
	public AkGameObjEnvironmentData m_envData = null;

	/// Listener 0 by default.
	public int listenerMask = 1; 

	/// Maintains and persists the Static setting of the gameobject, which is available only in the editor.
	[SerializeField]
	private bool isStaticObject = false;
	private AkGameObjPositionData m_posData = null;

    /// Cache the bounds to avoid calls to GetComponent()
    private Collider m_Collider;

    void Awake()
    {			
		// If the object was marked as static, don't update its position to save cycles.
#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)	
		{
			UnityEditor.EditorApplication.update += this.CheckStaticStatus;
		}
#endif 
		if(!isStaticObject)
		{
			m_posData = new AkGameObjPositionData();
		}		
		
		// Cache the bounds to avoid calls to GetComponent()
		m_Collider = GetComponent<Collider>();
	
        //Register a Game Object in the sound engine, with its name.		
        AKRESULT res = AkSoundEngine.RegisterGameObj(gameObject, gameObject.name, (uint)(listenerMask & ALL_LISTENER_MASK));
        if (res == AKRESULT.AK_Success)
        {
            // Get position with offset
            Vector3 position = GetPosition();

            //Set the original position
            AkSoundEngine.SetObjectPosition(
                gameObject,
                position.x,
                position.y,
                position.z,
                transform.forward.x,
                transform.forward.y,
                transform.forward.z);

            if (isEnvironmentAware)
            {
                m_envData = new AkGameObjEnvironmentData();
                //Check if this object is also an environment.
                AddAuxSend(gameObject);
            }
        }
    }
	
	private void CheckStaticStatus()
	{
#if UNITY_EDITOR
		if (gameObject != null && isStaticObject != gameObject.isStatic)
		{
			isStaticObject = gameObject.isStatic;
            UnityEditor.EditorUtility.SetDirty(this);
        }	
#endif
	}
	
	void OnEnable()
	{ 
		//if enabled is set to false, then the update function wont be called
		enabled = !isStaticObject;
	}
	
    void OnDestroy()
    {
#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)	
		{
			UnityEditor.EditorApplication.update -= this.CheckStaticStatus;
		}
#endif
		// We can't do the code in OnDestroy if the gameObj is unregistered, so do it now.		
		AkUnityEventHandler[] eventHandlers = gameObject.GetComponents<AkUnityEventHandler>();
		foreach( AkUnityEventHandler handler in eventHandlers )
		{
			if( handler.triggerList.Contains(AkUnityEventHandler.DESTROY_TRIGGER_ID) )
			{
				handler.DoDestroy();
			}
		}

#if UNITY_EDITOR	
		if (UnityEditor.EditorApplication.isPlaying)
#endif
        {

            if (AkSoundEngine.IsInitialized())
            {
                AkSoundEngine.UnregisterGameObj(gameObject);
            }
        }
    }

    void Update()
    {
#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)
		{
			return;
		}
#endif
        if (isStaticObject)
		{
			return;
		}

	    // Get position with offset
	    Vector3 position = GetPosition();

		//Didn't move.  Do nothing.
		if (m_posData.position == position && m_posData.forward == transform.forward)
	        return;

		m_posData.position = position;
		m_posData.forward = transform.forward;            

	    //Update position
	    AkSoundEngine.SetObjectPosition(
	        gameObject,
	        position.x,
	        position.y,
	        position.z,
	        transform.forward.x,
	        transform.forward.y,
	        transform.forward.z);

		if (isEnvironmentAware)
		{
			UpdateAuxSend();
		}        
	}
	/// Gets the position including the position offset, if applyPositionOffset is enabled.
	/// \return  The position.
	public Vector3 GetPosition()
	{
		if (m_positionOffsetData != null)
		{
			// Get offset in world space
			Vector3 worldOffset = transform.rotation * m_positionOffsetData.positionOffset;
			
			// Add offset to gameobject position
			return transform.position + worldOffset;
		}		
		return transform.position;
	}


    void OnTriggerEnter(Collider other)
    {
#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)
		{
			return;
		}
#endif

        if (isEnvironmentAware)
        {
            AddAuxSend(other.gameObject);
        }
    }

    void AddAuxSend(GameObject in_AuxSendObject)
    {
		AkEnvironmentPortal akPortal = in_AuxSendObject.GetComponent<AkEnvironmentPortal>();
		if(akPortal != null)
		{
			m_envData.activePortals.Add(akPortal);
			
			for(int i = 0; i < akPortal.environments.Length; i++) 
			{
				if(akPortal.environments[i] != null)
				{
					//Add environment only if its not already there 
					int index = m_envData.activeEnvironments.BinarySearch(akPortal.environments[i], AkEnvironment.s_compareByPriority);
					if(index < 0)
						m_envData.activeEnvironments.Insert(~index, akPortal.environments[i]);//List will still be sorted after insertion
				}
			}

			//Update and send the auxSendArray
			m_envData.auxSendValues = null;
			UpdateAuxSend();
			return;
		}
        
		AkEnvironment akEnvironment = in_AuxSendObject.GetComponent<AkEnvironment>();
		if (akEnvironment != null)
        {
			//Add environment only if its not already there 
			int index = m_envData.activeEnvironments.BinarySearch(akEnvironment, AkEnvironment.s_compareByPriority);
			if(index < 0)
			{
				m_envData.activeEnvironments.Insert(~index, akEnvironment);//List will still be sorted after insertion

				//Update only if the environment was inserted.
				//If it wasn't inserted, it means we're inside a portal so we dont update because portals have a highter priority than environments
				m_envData.auxSendValues = null;
				UpdateAuxSend();
			}
        }
    }

    void OnTriggerExit(Collider other)
    {
#if UNITY_EDITOR
		if (!UnityEditor.EditorApplication.isPlaying)
		{
			return;
		}
#endif

        if (isEnvironmentAware)
        {
			AkEnvironmentPortal akPortal = other.gameObject.GetComponent<AkEnvironmentPortal>();
			if(akPortal != null)
			{
				for(int i = 0; i < akPortal.environments.Length; i++)
				{
					if(akPortal.environments[i] != null)
					{
						//We just exited a portal so we remove its environments only if we're not inside of them
						if(!m_Collider.bounds.Intersects(akPortal.environments[i].GetCollider().bounds))
						{
							m_envData.activeEnvironments.Remove(akPortal.environments[i]);
						}
					}
				}
				//remove the portal
				m_envData.activePortals.Remove(akPortal);

				//Update and send the auxSendArray
				m_envData.auxSendValues = null;
				UpdateAuxSend();
				return;
			}

			AkEnvironment akEnvironment = other.gameObject.GetComponent<AkEnvironment>();
			if (akEnvironment != null)
			{
				//we check if the environment belongs to a portal
				for(int i = 0; i < m_envData.activePortals.Count; i++)
				{
					for(int j = 0; j < m_envData.activePortals[i].environments.Length; j++)
					{
						if(akEnvironment == m_envData.activePortals[i].environments[j])
						{
							//if it belongs to a portal, then we're inside that portal and we don't remove the environment
							m_envData.auxSendValues = null;
							UpdateAuxSend();
							return;
						}
					}
				}
				//if it doesn't belong to a portal, we remove it
				m_envData.activeEnvironments.Remove(akEnvironment);
				m_envData.auxSendValues = null;
				UpdateAuxSend();
				return;
			}
        }
    }

    void UpdateAuxSend()
    {
		if (m_envData.auxSendValues == null)
        {
#if UNITY_PS4
			// Workaround for PS4. Marshall.FreeHGlobal crashes the game, so we need to avoid resizing the array.
			// Allocate 4 entries right away to avoid the resize.
            m_envData.auxSendValues = new AkAuxSendArray((uint)AkEnvironment.MAX_NB_ENVIRONMENTS);
#else
            m_envData.auxSendValues = new AkAuxSendArray	(	m_envData.activeEnvironments.Count < AkEnvironment.MAX_NB_ENVIRONMENTS 
			                                      				? 
			                                              		(uint)m_envData.activeEnvironments.Count : (uint)AkEnvironment.MAX_NB_ENVIRONMENTS
			                                      			);
#endif
        }
        else
        {
			m_envData.auxSendValues.Reset();
        }
	

		//we search for MAX_NB_ENVIRONMENTS(4 at this time) environments with the hightest priority that belong to a portal and add them to the auxSendArray
		for(int i = 0; i < m_envData.activePortals.Count; i++)
		{
			for(int j = 0; j < m_envData.activePortals[i].environments.Length; j++)
			{
				AkEnvironment env = m_envData.activePortals[i].environments[j];

				if(env != null)
				{
					if(m_envData.activeEnvironments.BinarySearch(env, AkEnvironment.s_compareByPriority) < AkEnvironment.MAX_NB_ENVIRONMENTS)
					{
						m_envData.auxSendValues.Add(env.GetAuxBusID(), m_envData.activePortals[i].GetAuxSendValueForPosition(transform.position, j));
					}
				}
			}
		}

		//if we still dont have MAX_NB_ENVIRONMENTS in the auxSendArray, we add the next environments with the hightest priority until we reach MAX_NB_ENVIRONMENTS
		//or run out of environments
		if(m_envData.auxSendValues.m_Count < AkEnvironment.MAX_NB_ENVIRONMENTS && m_envData.auxSendValues.m_Count < m_envData.activeEnvironments.Count)
		{
			//Make a copy of all environments
			List<AkEnvironment> sortedEnvList = new List<AkEnvironment>(m_envData.activeEnvironments);

			//sort the list with the selection algorithm 
			sortedEnvList.Sort(AkEnvironment.s_compareBySelectionAlgorithm);

			int environmentsLeft = Math.Min(AkEnvironment.MAX_NB_ENVIRONMENTS - (int)m_envData.auxSendValues.m_Count, m_envData.activeEnvironments.Count - (int)m_envData.auxSendValues.m_Count);

			for(int i = 0; i < environmentsLeft; i++)
			{
				if(!m_envData.auxSendValues.Contains(sortedEnvList[i].GetAuxBusID()))
				{
					//An environment with the isDefault flag set to true is added only if its the only environment.
					//Since an environment with the isDefault flag has the lowest priority, it will be at index zero only if there is no other environment
					if(sortedEnvList[i].isDefault && i != 0)
						continue;

					m_envData.auxSendValues.Add(sortedEnvList[i].GetAuxBusID(), sortedEnvList[i].GetAuxSendValueForPosition(transform.position));

					//No other environment can be added after an environment with the excludeOthers flag set to true
					if(sortedEnvList[i].excludeOthers)
						break;
				}
			}
		}

		AkSoundEngine.SetGameObjectAuxSendValues(gameObject, m_envData.auxSendValues, m_envData.auxSendValues.m_Count);
    }    

#if UNITY_EDITOR
	public void OnDrawGizmosSelected()
	{
		Vector3 position = GetPosition();
		Gizmos.DrawIcon(position, "WwiseAudioSpeaker.png", false);
	}
#endif
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.