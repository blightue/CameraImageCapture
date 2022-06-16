using SuiSuiShou.CIC.Data;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    [System.Serializable]
    public enum ImageFormat
    { jpg, png, tga };

    [AddComponentMenu("Camera Image Capture/CameraImageCapture")]
    public class CameraImageCapture : CameraImageCaptureBase
    {
        private Vector2Int imageResolution = new Vector2Int(512, 512);

        private WriteFileType writeType = WriteFileType.MainThread;
        private ImageFormat imageFormat = ImageFormat.png;

        private bool isOverrideFile = false;
        private bool isImageSerial = true;

        private string saveFolderPath;
        public string fileName;

        public override Vector2Int ImageResolution { get => imageResolution; set => imageResolution = value; }
        public override WriteFileType WriteType { get => writeType; set => writeType = value; }
        public override ImageFormat ImageFormat { get => imageFormat; set => imageFormat = value; }
        public override string SaveFolderPath { get => saveFolderPath; set => saveFolderPath = value; }
        public override string FileName { get => fileName; set => fileName = value; }
        public override bool IsOverrideFile { get => isOverrideFile; set => isOverrideFile = value; }
        public override bool IsImageSerial { get => isImageSerial; set => isImageSerial = value; }

    }
}