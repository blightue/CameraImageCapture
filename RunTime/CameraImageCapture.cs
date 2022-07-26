using System.Collections.Generic;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    [AddComponentMenu("Camera Image Capture/Capturer")]
    public class CameraImageCapture : MonoBehaviour, CameraImageCaptureCore
    {
        [SerializeField] private Vector2Int imageResolution = new Vector2Int(512, 512);
        [SerializeField] private Camera targetCamera;


        [SerializeField] private WriteFileType writeType = WriteFileType.MainThread;
        [SerializeField] private ImageFormat imageFormat = ImageFormat.png;

        [SerializeField] private bool isImageSerial = true;
        [SerializeField] private bool isLogCap;

        [SerializeField] private string saveFolderPath;
        [SerializeField] private string fileName = "CameraShot";

        [SerializeField] private bool isOverrideCameraResolution;

        private Dictionary<FileInfor, int> fileInfors = new Dictionary<FileInfor, int>();

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

        public Vector2Int ImageResolution
        {
            get => imageResolution;
            set => imageResolution = value;
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
            get => writeType;
            set => writeType = value;
        }

        public ImageFormat ImageFormat
        {
            get => imageFormat;
            set => imageFormat = value;
        }

        public string SaveFolderPath
        {
            get => saveFolderPath;
            set => saveFolderPath = value;
        }

        public string FileName
        {
            get => fileName;
            set => fileName = value;
        }

        public bool IsLogCap
        {
            get => isLogCap;
            set => isLogCap = value;
        }

        public bool IsImageSerial
        {
            get => isImageSerial;
            set => isImageSerial = value;
        }

        public bool IsOverrideCameraResolution
        {
            get => isOverrideCameraResolution;
            set => isOverrideCameraResolution = value;
        }
    }
}