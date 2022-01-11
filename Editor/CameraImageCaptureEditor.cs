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
        Cic.InitDic();
        camera = serializedObject.FindProperty(nameof(Cic.targetCamera));
        imageRes = serializedObject.FindProperty(nameof(Cic.imageResolution));
        //fileName = serializedObject.FindProperty(nameof(Cic.fileName));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(camera);
        EditorGUILayout.PropertyField(imageRes);
        if (Cic.saveFolderPath == null || Cic.saveFolderPath.Length == 0) Cic.saveFolderPath = Application.persistentDataPath;
        Cic.saveFolderPath = EditorGUILayout.TextField("Save folder path", Cic.saveFolderPath);
        Cic.fileName = EditorGUILayout.TextField("File name", Cic.fileName);
        Cic.isUseThreat = EditorGUILayout.Toggle("Is use threat", Cic.isUseThreat);

        if(GUILayout.Button("Capture and save"))
        {
            Cic.CaptureAndSaveImage();
        }


        serializedObject.ApplyModifiedProperties();
    }

}