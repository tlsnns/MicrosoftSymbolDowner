using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace MicrosoftSymbolDowner
{
    static class ManifestTask
    {
        public static List<ManifestEntity> Tasks { get; } = new List<ManifestEntity>();

        public static bool TryInsertTask(string strFilePath, out string errorMessage)
        {
            if (!File.Exists(strFilePath))
            {
                errorMessage = " 没有找到输入文件" + strFilePath;
                return false;
            }

            FileStream fileStream = new FileStream(strFilePath, FileMode.Open, FileAccess.Read);

            PEReader peReader = new PEReader(fileStream);
            var debugDirectory = peReader.ReadDebugDirectory();

            CodeViewDebugDirectoryData? codeViewDebugDirectoryData = null;
            foreach (var item in debugDirectory)
            {
                if (item.Type == DebugDirectoryEntryType.CodeView)
                {
                    codeViewDebugDirectoryData = peReader.ReadCodeViewDebugDirectoryData(item);
                }
            }
            if (!codeViewDebugDirectoryData.HasValue)
            {
                errorMessage = "没有找到调试数据" + strFilePath;
                return false;
            }
            string strPDBName;
            var DirectorySeparatorCharLastIndex = codeViewDebugDirectoryData.Value.Path.LastIndexOf(Path.DirectorySeparatorChar);
            if (DirectorySeparatorCharLastIndex == -1)
            {
                strPDBName = codeViewDebugDirectoryData.Value.Path;
            }
            else
            {
                strPDBName = codeViewDebugDirectoryData.Value.Path.Substring(DirectorySeparatorCharLastIndex + 1);
            }
            string strGuid = codeViewDebugDirectoryData.Value.Guid.ToString("N");
            string strAge = codeViewDebugDirectoryData.Value.Age.ToString();
            Tasks.Add(new ManifestEntity { PDBName = strPDBName, PDBGuid = strGuid, Age = strAge });
            errorMessage = null;
            return true;
        }
        public static void WriteToManifestFile(string strFilePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(strFilePath));
            StreamWriter streamWriter = new StreamWriter(strFilePath, true);
            Tasks.ForEach(item =>
            {
                streamWriter.WriteLine($"{item.PDBName}\t{item.PDBGuid}\t{item.Age}");
            });
            streamWriter.Close();
        }
        public static void ReadFromManifestFile(string strFilePath)
        {
            var tmp = File.ReadAllLines(strFilePath);
            for (int i = 0; i < tmp.Length; i++)
            {
                var subTmp = tmp[0].Split('\t');
                Tasks.Add(new ManifestEntity { PDBName = subTmp[0], PDBGuid = subTmp[1], Age = subTmp[2] });
            }
        }
    }
}
