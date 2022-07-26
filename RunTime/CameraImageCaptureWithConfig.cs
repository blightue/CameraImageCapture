using System.Collections.Generic;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    [AddComponentMenu("Camera Image Capture/Capturer with config")]
    public class CameraImageCaptureWithConfig : MonoBehaviour, CameraImageCaptureCore
    {
        [HideInInspector] public CapturerConfig Config;

        [SerializeField] [HideInInspector] private Camera targetCamera;

        public Dictionary<FileInfor, int> fileInfors = new Dictionary<FileInfor, int>();

        public Vector2Int ImageResolution
        {
            get => Config.imageResolution;
            set => Config.imageResolution = value;
        }

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

        public WriteFileType WriteType
        {
            get => Config.writeType;
            set => Config.writeType = value;
        }

        public ImageFormat ImageFormat
        {
            get => Config.imageFormat;
            set => Config.imageFormat = value;
        }

        public bool IsLogCap
        {
            get => Config.isLogCap;
            set => Config.isLogCap = value;
        }

        public bool IsImageSerial
        {
            get => Config.isImageSerial;
            set => Config.isImageSerial = value;
        }

        public string SaveFolderPath
        {
            get => Config.saveFolderPath;
            set => Config.saveFolderPath = value;
        }

        public string FileName
        {
            get => Config.fileName;
            set => Config.fileName = value;
        }

        public bool IsOverrideCameraResolution
        {
            get => Config.isOverrideCameraResolution;
            set => Config.isOverrideCameraResolution = value;
        }
        
        public virtual void Reset()
        {
            TargetCamera = Camera.main;
            FileInfors = CaptureInforManager.ReadLocalData();
            SaveFolderPath = Application.persistentDataPath;
            Debug.Log("Reset CIC");
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            CaptureInforManager.WriteLocalData(FileInfors);
#endif
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            FileInfors = CaptureInforManager.ReadLocalData();
#endif
        }
    }
}