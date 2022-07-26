using System.IO;
using System.Collections.Generic;
using UnityEngine;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;

namespace SuiSuiShou.CIC.Core
{
    [System.Serializable]
    public enum ImageFormat
    {
        jpg,
        png,
        tga
    };

    public interface CameraImageCaptureCore
    {
        public abstract Vector2Int ImageResolution { get; set; }
        public abstract Dictionary<FileInfor, int> FileInfors { get; set; }
        public abstract Camera TargetCamera { get; set; }

        public abstract WriteFileType WriteType { get; set; }
        public abstract ImageFormat ImageFormat { get; set; }

        public abstract bool IsLogCap { get; set; }
        public abstract bool IsImageSerial { get; set; }
        public abstract bool IsOverrideCameraResolution { get; set; }

        public abstract string SaveFolderPath { get; set; }
        public abstract string FileName { get; set; }
    }

    public static class CameraImageCaptureCoreMethod
    {
#if UNITY_ENABLE_URP

        public static void CaptureAndSaveImage<T>(this T C) where T : CameraImageCaptureCore
        {
            if (C.CheckIsNeedFixedAlpha())
            {
                UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                    C.TargetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                data.renderPostProcessing = false;
                Texture2D texWithAlpha = C.CaptureImage(C.TargetCamera, C.ImageResolution, 8);
                data.renderPostProcessing = true;
                Texture2D texWithoutAlpha = C.CaptureImage(C.TargetCamera, C.ImageResolution, 8);

                Color[] colorWithAlpha = texWithAlpha.GetPixels();
                Color[] colorWithoutAlpha = texWithoutAlpha.GetPixels();
                for (int i = 0; i < colorWithoutAlpha.Length; i++)
                {
                    colorWithoutAlpha[i].a = colorWithAlpha[i].a;
                }

                texWithoutAlpha.SetPixels(colorWithoutAlpha);

                Object.DestroyImmediate(texWithAlpha);

                C.StartSaveImage(C.SaveFolderPath, C.FileName, texWithoutAlpha);
            }
            else
            {
                C.StartSaveImage(C.SaveFolderPath, C.FileName, C.CaptureImage(C.TargetCamera, C.ImageResolution, 8));
            }
        }

        private static bool CheckIsNeedFixedAlpha<T>(this T C) where T : CameraImageCaptureCore
        {
            if (!UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline is UnityEngine.Rendering.Universal
                    .UniversalRenderPipelineAsset)
                return false;

            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                C.TargetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

            if (data == null) return false;

            if (!data.renderPostProcessing) return false;

            return true;
        }
#else
        public void CaptureAndSaveImage()
        {
            StartSaveImage(SaveFolderPath, FileName, CaptureImage(targetCamera, ImageResolution, 8));
        }
#endif

        public static Texture2D CaptureImage<T>(this T C, Camera camera, Vector2Int resolution, int depth)
            where T : CameraImageCaptureCore
        {
            if (!C.CameraCheck(camera)) return null;
            RenderTexture cameraTexture = new RenderTexture(resolution.x, resolution.y, depth);

            camera.targetTexture = cameraTexture;
            camera.Render();

            if (RenderTexture.active != null) RenderTexture.active.Release();
            RenderTexture.active = cameraTexture;

            if (C.IsLogCap)
                Debug.Log("Camera rect = " + camera.pixelWidth + " - " + camera.pixelHeight
                          + "  Screen resolution = " + Screen.width + " - " + Screen.height);
            Texture2D texture2D = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);

            texture2D.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            RenderTexture lastTexture = RenderTexture.active;
            RenderTexture.active = null;
            lastTexture.Release();

            camera.targetTexture = null;

            return texture2D;
        }

        public static void StartSaveImage<T>(this T C, string folderPath, string fileName, Texture2D texture)
            where T : CameraImageCaptureCore
        {
            if (!C.FolderPathCheck(folderPath)) return;
            // fileName = UpdateFileName(fileName);
            if (!C.FileNameCheck(fileName)) return;

            byte[] saveData = null;
            switch (C.ImageFormat)
            {
                //case ImageFormat.exr:
                //    saveData = texture.EncodeToEXR();
                //    break;
                case ImageFormat.jpg:
                    saveData = texture.EncodeToJPG();
                    break;

                case ImageFormat.png:
                    saveData = texture.EncodeToPNG();
                    break;

                case ImageFormat.tga:
                    saveData = texture.EncodeToTGA();
                    break;
            }

            Object.DestroyImmediate(texture);

            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning(string.Format($"Folder path {folderPath} do not exist"));
                Directory.CreateDirectory(folderPath);
            }

