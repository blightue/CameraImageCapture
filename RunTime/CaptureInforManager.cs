using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class CaptureInforManager
{

    private static string foldPath = Path.Combine(Application.persistentDataPath, "CameraCapture");
    private static string inforPath = Path.Combine(foldPath, "CameraCaptureInfor.json");

    //public static CaptureInfor captureInfor;

    public static async void WriteLocalData(Dictionary<string, FileInfor> fileInfors)
    {
        Queue<string> fileNameQueue = new Queue<string>();
        Queue<FileInfor> fileInforQueue = new Queue<FileInfor>();
        foreach (var item in fileInfors)
        {
            fileNameQueue.Enqueue(item.Key);
            fileInforQueue.Enqueue(item.Value);
        }
        
        if (!Directory.Exists(foldPath)) Directory.CreateDirectory(foldPath);
        if (!File.Exists(inforPath)) File.Create(inforPath);
        CaptureInfor captureInfor = new CaptureInfor();

        captureInfor.fileNames = fileNameQueue.ToArray();
        captureInfor.fileInfors = fileInforQueue.ToArray();

        string jsonForWrite = JsonUtility.ToJson(captureInfor);

        using (StreamWriter streamWrite = new StreamWriter(inforPath))
        {
            await streamWrite.WriteAsync(jsonForWrite);
        }

        //Debug.Log($"Write Capture infor at {inforPath}");
    }
    public static Dictionary<string, FileInfor> ReadLocalData()
    {
        //Debug.Log($"Start read Capture infor at {inforPath}");


        Dictionary<string, FileInfor> fileInforDic = new Dictionary<string, FileInfor>();

        if (!File.Exists(inforPath)) return new Dictionary<string, FileInfor>();

        string inforJson;

        using (StreamReader streamReader = new StreamReader(inforPath))
        {
            inforJson = streamReader.ReadToEnd();
        }

        CaptureInfor captureInfor = JsonUtility.FromJson<CaptureInfor>(inforJson);

        if (captureInfor == null ||
            captureInfor.fileNames == null ||
            captureInfor.fileInfors == null ||
            captureInfor.fileNames.Length == 0 ||
            captureInfor.fileInfors.Length == 0
            )
        {
            Debug.Log("CaptureInfor is empty");
            return new Dictionary<string, FileInfor>();
        }

        for (int i = 0; i < captureInfor.fileNames.Length; i++)
        {
            fileInforDic[captureInfor.fileNames[i]] = captureInfor.fileInfors[i];
        }
        return fileInforDic;
    }
}


[System.Serializable]
public class CaptureInfor
{
    public string[] fileNames;
    public FileInfor[] fileInfors;

    public CaptureInfor()
    {
        this.fileNames = new string[0];
        this.fileInfors = new FileInfor[0];
    }
}

[System.Serializable]
public class FileInfor
{
    public string folderPath;
    public int fileCount;

    public FileInfor(string folderPath, int fileCount)
    {
        this.folderPath = folderPath;
        this.fileCount = fileCount;
    }
}
