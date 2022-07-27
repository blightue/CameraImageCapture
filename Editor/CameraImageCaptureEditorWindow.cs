using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SuiSuiShou.CIC.Core;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;


public class CameraImageCaptureEditorWindow : EditorWindow, ICameraImageCaptureCore
{
    private CapturerConfig config;

    private Dictionary<FileInfor, int> fileInfors;
    private Camera targetCamera;

    private bool showComponents;
    private bool showFileSetting;

    private string foldPathPanel;

    public Dictionary<FileInfor, int> FileInfors
    {
        get => fileInfors;
        set => fileInfors = value;
    }

    public Camera TargetCamera
    {
        get => targetCamera;
        set => targetCamera = value;
    }

    public Vector2Int ImageResolution
    {
        get => config.imageResolution;
        set => config.imageResolution = value;
    }

    public WriteFileType WriteType
    {
        get => config.writeType;
        set => config.writeType = value;
    }

    public ImageFormat ImageFormat
    {
        get => config.imageFormat;
        set => config.imageFormat = value;
    }

    public bool IsLogCap
    {
        get => config.isLogCap;
        set => config.isLogCap = value;
    }

    public bool IsImageSerial
    {
        get => config.isImageSerial;
        set => config.isImageSerial = value;
    }

    public bool IsOverrideCameraResolution
    {
        get => config.isOverrideCameraResolution;
        set => config.isOverrideCameraResolution = value;
    }

    public string SaveFolderPath
    {
        get => config.saveFolderPath;
        set => config.saveFolderPath = value;
    }

    public string FileName
    {
        get => config.fileName;
        set => config.fileName = value;
    }

    [MenuItem("Tools/Camera Image Capture/Capturer")]
    private static void ShowWindow()
    {
        var window = GetWindow<CameraImageCaptureEditorWindow>();
        window.titleContent = new GUIContent("Camera Image Capture");
        window.targetCamera = Camera.main;
        window.Show();
    }
    
    protected virtual void OnEnable()
    {
        this.FileInfors = CaptureInforManager.ReadLocalData();
    }

    protected virtual void OnDestroy()
    {
        CaptureInforManager.WriteLocalData(this.FileInfors);
    }

    private void OnGUI()
    {
        config = (CapturerConfig) EditorGUILayout.ObjectField("Config", config, typeof(CapturerConfig), false);

        InspectorComponents();

        if (config == null)
        {
            InspectorNoteSelect();
        }
        else
        {
            InspectorFileSetting();

            InspectorOthers();
        }

        GUI.enabled = config != null;
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

    protected virtual void InspectorComponents()
    {
        showComponents = EditorGUILayout.BeginFoldoutHeaderGroup(showComponents, "Components");
        if (showComponents)
        {
            TargetCamera =
                (Camera) EditorGUILayout.ObjectField("Target Camera", TargetCamera, typeof(Camera), true);

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorFileSetting()
    {
        showFileSetting = EditorGUILayout.BeginFoldoutHeaderGroup(showFileSetting, "Export setting");
        if (showFileSetting)
        {
            FileName = EditorGUILayout.TextField("File name", FileName);

            GUILayout.BeginVertical("box");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Export folder");
            if (GUILayout.Button("Change folder"))
            {
                foldPathPanel = EditorUtility.OpenFolderPanel("Select export path", SaveFolderPath, "");
                if (foldPathPanel != "") SaveFolderPath = foldPathPanel;
            }

            GUILayout.EndHorizontal();

            if (SaveFolderPath == null || SaveFolderPath.Length == 0)
                SaveFolderPath = Application.persistentDataPath;
            EditorStyles.textArea.wordWrap = true;
            SaveFolderPath = EditorGUILayout.TextArea(SaveFolderPath, GUILayout.Height(40));
            GUILayout.EndVertical();

            GUILayout.BeginVertical("box");
            IsImageSerial = EditorGUILayout.Toggle("Image serialized", IsImageSerial);
            IsLogCap = EditorGUILayout.Toggle("Log Capture", IsLogCap);
            // CIC.IsOverrideFile = EditorGUILayout.Toggle("Override file", CIC.IsOverrideFile);
            WriteType = (WriteFileType) EditorGUILayout.EnumPopup("Write type", WriteType);
            ImageFormat = (ImageFormat) EditorGUILayout.EnumPopup("Image format", ImageFormat);
            GUILayout.EndVertical();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    protected virtual void InspectorOthers()
    {
        //EditorGUILayout.LabelField("Image resolution");
        IsOverrideCameraResolution =
            EditorGUILayout.Toggle("Is Override Camera Resolution", IsOverrideCameraResolution);
        if (!IsOverrideCameraResolution)
            ImageResolution = new Vector2Int(TargetCamera.pixelWidth, TargetCamera.pixelHeight);
        GUI.enabled = IsOverrideCameraResolution;
        ImageResolution = EditorGUILayout.Vector2IntField("Image resolution", ImageResolution);
        GUI.enabled = true;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }

    protected virtual void InspectorButtons()
    {
        if (GUILayout.Button("Capture and save")) this.CaptureAndSaveImage();
#if UNITY_EDITOR_WIN
        if (GUILayout.Button("Show in exporter")) EditorUtility.RevealInFinder(SaveFolderPath);
#elif UNITY_EDITOR_MAC
        if (GUILayout.Button("Reveal In Finder")) EditorUtility.RevealInFinder(CIC.SaveFolderPath);
#endif
    }
}