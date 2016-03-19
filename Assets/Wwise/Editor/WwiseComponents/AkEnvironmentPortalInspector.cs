#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;


[CustomEditor(typeof(AkEnvironmentPortal))]
public class AkEnvironmentPortalInspector : Editor
{
	[MenuItem("GameObject/Wwise/Environment Portal", false, 1)]
	public static void CreatePortal()
	{
		GameObject portal = new GameObject ("EnvironmentPortal");
	
		portal.AddComponent<AkEnvironmentPortal> ();

		portal.GetComponent<Collider>().isTrigger = true;

		portal.GetComponent<Rigidbody>().useGravity = false;
		portal.GetComponent<Rigidbody>().isKinematic = true;
		portal.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

		Selection.objects = new UnityEngine.Object[]{portal};
	}

	AkEnvironmentPortal 	m_envPortal;
	int[]					m_selectedIndex = new int[2];

	void OnEnable()
	{
		m_envPortal = target as AkEnvironmentPortal;
        FindOverlappingEnvironments();
		for(int i = 0; i < 2; i++) 
		{
			int index = m_envPortal.envList[i].list.IndexOf (m_envPortal.environments [i]);
			m_selectedIndex [i] = index == -1 ? 0 : index;
		}
	}

	public override void OnInspectorGUI()
	{
		GUILayout.BeginVertical ("Box"); 
		{
			for(int i = 0; i < 2; i++)
			{
				string[] labels = new String[m_envPortal.envList[i].list.Count];
					
				for(int j = 0; j < labels.Length; j++)
				{					
					if(m_envPortal.envList[i].list[j] != null)
					{
						labels[j] = j+1 + ". " + GetEnvironmentName(m_envPortal.envList[i].list[j]) + " (" + m_envPortal.envList[i].list[j].name + ")"; 
					}
					else
					{
						m_envPortal.envList[i].list.RemoveAt(j);
					}
				}

				m_selectedIndex[i] = EditorGUILayout.Popup("Environment #" + (i+1), m_selectedIndex[i], labels);  

				m_envPortal.environments [i] = (m_selectedIndex [i] < 0 || m_selectedIndex [i] >= m_envPortal.envList[i].list.Count) ? null : m_envPortal.envList [i].list [m_selectedIndex [i]];
			}
		}
		GUILayout.EndVertical (); 
	
		GUILayout.Space (2);

		GUILayout.BeginVertical("Box"); 
		{
			string[] axisLabels = {"X", "Y", "Z"};

			int index = 0;
			for(int i = 0; i < 3; i++)
			{
				if(m_envPortal.axis[i] == 1) 
				{
					index = i;
					break;
				}
			}

			index = EditorGUILayout.Popup ("Axis" , index, axisLabels);

			if(m_envPortal.axis[index] != 1)
			{
				m_envPortal.axis.Set (0, 0, 0);
                m_envPortal.envList = new AkEnvironmentPortal.EnvListWrapper[]
	            {
		            new AkEnvironmentPortal.EnvListWrapper(),
		            new AkEnvironmentPortal.EnvListWrapper()
                };
				m_envPortal.axis [index] = 1;

				//We move and replace the game object to trigger the OnTriggerStay function
				FindOverlappingEnvironments();
			}
		}
		GUILayout.EndVertical ();

		Repaint ();
	}

	string GetEnvironmentName(AkEnvironment in_env)
	{
		for(int i = 0; i < AkWwiseProjectInfo.GetData().AuxBusWwu.Count; i++)
		{
			for(int j = 0; j < AkWwiseProjectInfo.GetData().AuxBusWwu[i].List.Count; j++)
			{
				if(in_env.GetAuxBusID() == (uint)AkWwiseProjectInfo.GetData().AuxBusWwu[i].List[j].ID)
				{
					return AkWwiseProjectInfo.GetData().AuxBusWwu[i].List[j].Name;
				}
			}
		}

		return String.Empty;
	}

    public void FindOverlappingEnvironments()
    {
        Collider myCollider = m_envPortal.gameObject.GetComponent<Collider>();
        if (myCollider == null)
        {
            return;
        }

        AkEnvironment[] environments = FindObjectsOfType<AkEnvironment>();
        foreach (AkEnvironment environment in environments)
        {
            Collider otherCollider = environment.gameObject.GetComponent<Collider>();
            if (otherCollider == null)
            {
                continue;
            }

            if (myCollider.bounds.Intersects(otherCollider.bounds))
            {
                //if index == 0 => the environment is on the negative side of the portal(opposite to the direction of the chosen axis)
                //if index == 1 => the environment is on the positive side of the portal(same direction as the chosen axis) 
                int index = (Vector3.Dot(m_envPortal.transform.rotation * m_envPortal.axis, environment.transform.position - m_envPortal.transform.position) >= 0) ? 1 : 0;
                if (!m_envPortal.envList[index].list.Contains(environment))
                {
                    m_envPortal.envList[index].list.Add(environment);
                    m_envPortal.envList[++index % 2].list.Remove(environment);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    m_envPortal.envList[i].list.Remove(environment);
                }
            }
        }
    }
}

#endif