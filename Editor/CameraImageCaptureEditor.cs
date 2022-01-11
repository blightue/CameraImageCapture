using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomEditor(typeof(CameraImageCapture))]
[CanEditMultipleObjects]
public class CameraImageCaptureEditor : Editor
{
    private CameraImageCapture cic;

    private SerializedProperty camera;
    private SerializedProperty imageRes;
    //private SerializedProperty savePath;
    private SerializedProperty fileName;

    public CameraImageCapture Cic
    {
        get
        {
            if (cic == null)
                cic = (CameraImageCapture)target;
            return cic;
        }
    }

    private void OnEnable()
    {
        ResetObj();
        camera = serializedObject.FindProperty(nameof(Cic.targetCamera));
        imageRes = serializedObject.FindProperty(nameof(Cic.imageResolution));
        //savePath = serializedObject.FindProperty(nameof(Cic.saveFolderPath));
        fileName = serializedObject.FindProperty(nameof(Cic.FileName));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(camera);
        EditorGUILayout.PropertyField(imageRes);
        //EditorGUILayout.PropertyField(fileName);
        if (Cic.saveFolderPath == null || Cic.saveFolderPath.Length == 0) Cic.saveFolderPath = Application.persistentDataPath;
        Cic.saveFolderPath = EditorGUILayout.TextField("Save folder path", Cic.saveFolderPath);
        //Cic.FileName = EditorGUILayout.TextField("File name", Cic.FileName);
        Cic.isUseThreat = EditorGUILayout.Toggle("Is use threat", Cic.isUseThreat);

        if(GUILayout.Button("Capture and save"))
        {
            Cic.CaptureAndSaveImage();
        }


        serializedObject.ApplyModifiedProperties();
    }

    private void ResetObj()
    {
        if (Cic.targetCamera != null && 
            Cic.saveFolderPath == null && 
            Cic.saveFolderPath == "" &&
            Cic.FileName == null &&
            Cic.FileName == "") 
            Cic.Reset();
    }    

}