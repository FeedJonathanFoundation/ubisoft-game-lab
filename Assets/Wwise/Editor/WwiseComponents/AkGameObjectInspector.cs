﻿#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

public class DefaultHandles
{
    public static bool Hidden
    {
        get
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            return ((bool)field.GetValue(null));
        }
        set
        {
            Type type = typeof(Tools);
            FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, value);
        }
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(AkGameObj))]
public class AkGameObjectInspector : Editor
{
    AkGameObj m_AkGameObject;    
	    
    bool hideDefaultHandle = false;

    void OnEnable()
    {
        m_AkGameObject = target as AkGameObj;        

        DefaultHandles.Hidden = hideDefaultHandle;
    }

    void OnDisable()
    {
        DefaultHandles.Hidden = false;
    }

    public override void OnInspectorGUI()
    {           

		GUILayout.BeginVertical("Box");

		// Unity tries to construct a AkGameObjPositionOffsetData all the time. Need this ugly workaround
		// to prevent it from doing this.
        if (m_AkGameObject.m_positionOffsetData != null)
        {
            if(!m_AkGameObject.m_positionOffsetData.KeepMe)
            {
                m_AkGameObject.m_positionOffsetData = null;
            }
        }

		bool applyPosOffset = m_AkGameObject.m_positionOffsetData != null;
		applyPosOffset = EditorGUILayout.Toggle ("Apply Position Offset: ", applyPosOffset);
		if(m_AkGameObject.m_positionOffsetData == null && applyPosOffset)
		{
			m_AkGameObject.m_positionOffsetData = new AkGameObjPositionOffsetData(true);
        }
        else if(!applyPosOffset && m_AkGameObject.m_positionOffsetData != null)
		{
			m_AkGameObject.m_positionOffsetData = null;
		}

		if (m_AkGameObject.m_positionOffsetData != null) 
		{
			m_AkGameObject.m_positionOffsetData.positionOffset = EditorGUILayout.Vector3Field("Position Offset", m_AkGameObject.m_positionOffsetData.positionOffset);

			GUILayout.Space(2);
			
			if (hideDefaultHandle)
			{
				
				if (GUILayout.Button("Show Main Transform"))
				{
					hideDefaultHandle = false;
					DefaultHandles.Hidden = hideDefaultHandle;
				}
			}
			else
			{
				if (GUILayout.Button("Hide Main Transform"))
				{
					hideDefaultHandle = true;
					DefaultHandles.Hidden = hideDefaultHandle;
				}
			}
		}
		else
		{
			if (hideDefaultHandle == true)
			{
				hideDefaultHandle = false;
				DefaultHandles.Hidden = hideDefaultHandle;
			}
		}

		GUILayout.EndVertical ();

		GUILayout.Space (3);

		GUILayout.BeginVertical ("Box");


		m_AkGameObject.isEnvironmentAware = EditorGUILayout.Toggle ("Environment Aware: ", m_AkGameObject.isEnvironmentAware);

		if (m_AkGameObject.isEnvironmentAware && m_AkGameObject.GetComponent<Rigidbody>() == null)
		{
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.red;
			style.wordWrap = true;
			GUILayout.Label("Objects affected by Environment need to have a RigidBody attached.", style);
			if (GUILayout.Button("Add Rigidbody!"))
			{
				Rigidbody rb = m_AkGameObject.gameObject.AddComponent<Rigidbody>();
				rb.useGravity = false;
				rb.isKinematic = true;
			}
		} 

		GUILayout.EndVertical (); 
		
		GUILayout.Space (3);

		string [] maskLabels = new string[AkSoundEngine.AK_NUM_LISTENERS];
		for(int i = 0; i < AkSoundEngine.AK_NUM_LISTENERS; i++)
		{
			maskLabels[i] = "L" + i;
		}
		m_AkGameObject.listenerMask = EditorGUILayout.MaskField("Listeners", m_AkGameObject.listenerMask, maskLabels); 

        if (GUI.changed)
        {
            EditorUtility.SetDirty(m_AkGameObject);
        }
    }
	      
    void OnSceneGUI()
    {
        if (m_AkGameObject.m_positionOffsetData == null)
            return;

        // Transform local offset to world coordinate
        Vector3 pos = m_AkGameObject.transform.TransformPoint(m_AkGameObject.m_positionOffsetData.positionOffset);

        // Get new handle position
        pos = Handles.PositionHandle(pos, Quaternion.identity);

        // Transform wolrd offset to local coordintae
        m_AkGameObject.m_positionOffsetData.positionOffset = m_AkGameObject.transform.InverseTransformPoint(pos);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}
#endif