            string fullpath;
            if (C.IsImageSerial)
            {
                FileInfor fileInfor = new FileInfor(folderPath, fileName, C.ImageFormat);
                if (!C.FileInfors.ContainsKey(fileInfor)) C.FileInfors[fileInfor] = 0;
                fullpath = Path.Combine(folderPath,
                    fileName + '-' + C.FileInfors[fileInfor].ToString() + '.' + C.ImageFormat.ToString());
                C.FileInfors[fileInfor] = C.FileInfors[fileInfor] + 1;
            }
            else
                fullpath = Path.Combine(folderPath, fileName + '.' + C.ImageFormat.ToString());

            switch (C.WriteType)
            {
                case WriteFileType.MainThread:
                    DataWriter.WriteDataMain(fullpath, saveData);
                    break;

                case WriteFileType.Async:
                    DataWriter.WriteDataTask(fullpath, saveData);
                    break;
                //case WriteFileType.JobSystem:
                //    DataSaver.WriteDataJobS(fullpath, saveData);
                //    break;
            }

            if (C.IsLogCap) Debug.Log("Capture image save to " + fullpath);
        }

        private static bool CameraCheck<T>(this T C, Camera camera) where T : CameraImageCaptureCore
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return false;
            }

            return true;
        }

        private static bool FolderPathCheck<T>(this T C, string path) where T : CameraImageCaptureCore
        {
            if (path == "" || path == null)
            {
                Debug.LogWarning("Savepath is null or empty");
                return false;
            }

            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Savepath do not exist");
                return false;
            }

            return true;
        }

        private static bool FileNameCheck<T>(this T C, string fileName) where T : CameraImageCaptureCore
        {
            if (fileName == "" || fileName == null)
            {
                Debug.LogWarning("fileName is null or empty");
                return false;
            }

            return true;
        }
    }

