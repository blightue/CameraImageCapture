using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CIC.Data;


namespace CIC.Core
{

    [AddComponentMenu("Camera Image Capture/CameraImageCapture")]
    public class CameraImageCapture : MonoBehaviour
    {
        public Camera targetCamera;
        public Vector2Int imageResolution = new Vector2Int(512, 512);

        public string saveFolderPath;
        public bool isUseThread = true;
        public bool isOverrideFile = false;

        public string fileName;

        private Dictionary<string, FileInfors> fileInfors = new Dictionary<string, FileInfors>();

        public void Reset()
        {
            targetCamera = Camera.main;
            saveFolderPath = Application.persistentDataPath;
            fileInfors = new Dictionary<string, FileInfors>();
            fileName = "cameraCaptures";
            Debug.Log("Reset CIC");
        }

        public void InitDic()
        {
            fileInfors = new Dictionary<string, FileInfors>();
        }

        public void CaptureAndSaveImage()
        {
            StartSaveImage(saveFolderPath, fileName, CaptureImage(targetCamera, imageResolution, 8));
        }

        public Texture2D CaptureImage(Camera camera, Vector2Int resolution, int depth)
        {
            if (!CameraCheck(camera)) return null;
            RenderTexture cameraTexture = new RenderTexture(resolution.x, resolution.y, depth);

            camera.targetTexture = cameraTexture;
            camera.Render();

            RenderTexture.active = cameraTexture;

            Debug.Log("Camera rect = " + camera.pixelWidth + " - " + camera.pixelHeight
                + "  Screen resolution = " + Screen.width + " - " + Screen.height);
            Texture2D texture2D = new Texture2D(resolution.x, resolution.y, TextureFormat.RGBA32, false);

            texture2D.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);

            RenderTexture.active = null;
            camera.targetTexture = null;

            return texture2D;
        }

        public void StartSaveImage(string folderPath, string fileName, Texture2D texture)
        {
            if (!FolderPathCheck(folderPath)) return;
            fileName = UpdateFileName(fileName);
            if (!FileNameCheck(fileName)) return;
            byte[] saveData = texture.EncodeToPNG();

            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning(string.Format("Folder path {0} do not exist"));
                return;
            }

            string fullpath = Path.Combine(folderPath, fileName + "-" + fileInfors[fileName].fileCount + ".png");
            fileInfors[fileName].fileCount++;

            if (isUseThread)
                DataSaver.SaveDataThreat(fullpath, saveData);
            else
                DataSaver.SaveData(fullpath, saveData);

            Debug.Log("Capture image save to " + fullpath);
        }

        private string UpdateFileName(string name)
        {
            if (!fileInfors.ContainsKey(name))
            {
                if (isOverrideFile)
                {
                    fileInfors[name] = new FileInfors(saveFolderPath, 0);
                    return name;
                }
                while
                    (
                    PlayerPrefs.HasKey("CICFileCount" + name) &&
                    File.Exists(Path.Combine(saveFolderPath, name + "-0.png"))
                    )
                {
                    name += "-New";
                }
                fileInfors[name] = new FileInfors(saveFolderPath, 0);
                //Debug.Log(fileInfors[name].fileCount + fileInfors.ContainsKey(name).ToString());
            }
            fileName = name;
            return name;
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

        private void OnApplicationQuit()
        {
            if (fileInfors.Count > 0)
            {
                foreach (var item in fileInfors)
                {
                    PlayerPrefs.SetInt("CICFileCount" + item.Key, item.Value.fileCount);
                    PlayerPrefs.SetString("CICFileFolder" + item.Key, item.Value.folderPath);
                }
                PlayerPrefs.Save();
            }
        }


        private class FileInfors
        {
            public string folderPath;
            public int fileCount;

            public FileInfors(string folderPath, int fileCount)
            {
                this.folderPath = folderPath;
                this.fileCount = fileCount;
            }
        }
    }
}