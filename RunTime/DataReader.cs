using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SuiSuiShou.CIC.Data
{
    public static class DataReader
    {
        public static string ReadDataTextMain(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public static string[] ReadDataAllLineMain(string filePath)
        {
            return File.ReadAllLines(filePath);
        }

        public static byte[] ReadDataBytesMain(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }


        public async static Task<string> ReadDataTextAsync(string filePath)
        {
            string result;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] bts = new byte[fs.Length];
                await fs.ReadAsync(bts, 0, bts.Length);
                result = Encoding.Unicode.GetString(bts);
            }

            return result;
        }


        public async static Task<byte[]> ReadDataBytesAsync(string filePath)
        {
            byte[] result;

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                result = new byte[fs.Length];
                await fs.ReadAsync(result, 0, result.Length);
            }

            return result;
        }
    }
}