using SuiSuiShou.CIC.Data;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    [System.Serializable]
    public enum ImageFormat
    { jpg, png, tga };

    [AddComponentMenu("Camera Image Capture/Capturer")]
    public class CameraImageCapture : CameraImageCaptureBase
    {
        [SerializeField] private Vector2Int imageResolution = new Vector2Int(512, 512);

        [SerializeField] private WriteFileType writeType = WriteFileType.MainThread;
        [SerializeField] private ImageFormat imageFormat = ImageFormat.png;

        [SerializeField] private bool isImageSerial = true;
        [SerializeField] private bool isLogCap;

        [SerializeField] private string saveFolderPath;
        [SerializeField] private string fileName = "CameraShot";
        
        [SerializeField] private bool isOverrideCameraResolution;

        public override void Reset()
        {
            base.Reset();
            SaveFolderPath = Application.persistentDataPath;
        }

        public override Vector2Int ImageResolution { get => imageResolution; set => imageResolution = value; }
        public override WriteFileType WriteType { get => writeType; set => writeType = value; }
        public override ImageFormat ImageFormat { get => imageFormat; set => imageFormat = value; }
        public override string SaveFolderPath { get => saveFolderPath; set => saveFolderPath = value; }
        public override string FileName { get => fileName; set => fileName = value; }

        public override bool IsLogCap { get => isLogCap; set => isLogCap = value; }
        public override bool IsImageSerial { get => isImageSerial; set => isImageSerial = value; }
        public override bool IsOverrideCameraResolution { get => isOverrideCameraResolution; set => isOverrideCameraResolution = value; }
    }
}