using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;
using SuiSuiShou.CIC.Data;

[CanEditMultipleObjects]
public abstract class CameraImageCaptureBaseEditor : Editor
{
    protected CameraImageCapture cic;

    protected SerializedProperty camera;
    //private SerializedProperty imageRes;
    //private SerializedProperty writeType;
    //private SerializedProperty imageFormat;

    protected bool showFileSetting = true;
    protected bool showComponents = true;

    protected string foldPathPanel;

    public CameraImageCapture CIC
    {
        get
        {
            if (cic == null)
                cic = (CameraImageCapture)target;
            return cic;
        }
    }

    protected virtual void OnEnable()
    {
        CIC.fileInfors = CaptureInforManager.ReadLocalData();
        camera = serializedObject.FindProperty(nameof(CIC.targetCamera));
        //imageRes = serializedObject.FindProperty(nameof(Cic.ImageResolution));
        //writeType = serializedObject.FindProperty(nameof(Cic.WriteType));
        //imageFormat = serializedObject.FindProperty(nameof(Cic.ImageFormat));

        //fileName = serializedObject.FindProperty(nameof(Cic.fileName));
    }

    protected virtual void OnDestroy()
    {
        CaptureInforManager.WriteLocalData(CIC.fileInfors);
    }


    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        InspectorComponents();

        InspectorFileSetting();

        InspectorOthers();

        if (GUILayout.Button("Capture and save")) CIC.CaptureAndSaveImage();

        serializedObject.ApplyModifiedProperties();
    }

    protected virtual void InspectorComponents()
    {
        showComponents = EditorGUILayout.BeginFoldoutHeaderGroup(showComponents, "Components");
        if (showComponents)
        {
            EditorGUILayout.PropertyField(camera);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorFileSetting()
    {
        showFileSetting = EditorGUILayout.BeginFoldoutHeaderGroup(showFileSetting, "Export setting");
        if (showFileSetting)
        {
            CIC.fileName = EditorGUILayout.TextField("File name", CIC.fileName);

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Export folder");
            if (GUILayout.Button("Change folder"))
            {
                foldPathPanel = EditorUtility.OpenFolderPanel("Select export path", CIC.SaveFolderPath, "");
                if (foldPathPanel != "") CIC.SaveFolderPath = foldPathPanel;
            }
            GUILayout.EndHorizontal();

            if (CIC.SaveFolderPath == null || CIC.SaveFolderPath.Length == 0) CIC.SaveFolderPath = Application.persistentDataPath;
            EditorStyles.textArea.wordWrap = true;
            CIC.SaveFolderPath = EditorGUILayout.TextArea(CIC.SaveFolderPath, GUILayout.Height(40));
            GUILayout.EndVertical();
            
            GUILayout.BeginVertical("box");
            CIC.IsImageSerial = EditorGUILayout.Toggle("Image serialized", CIC.IsImageSerial);
            CIC.IsOverrideFile = EditorGUILayout.Toggle("Override file", CIC.IsOverrideFile);
            CIC.WriteType = (WriteFileType)EditorGUILayout.EnumPopup("Write type", CIC.WriteType);
            CIC.ImageFormat = (ImageFormat)EditorGUILayout.EnumPopup("Image format", CIC.ImageFormat);
            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorOthers()
    {
        CIC.ImageResolution = EditorGUILayout.Vector2IntField("Image resolution", CIC.ImageResolution);

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}