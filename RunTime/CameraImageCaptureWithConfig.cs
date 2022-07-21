using SuiSuiShou.CIC.Data;
using UnityEngine;

namespace SuiSuiShou.CIC.Core
{
    [AddComponentMenu("Camera Image Capture/Capturer with config")]
    public class CameraImageCaptureWithConfig : CameraImageCaptureBase
    {
        [HideInInspector] public CapturerConfig Config;

        public override Vector2Int ImageResolution
        {
            get => Config.imageResolution;
            set => Config.imageResolution = value;
        }

        public override WriteFileType WriteType
        {
            get => Config.writeType;
            set => Config.writeType = value;
        }

        public override ImageFormat ImageFormat
        {
            get => Config.imageFormat;
            set => Config.imageFormat = value;
        }

        public override bool IsLogCap
        {
            get => Config.isLogCap;
            set => Config.isLogCap = value;
        }

        public override bool IsImageSerial
        {
            get => Config.isImageSerial;
            set => Config.isImageSerial = value;
        }

        public override string SaveFolderPath
        {
            get => Config.saveFolderPath;
            set => Config.saveFolderPath = value;
        }

        public override string FileName
        {
            get => Config.fileName;
            set => Config.fileName = value;
        }

        public override bool IsOverrideCameraResolution
        {
            get => Config.isOverrideCameraResolution;
            set => Config.isOverrideCameraResolution = value;
        }


    }
}