using System.IO;
using System.Threading;
using System.Text;
using Unity.Jobs;
using Unity.Collections;

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
            //SaveJob job = new SaveJob(fullPath, data);
            //Thread thread = new Thread(new ThreadStart(job.ExcuteBytes));
            //thread.Start();
            WriteByteJob job = new WriteByteJob(fullPath, data);

        }
        public static void SaveDataThreat(string fullPath, string data)
        {
            //SaveJob job = new SaveJob(fullPath, data);
            //Thread thread = new Thread(new ThreadStart(job.ExcuteText));
            //thread.Start();

            WriteTextJob job = new WriteTextJob(fullPath, data);
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

    public class WriteByteJob
    {
        public string fullPath;
        public byte[] data;

        public WriteByteJob(string fullPath, byte[] data)
        {
            this.fullPath = fullPath;
            this.data = data;
            Execute();
        }

        private void Execute()
        {
            NativeArray<byte> nativeData = new NativeArray<byte>(data, Allocator.TempJob);
            NativeArray<char> nativePath = new NativeArray<char>(fullPath.ToCharArray(), Allocator.TempJob);

            WriteJobs jobs = new WriteJobs() { fileData = nativeData, fileFullPath = nativePath };

            JobHandle jobHandle = jobs.Schedule();

            jobHandle.Complete();

            nativeData.Dispose();
            nativePath.Dispose();
        }
        private struct WriteJobs : IJob
        {
            public NativeArray<byte> fileData;
            public NativeArray<char> fileFullPath;

            public void Execute()
            {
                File.WriteAllBytes(new string(fileFullPath.ToArray()), fileData.ToArray());
            }
        }

    }

    public class WriteTextJob
    {
        public string fullPath;
        public string data;

        public WriteTextJob(string fullPath, string data)
        {
            this.fullPath = fullPath;
            this.data = data;
            Execute();
        }

        private void Execute()
        {
            NativeArray<char> nativeData = new NativeArray<char>(data.ToCharArray(), Allocator.TempJob);
            NativeArray<char> nativePath = new NativeArray<char>(fullPath.ToCharArray(), Allocator.TempJob);

            WriteJobs jobs = new WriteJobs() { fileData = nativeData, fileFullPath = nativePath };

            JobHandle jobHandle = jobs.Schedule();

            jobHandle.Complete();

            nativeData.Dispose();
            nativePath.Dispose();
        }
        private struct WriteJobs : IJob
        {
            public NativeArray<char> fileData;
            public NativeArray<char> fileFullPath;

            public void Execute()
            {
                File.WriteAllText(new string(fileFullPath.ToArray()), new string(fileData.ToArray()));
            }
        }

    }
}