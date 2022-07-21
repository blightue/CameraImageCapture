using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;

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
            InspectorFileSetting();

            InspectorOthers();
        }
        else
        {
            InspectorNoteSelect();
        }

        GUI.enabled = CICConfig.Config != null;
        InspectorButtons();
        GUI.enabled = true;
    }

    protected void InspectorNoteSelect()
    {
        GUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Select a capture config file first");
        EditorGUILayout.LabelField("Creat capture config via Creat/Camera Image Capture/Capturer config");
        GUILayout.EndVertical();
    }
}