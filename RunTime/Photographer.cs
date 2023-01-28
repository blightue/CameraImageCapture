using System.Collections.Generic;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    public static class Photographer
    {
        private static ImageCapture _imageCapture = new ImageCapture();
        
        /// <summary>
        /// Capture image resolution
        /// </summary>
        public static Vector2Int ImageResolution { get => _imageCapture.ImageResolution; set => _imageCapture.ImageResolution = value; }
        /// <summary>
        /// Target camera for capturing
        /// </summary>
        public static Camera TargetCamera { get  => _imageCapture.TargetCamera; set => _imageCapture.TargetCamera = value; }
        /// <summary>
        /// Image file write type
        /// </summary>
        public static WriteFileType WriteType { get => _imageCapture.WriteType; set => _imageCapture.WriteType = value; }
        /// <summary>
        /// Image type
        /// </summary>
        public static ImageFormat ImageFormat { get => _imageCapture.ImageFormat; set => _imageCapture.ImageFormat = value; }
        /// <summary>
        /// True for logging capture status
        /// </summary>
        public static bool IsLogCap { get => _imageCapture.IsLogCap; set => _imageCapture.IsLogCap = value; }
        /// <summary>
        /// True for sequential image file name
        /// </summary>
        public static bool IsImageSerial { get => _imageCapture.IsImageSerial; set => _imageCapture.IsImageSerial = value; }
        /// <summary>
        /// True for changing ImageResolution in editor
        /// </summary>
        public static bool IsOverrideCameraResolution { get => _imageCapture.IsOverrideCameraResolution; set => _imageCapture.IsOverrideCameraResolution = value; }
        /// <summary>
        /// Fold path for image file to writing
        /// </summary>
        public static string SaveFolderPath { get => _imageCapture.SaveFolderPath; set => _imageCapture.SaveFolderPath = value; }
        /// <summary>
        /// Image file name
        /// </summary>
        public static string FileName { get => _imageCapture.FileName; set => _imageCapture.FileName = value; }

        /// <summary>
        /// Capture and save image file base on this singleton config
        /// </summary>
        public static void CaptureAndSaveImage()
        {
            _imageCapture.CaptureAndSaveImage();
        }
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