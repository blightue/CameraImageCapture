using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using SuiSuiShou.CIC.Core;

namespace SuiSuiShou.CIC.Infor
{
    public static class CaptureInforManager
    {
        private static string foldPath = Path.Combine(Application.persistentDataPath, "CameraCapture");
        private static string inforPath = Path.Combine(foldPath, "CameraCaptureInfor.json");

        public static void WriteLocalData(Dictionary<FileInfor, int> fileInfors)
        {
            Queue<FileInfor> fileInforQueue = new Queue<FileInfor>();
            Queue<int> fileCountQueue = new Queue<int>();
            foreach (var item in fileInfors)
            {
                fileInforQueue.Enqueue(item.Key);
                fileCountQueue.Enqueue(item.Value);
            }

            if (!Directory.Exists(foldPath)) Directory.CreateDirectory(foldPath);
            if (!File.Exists(inforPath)) File.Create(inforPath);
            CaptureInfor captureInfor = new CaptureInfor();

            captureInfor.fileInfors = fileInforQueue.ToArray();
            captureInfor.fileCount = fileCountQueue.ToArray();

            string jsonForWrite = JsonUtility.ToJson(captureInfor, true);

            using (StreamWriter streamWrite = new StreamWriter(inforPath))
            {
                streamWrite.Write(jsonForWrite);
            }

            // Debug.Log($"Write Capture infor at {inforPath}");
        }

        public static Dictionary<FileInfor, int> ReadLocalData()
        {
            //Debug.Log($"Start read Capture infor at {inforPath}");


            Dictionary<FileInfor, int> fileInforDic = new Dictionary<FileInfor, int>();

            if (!File.Exists(inforPath)) return fileInforDic;

            string inforJson;

            using (StreamReader streamReader = new StreamReader(inforPath))
            {
                inforJson = streamReader.ReadToEnd();
            }

            CaptureInfor captureInfor = JsonUtility.FromJson<CaptureInfor>(inforJson);

            if (captureInfor == null ||
                captureInfor.fileCount == null ||
                captureInfor.fileInfors == null ||
                captureInfor.fileCount.Length == 0 ||
                captureInfor.fileInfors.Length == 0
               )
            {
                Debug.Log("CaptureInfor is empty");
                return fileInforDic;
            }

            for (int i = 0; i < captureInfor.fileInfors.Length; i++)
            {
                int checkedFileCount = 0;
                if (!CheckIsDiskFileCountUnchanged(captureInfor.fileInfors[i], captureInfor.fileCount[i],
                        out checkedFileCount))
                    captureInfor.fileCount[i] = checkedFileCount;
            }


            for (int i = 0; i < captureInfor.fileInfors.Length; i++)
            {
                fileInforDic[captureInfor.fileInfors[i]] = captureInfor.fileCount[i];
            }

            return fileInforDic;
        }

        private static bool CheckIsDiskFileCountUnchanged(FileInfor infor, int loggedCount, out int changedCount)
        {
            string[] filePath = null;

            filePath = Directory.GetFiles(infor.folderPath, $"{infor.fileName}-*.{infor.imageFormat.ToString()}",
                SearchOption.TopDirectoryOnly);

            if (filePath.Length == 0)
            {
                Debug.Log($"Empty {infor.folderPath} - {infor.fileName}");
                changedCount = 0;
                return false;
            }

            // if (filePath.Length == loggedCount )
            // {
            //     changedCount = loggedCount;
            //     Debug.Log("Unchanged");
            //     return true;
            // }
            int[] indexes = IndexesFromFilePathes(filePath, infor);
            // Debug.Log(indexes[indexes.Length - 1]);
            int lastIndex = indexes[indexes.Length - 1];

            string debugText = "";

            foreach (var VARIABLE in filePath)
            {
                debugText += VARIABLE + "-" + IndexFromFilePath(VARIABLE, infor) +
                             System.Environment.NewLine;
            }

            Debug.Log(debugText + filePath.Length);
            if (lastIndex == loggedCount - 1)
            {
                changedCount = loggedCount;
                // Debug.Log("Unchanged");
                return true;
            }
            else
            {
                changedCount = lastIndex + 1;
                // Debug.Log("Changed to " + changedCount);
                return false;
            }
        }

        private static int[] IndexesFromFilePathes(string[] filePathes, FileInfor infor)
        {
            Queue<int> indexes = new Queue<int>();

            foreach (var path in filePathes)
            {
                indexes.Enqueue(IndexFromFilePath(path, infor));
            }

            int[] indexArray = indexes.ToArray();
            Array.Sort(indexArray);

            return indexArray;
        }
        
        private static int IndexFromFilePath(string filePath, FileInfor infor)
        {
            string[] fileParts = filePath.Split(CameraImageCaptureBase.SerialSeparator);

            string fileTail = fileParts[fileParts.Length - 1];
            //Debug.Log(fileTail);
            string indexString = fileTail.Substring(0, fileTail.Length - infor.imageFormat.ToString().Length - 1);
            int index = -1;
            int.TryParse(indexString, out index);
            return index;
        }
    }


    [System.Serializable]
    public class CaptureInfor
    {
        public FileInfor[] fileInfors;
        public int[] fileCount;


        public CaptureInfor(FileInfor[] fileInfors, int[] fileCount)
        {
            this.fileInfors = fileInfors;
            this.fileCount = fileCount;
        }

        public CaptureInfor()
        {
        }
    }

    [System.Serializable]
    public struct FileInfor
    {
        public string folderPath;
        public string fileName;
        public ImageFormat imageFormat;

        public FileInfor(string folderPath, string fileName, ImageFormat imageFormat)
        {
            this.folderPath = folderPath;
            this.fileName = fileName;
            this.imageFormat = imageFormat;
        }
    }
}