/*Legecy CICBase
    public abstract class CameraImageCaptureBase : MonoBehaviour
    {
        public const char SerialSeparator = '-';

        #region Properties

        public abstract Vector2Int ImageResolution { get; set; }

        public abstract WriteFileType WriteType { get; set; }
        public abstract ImageFormat ImageFormat { get; set; }

        // public abstract bool IsOverrideFile { get; set; }
        public abstract bool IsLogCap { get; set; }
        public abstract bool IsImageSerial { get; set; }

        public abstract string SaveFolderPath { get; set; }
        public abstract string FileName { get; set; }
        public abstract bool IsOverrideCameraResolution { get; set; }

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

        #endregion

        public Dictionary<FileInfor, int> fileInfors = new Dictionary<FileInfor, int>();


        public Camera targetCamera;


        public virtual void Reset()
        {
            targetCamera = Camera.main;
            // SaveFolderPath = Application.persistentDataPath;
            // ImageFormat = ImageFormat.png;
            // IsOverrideFile = false;
            // IsImageSerial = true;
            // FileName = "cameraCaptures";
            fileInfors = CaptureInforManager.ReadLocalData();
            Debug.Log("Reset CIC");
        }

        private void OnDisable()
        {
#if !UNITY_EDITOR
            CaptureInforManager.WriteLocalData(fileInfors);
#endif
        }

        private void OnEnable()
        {
#if !UNITY_EDITOR
            fileInfors = CaptureInforManager.ReadLocalData();
#endif
        }


#if UNITY_ENABLE_URP

        public void CaptureAndSaveImage()
        {
            if (CheckIsNeedFixedAlpha())
            {
                UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                    targetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                data.renderPostProcessing = false;
                Texture2D texWithAlpha = CaptureImage(targetCamera, ImageResolution, 8);
                data.renderPostProcessing = true;
                Texture2D texWithoutAlpha = CaptureImage(targetCamera, ImageResolution, 8);

                Color[] colorWithAlpha = texWithAlpha.GetPixels();
                Color[] colorWithoutAlpha = texWithoutAlpha.GetPixels();
                for (int i = 0; i < colorWithoutAlpha.Length; i++)
                {
                    colorWithoutAlpha[i].a = colorWithAlpha[i].a;
                }

                texWithoutAlpha.SetPixels(colorWithoutAlpha);

                DestroyImmediate(texWithAlpha);

                StartSaveImage(SaveFolderPath, FileName, texWithoutAlpha);
            }
            else
            {
                StartSaveImage(SaveFolderPath, FileName, CaptureImage(targetCamera, ImageResolution, 8));
            }
        }

        private bool CheckIsNeedFixedAlpha()
        {
            if (!UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline is UnityEngine.Rendering.Universal
                    .UniversalRenderPipelineAsset)
                return false;

            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                targetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

            if (data == null) return false;

            if (!data.renderPostProcessing) return false;

            return true;
        }
#else
        public void CaptureAndSaveImage()
        {
            StartSaveImage(SaveFolderPath, FileName, CaptureImage(targetCamera, ImageResolution, 8));
        }
#endif

        public Texture2D CaptureImage(Camera camera, Vector2Int resolution, int depth)
        {
            if (!CameraCheck(camera)) return null;
            RenderTexture cameraTexture = new RenderTexture(resolution.x, resolution.y, depth);

            camera.targetTexture = cameraTexture;
            camera.Render();

            if (RenderTexture.active != null) RenderTexture.active.Release();
            RenderTexture.active = cameraTexture;

            if (IsLogCap)
                Debug.Log("Camera rect = " + camera.pixelWidth + " - " + camera.pixelHeight
                          + "  Screen resolution = " + Screen.width + " - " + Screen.height);
            Texture2D texture2D = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);

            texture2D.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            RenderTexture lastTexture = RenderTexture.active;
            RenderTexture.active = null;
            lastTexture.Release();

            camera.targetTexture = null;

            return texture2D;
        }

        public void StartSaveImage(string folderPath, string fileName, Texture2D texture)
        {
            if (!FolderPathCheck(folderPath)) return;
            // fileName = UpdateFileName(fileName);
            if (!FileNameCheck(fileName)) return;

            byte[] saveData = null;
            switch (ImageFormat)
            {
                //case ImageFormat.exr:
                //    saveData = texture.EncodeToEXR();
                //    break;
                case ImageFormat.jpg:
                    saveData = texture.EncodeToJPG();
                    break;

                case ImageFormat.png:
                    saveData = texture.EncodeToPNG();
                    break;

                case ImageFormat.tga:
                    saveData = texture.EncodeToTGA();
                    break;
            }

            DestroyImmediate(texture);

            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning(string.Format("Folder path {0} do not exist"));
                Directory.CreateDirectory(folderPath);
            }

            string fullpath;
            if (IsImageSerial)
            {
                FileInfor fileInfor = new FileInfor(folderPath, fileName, ImageFormat);
                if (!fileInfors.ContainsKey(fileInfor)) fileInfors[fileInfor] = 0;
                fullpath = Path.Combine(folderPath,
                    fileName + SerialSeparator + fileInfors[fileInfor].ToString() + '.' + ImageFormat.ToString());
                fileInfors[fileInfor] = fileInfors[fileInfor] + 1;
            }
            else
                fullpath = Path.Combine(folderPath, fileName + '.' + ImageFormat.ToString());

            switch (WriteType)
            {
                case WriteFileType.MainThread:
                    DataWriter.WriteDataMain(fullpath, saveData);
                    break;

                case WriteFileType.Async:
                    DataWriter.WriteDataTask(fullpath, saveData);
                    break;
                //case WriteFileType.JobSystem:
                //    DataSaver.WriteDataJobS(fullpath, saveData);
                //    break;
            }

            if (IsLogCap) Debug.Log("Capture image save to " + fullpath);
        }

        private bool CameraCheck(Camera camera)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return false;
            }

            return true;
        }

        private bool FolderPathCheck(string path)
        {
            if (path == "" || path == null)
            {
                Debug.LogWarning("Savepath is null or empty");
                return false;
            }

            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Savepath do not exist");
                return false;
            }

            return true;
        }

        private bool FileNameCheck(string fileName)
        {
            if (fileName == "" || fileName == null)
            {
                Debug.LogWarning("fileName is null or empty");
                return false;
            }

            return true;
        }
    }
    */
}