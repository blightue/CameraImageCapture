using System;
using UnityEngine;
using SuiSuiShou.CIC.Data;


namespace SuiSuiShou.CIC.Core
{
    [CreateAssetMenu(menuName = "Camera Capture/Capture config")]
    public class CaptureConfig : ScriptableObject
    {
        public int ImageWidth = 1920;
        public int ImageHeight = 1080;
        public WriteFileType writeType = WriteFileType.MainThread;
        public ImageFormat imageFormat = ImageFormat.png;

        public string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string fileName = "CameraShot";

        public bool ImageSerial = true;
    }
}