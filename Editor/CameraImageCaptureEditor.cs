using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;

[CustomEditor(typeof(CameraImageCapture))]
[CanEditMultipleObjects]
public class CameraImageCaptureEditor : CameraImageCaptureBaseEditor
{



    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InspectorComponents();

        InspectorFileSetting();

        InspectorOthers();

        InspectorButtons();
        
        serializedObject.ApplyModifiedProperties();
    }
}