using System;
using UnityEngine;
using SuiSuiShou.CIC.Data;


namespace SuiSuiShou.CIC.Core
{
    [CreateAssetMenu(menuName = "Camera Image Capture/Capturer config")]
    public class CapturerConfig : ScriptableObject
    {
        public Vector2Int imageResolution = new Vector2Int(512, 512);
        public WriteFileType writeType = WriteFileType.MainThread;
        public ImageFormat imageFormat = ImageFormat.png;

        public string saveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string fileName = "CameraShot";

        public bool isImageSerial = true;
        public bool isLogCap = false;
        public bool isOverrideCameraResolution = false;
        
    }
}