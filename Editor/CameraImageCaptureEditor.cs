using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;

[CustomEditor(typeof(CameraImageCapture))]
[CanEditMultipleObjects]
public class CameraImageCaptureEditor : CameraImageCaptureBaseEditor
{
    protected CameraImageCapture cic;

    //protected SerializedProperty camera;
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
                cic = (CameraImageCapture) target;
            return cic;
        }
    }

    protected virtual void OnEnable()
    {
        CIC.fileInfors = CaptureInforManager.ReadLocalData();
        //camera = serializedObject.FindProperty(nameof(CIC.targetCamera));
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
            CIC.targetCamera =
                (Camera) EditorGUILayout.ObjectField("Target Camera", CIC.targetCamera, typeof(Camera), true);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorFileSetting()
    {
        showFileSetting = EditorGUILayout.BeginFoldoutHeaderGroup(showFileSetting, "Export setting");
        if (showFileSetting)
        {
            CIC.FileName = EditorGUILayout.TextField("File name", CIC.FileName);

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Export folder");
            if (GUILayout.Button("Change folder"))
            {
                foldPathPanel = EditorUtility.OpenFolderPanel("Select export path", CIC.SaveFolderPath, "");
                if (foldPathPanel != "") CIC.SaveFolderPath = foldPathPanel;
            }

            GUILayout.EndHorizontal();

            if (CIC.SaveFolderPath == null || CIC.SaveFolderPath.Length == 0)
                CIC.SaveFolderPath = Application.persistentDataPath;
            EditorStyles.textArea.wordWrap = true;
            CIC.SaveFolderPath = EditorGUILayout.TextArea(CIC.SaveFolderPath, GUILayout.Height(40));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            CIC.IsImageSerial = EditorGUILayout.Toggle("Image serialized", CIC.IsImageSerial);
            CIC.IsLogCap = EditorGUILayout.Toggle("Log Capture", CIC.IsLogCap);
            // CIC.IsOverrideFile = EditorGUILayout.Toggle("Override file", CIC.IsOverrideFile);
            CIC.WriteType = (WriteFileType) EditorGUILayout.EnumPopup("Write type", CIC.WriteType);
            CIC.ImageFormat = (ImageFormat) EditorGUILayout.EnumPopup("Image format", CIC.ImageFormat);
            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorOthers()
    {
        //EditorGUILayout.LabelField("Image resolution");
        CIC.IsOverrideCameraResolution =
            EditorGUILayout.Toggle("Is Override Camera Resolution", CIC.IsOverrideCameraResolution);
        if (!CIC.IsOverrideCameraResolution)
            CIC.ImageResolution = new Vector2Int(CIC.targetCamera.pixelWidth, CIC.targetCamera.pixelHeight);
        GUI.enabled = CIC.IsOverrideCameraResolution;
        CIC.ImageResolution = EditorGUILayout.Vector2IntField("Image resolution", CIC.ImageResolution);
        GUI.enabled = true;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
}