using System.IO;
using System.Threading;

namespace CIC.Data
{

    public static class DataSaver
    {
        public static void SaveData(string fullPath, byte[] data)
        {
            File.WriteAllBytes(fullPath, data);
        }
        public static void SaveData(string fullPath, string data)
        {
            File.WriteAllText(fullPath, data);
        }
        public static void SaveDataThreat(string fullPath, byte[] data)
        {
            SaveJob job = new SaveJob(fullPath, data);
            Thread thread = new Thread(new ThreadStart(job.ExcuteBytes));
            thread.Start();
        }
        public static void SaveDataThreat(string fullPath, string data)
        {
            SaveJob job = new SaveJob(fullPath, data);
            Thread thread = new Thread(new ThreadStart(job.ExcuteText));
            thread.Start();
        }


    }
    public class SaveJob
    {
        public string fullPath;
        public byte[] byteData;
        public string textData;

        public SaveJob(string fullPath, byte[] data)
        {
            this.fullPath = fullPath;
            this.byteData = data;
        }

        public SaveJob(string fullPath, string textData)
        {
            this.fullPath = fullPath;
            this.textData = textData;
        }

        public void ExcuteBytes()
        {
            File.WriteAllBytes(fullPath, byteData);
        }
        public void ExcuteText()
        {
            File.WriteAllText(fullPath, textData);
        }
    }
}