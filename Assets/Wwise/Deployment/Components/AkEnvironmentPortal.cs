#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;


/// @brief Use this component to define an area that straddles two different AkEnvironments zones and allow mixing between both zones.
/// - \ref unity_use_AkEvironment_AkEvironmentPortal
[AddComponentMenu("Wwise/AkEnvironmentPortal")]
[RequireComponent (typeof(BoxCollider))]
[RequireComponent (typeof(Rigidbody))]
[ExecuteInEditMode]
public class AkEnvironmentPortal : MonoBehaviour
{
	public AkEnvironment[]	environments	= new AkEnvironment[2];	///The array is already sortet by position.
																	///The first environment is on the negative side of the portal(opposite to the direction of the chosen axis)
																	///The second environment is on the positive side of the portal
	
	
	public Vector3 			axis 			= new Vector3(1,0,0); ///The axis used to find the contribution of each environment

	public  float GetAuxSendValueForPosition(Vector3 in_position, int index)
	{
		//total lenght of the portal in the direction of axis
		float portalLenght = Vector3.Dot (Vector3.Scale(GetComponent<BoxCollider> ().size, transform.lossyScale), axis);

		//transform axis to world coordinates 
		Vector3 axisWorld = Vector3.Normalize( transform.rotation * axis);

		//Get distance form left side of the portal(opposite to the direction of axis) to the game object in the direction of axisWorld
		float dist = Vector3.Dot ( in_position - (transform.position - (portalLenght*0.5f*axisWorld)), axisWorld);

		//calculate value of the environment referred by index 
		if(index == 0) 
			return ((portalLenght - dist) * (portalLenght - dist)) / (portalLenght*portalLenght);
		else 
			return (dist * dist) / (portalLenght*portalLenght);
	}


	///This enables us to detect intersections between portals and environments in the editor 
#if UNITY_EDITOR
	[Serializable]
	public class EnvListWrapper
	{
		public List<AkEnvironment> list = new List<AkEnvironment>();
	}

	//Unity can't serialize an array of list so we wrap the list in a serializable class 
	public EnvListWrapper[] envList = new EnvListWrapper[]
	{
		new EnvListWrapper(),	//All environments on the negative side of each portal(opposite to the direction of the chosen axis)
		new EnvListWrapper()	//All environments on the positive side of each portal(same direction as the chosen axis)
	};
#endif
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.