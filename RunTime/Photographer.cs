using System.Collections.Generic;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    public static class Photographer
    {
        private static ImageCapture _imageCapture;
        
        
    }

    internal class ImageCapture : ICameraImageCaptureCore
    {
        public Vector2Int ImageResolution { get; set; } = new Vector2Int(512, 512);
        public Dictionary<FileInfor, int> FileInfors { get; set; } = new Dictionary<FileInfor, int>();
        public Camera TargetCamera { get; set; } = Camera.main;
        public WriteFileType WriteType { get; set; } = WriteFileType.MainThread;
        public ImageFormat ImageFormat { get; set; } = ImageFormat.png;
        public bool IsLogCap { get; set; }

        public bool IsImageSerial { get; set; } = true;
        public bool IsOverrideCameraResolution { get; set; }

        public string SaveFolderPath { get; set; } = Application.persistentDataPath;
        public string FileName { get; set; } = "CameraShot";
    }
}