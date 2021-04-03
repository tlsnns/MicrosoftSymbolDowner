using System.Text;
using System.IO;

namespace tlsnn.NetCore.Logs
{
    class FileLogProvider : LogProvider
    {
        StreamWriter StreamWriter;

        public FileLogProvider(string savePath) : base()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));
            StreamWriter = new StreamWriter(savePath, true, Encoding.UTF8);
        }
        protected override void CClose()
        {
            StreamWriter.Close();
        }
        protected override void SaveWork(string strInfo)
        {
            StreamWriter.WriteLine(strInfo);
        }
    }
}
