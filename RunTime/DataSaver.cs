using System.IO;
//using Unity.Jobs;
//using Unity.Collections;

namespace SuiSuiShou.CIC.Data
{

    public static class DataSaver
    {
        public static void WriteDataMain(string fullPath, byte[] data)
        {
            File.WriteAllBytes(fullPath, data);
        }
        public static void WriteDataMain(string fullPath, string data)
        {
            File.WriteAllText(fullPath, data);
        }
        
        /*Job System
        public static void WriteDataJobS(string fullPath, byte[] data)
        {
            WriteByteJob job = new WriteByteJob(fullPath, data);
        }
        public static void WriteDataJobS(string fullPath, string data)
        {
            WriteTextJob job = new WriteTextJob(fullPath, data);
        }
        */
        
        public static void WriteDataTask(string fullPath, byte[] data)
        {
            WriteAsyncJob job = new WriteAsyncJob(fullPath, data);
        }
        public static void WriteDataTask(string fullPath, string data)
        {
            WriteAsyncJob job = new WriteAsyncJob(fullPath, data);
        }


    }


    public class WriteInMainJob
    {
        public string fullPath;
        public byte[] byteData;
        public string textData;

        public WriteInMainJob(string fullPath, byte[] data)
        {
            this.fullPath = fullPath;
            this.byteData = data;
        }

        public WriteInMainJob(string fullPath, string textData)
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

    public class WriteAsyncJob
    {
        public string fullPath;
        public byte[] byteData;
        public string textData;

        public WriteAsyncJob(string fullPath, byte[] byteData)
        {
            this.fullPath = fullPath;
            this.byteData = byteData;
            ExcuteBytes();
        }

        public WriteAsyncJob(string fullPath, string textData)
        {
            this.fullPath = fullPath;
            this.textData = textData;
            ExcuteText();
        }

        private async void ExcuteBytes()
        {
            using (FileStream file = File.Open(fullPath, FileMode.OpenOrCreate))
            {
                file.Seek(0, SeekOrigin.End);
                await file.WriteAsync(byteData, 0, byteData.Length);
            }
        }
        private async void ExcuteText()
        {
            using (StreamWriter file = File.CreateText(fullPath))
            {
                await file.WriteAsync(textData);
            }
        }
    }


    /* Job system
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
            //Debug.Log(Thread.CurrentThread.Name);
            jobs.Schedule().Complete();
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
    */

    public enum WriteFileType
    {
        MainThread,
        Async
            //, JobSystem
    };
}