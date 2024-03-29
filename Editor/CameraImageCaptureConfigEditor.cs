﻿using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;

[CustomEditor(typeof(CameraImageCaptureWithConfig))]
[CanEditMultipleObjects]
public class CameraImageCaptureConfigEditor : CameraImageCaptureBaseEditor
{
    protected CameraImageCaptureWithConfig cicConfig;

    protected CameraImageCaptureWithConfig CICConfig
    {
        get
        {
            if (cicConfig == null)
            {
                cicConfig = (CameraImageCaptureWithConfig) target;
            }

            return cicConfig;
        }
    }


    public override void OnInspectorGUI()
    {
        CICConfig.Config =
            (CapturerConfig) EditorGUILayout.ObjectField("Config File", CICConfig.Config, typeof(CapturerConfig), false);

        InspectorComponents();

        if (CICConfig.Config != null)
        {
            EditorGUI.BeginChangeCheck();
            {
                InspectorFileSetting();
                InspectorOthers();
            }
            if(EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(CICConfig.Config);
        }
        else
        {
            InspectorNotSelect();
        }

        GUI.enabled = CICConfig.Config != null;
        InspectorButtons();
        GUI.enabled = true;
    }

    protected void InspectorNotSelect()
    {
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Select a capture config file first");
        EditorGUILayout.LabelField("Creat capture config via Creat/Camera Image Capture/Capturer config");
        GUILayout.EndVertical();
    }
}