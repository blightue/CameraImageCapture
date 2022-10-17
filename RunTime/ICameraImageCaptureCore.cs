using System.IO;
using System.Collections.Generic;
using UnityEngine;
using SuiSuiShou.CIC.Data;
using SuiSuiShou.CIC.Infor;
using System.Threading.Tasks;

namespace SuiSuiShou.CIC.Core
{
    [System.Serializable]
    public enum ImageFormat
    {
        jpg,
        png,
        tga
    };

    public interface ICameraImageCaptureCore
    {
        /// <summary>
        /// Capture image resolution
        /// </summary>
        public abstract Vector2Int ImageResolution { get; set; }
        /// <summary>
        /// FileInfor for log image sequence
        /// </summary>
        public abstract Dictionary<FileInfor, int> FileInfors { get; set; }
        /// <summary>
        /// Target camera for capturing
        /// </summary>
        public abstract Camera TargetCamera { get; set; }

        /// <summary>
        /// Image file write type
        /// </summary>
        public abstract WriteFileType WriteType { get; set; }
        /// <summary>
        /// Image type
        /// </summary>
        public abstract ImageFormat ImageFormat { get; set; }
        
        /// <summary>
        /// True for logging capture status
        /// </summary>
        public abstract bool IsLogCap { get; set; }
        /// <summary>
        /// True for sequential image file name
        /// </summary>
        public abstract bool IsImageSerial { get; set; }
        /// <summary>
        /// True for changing ImageResolution in editor
        /// </summary>
        public abstract bool IsOverrideCameraResolution { get; set; }
        
        /// <summary>
        /// Fold path for image file to writing
        /// </summary>
        public abstract string SaveFolderPath { get; set; }
        /// <summary>
        /// Image file name
        /// </summary>
        public abstract string FileName { get; set; }
    }

    public static class CameraImageCaptureCoreMethod
    {
#if UNITY_ENABLE_URP
        /// <summary>
        /// Capture and save image file base on instance config
        /// </summary>
        /// <param name="C"></param>
        public static void CaptureAndSaveImage(this ICameraImageCaptureCore C)
        {
            if (C.CheckIsNeedFixedAlpha())
            {
                UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                    C.TargetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                data.renderPostProcessing = false;
                Texture2D texWithAlpha = C.CaptureImage(C.TargetCamera, C.ImageResolution, 8);
                data.renderPostProcessing = true;
                Texture2D texWithoutAlpha = C.CaptureImage(C.TargetCamera, C.ImageResolution, 8);

                Color[] piexlsWithAlpha = texWithAlpha.GetPixels();
                Color[] piexlsWithoutAlpha = texWithoutAlpha.GetPixels();
                // for (int i = 0; i < piexlsWithoutAlpha.Length; i++)
                // {
                //     piexlsWithoutAlpha[i].a = piexlsWithAlpha[i].a;
                // }
                Parallel.For(0, piexlsWithAlpha.Length, i => { piexlsWithoutAlpha[i].a = piexlsWithAlpha[i].a; });

                texWithoutAlpha.SetPixels(piexlsWithoutAlpha);

                Object.DestroyImmediate(texWithAlpha);

                C.StartSaveImage(C.SaveFolderPath, C.FileName, texWithoutAlpha);
            }
            else
            {
                C.StartSaveImage(C.SaveFolderPath, C.FileName, C.CaptureImage(C.TargetCamera, C.ImageResolution, 8));
            }
        }

        private static bool CheckIsNeedFixedAlpha(this ICameraImageCaptureCore C)
        {
            if (!UnityEngine.Rendering.GraphicsSettings.defaultRenderPipeline is UnityEngine.Rendering.Universal
                    .UniversalRenderPipelineAsset)
                return false;

            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData data =
                C.TargetCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

            if (data == null) return false;

            if (!data.renderPostProcessing) return false;

            if (!(C.TargetCamera.clearFlags == CameraClearFlags.Nothing ||
                  C.TargetCamera.clearFlags == CameraClearFlags.SolidColor && C.TargetCamera.backgroundColor.a == 0))
                return false;

            return true;
        }
#else
        /// <summary>
        /// Capture and save image file base on instance config
        /// </summary>
        /// <param name="C"></param>
        public static void CaptureAndSaveImage(this ICameraImageCaptureCore C)
        {
            C.StartSaveImage(C.SaveFolderPath, C.FileName, C.CaptureImage(C.TargetCamera, C.ImageResolution, 8));
        }
#endif

        /// <summary>
        /// Capture and return a Texture2D
        /// </summary>
        /// <param name="C"></param>
        /// <param name="camera">target camera</param>
        /// <param name="resolution">image resolution</param>
        /// <param name="depth">image depth</param>
        /// <returns></returns>
        public static Texture2D CaptureImage(this ICameraImageCaptureCore C, Camera camera, Vector2Int resolution,
            int depth)
        {
            if (!C.CameraCheck(camera)) return null;
            RenderTexture cameraTexture = new RenderTexture(resolution.x, resolution.y, depth);

            camera.targetTexture = cameraTexture;
            camera.Render();

            if (RenderTexture.active != null) RenderTexture.active.Release();
            RenderTexture.active = cameraTexture;

            if (C.IsLogCap)
                // Debug.Log("Camera rect = " + camera.pixelWidth + " - " + camera.pixelHeight
                //           + "  Screen resolution = " + Screen.width + " - " + Screen.height);
                Debug.Log($"Capture camera with resolution = {resolution}");
            Texture2D texture2D = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);

            texture2D.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            RenderTexture lastTexture = RenderTexture.active;
            RenderTexture.active = null;
            lastTexture.Release();

            camera.targetTexture = null;

            return texture2D;
        }

        /// <summary>
        /// Start write image file base on instance config
        /// </summary>
        /// <param name="C"></param>
        /// <param name="folderPath">target folder path</param>
        /// <param name="fileName">target file name</param>
        /// <param name="texture">source Texture2D</param>
        public static void StartSaveImage(this ICameraImageCaptureCore C, string folderPath, string fileName,
            Texture2D texture)
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
                    DataWriter.WriteDataAsync(fullpath, saveData);
                    break;
            }

            if (C.IsLogCap) Debug.Log("Capture image save to " + fullpath);
        }

        private static bool CameraCheck(this ICameraImageCaptureCore C, Camera camera)
        {
            if (camera == null)
            {
                Debug.LogWarning("Camera is null");
                return false;
            }

            return true;
        }

        private static bool FolderPathCheck(this ICameraImageCaptureCore C, string path)
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

        private static bool FileNameCheck(this ICameraImageCaptureCore C, string fileName)
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
