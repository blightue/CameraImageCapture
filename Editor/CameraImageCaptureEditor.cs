using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(CameraImageCapture))]
[CanEditMultipleObjects]
public class CameraImageCaptureEditor : CameraImageCaptureBaseEditor
{



    public override void OnInspectorGUI()
    {
        
        //EditorGUI.BeginChangeCheck();
        {
            InspectorComponents();

            InspectorFileSetting();

            InspectorOthers();

            InspectorButtons();
        }
        //bool isChanged = EditorGUI.EndChangeCheck();
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty((target as CameraImageCapture).gameObject.scene);
        }
        
    }
}