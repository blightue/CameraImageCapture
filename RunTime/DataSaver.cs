using System.Collections;
using System.IO;
using System.Threading;

namespace CIC.DataSaver
{

    public static class DataSaver
    {
        public static void SaveData(string fullPath, byte[] data)
        {
            File.WriteAllBytes(fullPath, data);
        }
        public static void SaveDataThreat(string fullPath, byte[] data)
        {
            SaveJob job = new SaveJob(fullPath, data);
            Thread thread = new Thread(new ThreadStart(job.Excute));
            thread.Start();
        }


    }
    public class SaveJob
    {
        public string fullPath;
        public byte[] data;

        public SaveJob(string fullPath, byte[] data)
        {
            this.fullPath = fullPath;
            this.data = data;
        }

        public void Excute()
        {
            File.WriteAllBytes(fullPath, data);
        }
    }
}