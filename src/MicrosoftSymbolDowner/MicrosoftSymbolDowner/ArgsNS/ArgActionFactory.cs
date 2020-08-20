using tlsnn.NetCore.ArgsNS;
using tlsnn.NetCore.ArgsNS.CommandNS;
using tlsnn.NetCore.IO;
using MicrosoftSymbolDowner.CommandNS;
using System;
using System.Collections.Generic;
using System.IO;

namespace MicrosoftSymbolDowner
{
    class ArgActionsFactory
    {
        static public Dictionary<string, Action<string[], InvokeCommander, StartInfo>> CreateActions()
        {
            Dictionary<string, Action<string[], InvokeCommander, StartInfo>> keyValuePairs = new Dictionary<string, Action<string[], InvokeCommander, StartInfo>>(StringComparer.OrdinalIgnoreCase);
            //帮助命令
            keyValuePairs.Add("--h", (data, i, s) =>
            {
                i.AddCommand(new HelperCommand(s, "--h"));
            });
            //创建清单文件
            keyValuePairs.Add("--cm", (data, i, s) =>
            {
                i.AddCommand(new CreateManifestListCommand(s, "--cm"));
            });
            //下载pdb文件
            keyValuePairs.Add("--dp", (data, i, s) =>
            {
                Command command = new DownloadPDBCommand(s, "--dp");
                i.AddCommand(command);
            });
            //输入清单文件
            keyValuePairs.Add("-im", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.InputManifestFile = data[0];
            });
            //输入pe文件
            keyValuePairs.Add("-if", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                foreach (string filePath in data)
                {
                    var s1 = s as StartInfo1;
                    s1.InputPEFilePaths.Add(filePath);
                }
            });
            //输入pe目录
            keyValuePairs.Add("-id", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                foreach (string fileDir in data)
                {
                    if (Directory.Exists(fileDir))
                    {
                        var s1 = s as StartInfo1;
                        s1.InputPEFilePaths.AddRange(IOUtils.GetAllFilesFromFolder(fileDir, false, "*.exe", "*.dll", "*.sys"));
                    }
                    else
                    {
                        throw new Exception("指定的目录不存在");
                    }
                }
            });
            keyValuePairs.Add("-idr", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                foreach (string fileDir in data)
                {
                    if (Directory.Exists(fileDir))
                    {
                        var s1 = s as StartInfo1;
                        s1.InputPEFilePaths.AddRange(IOUtils.GetAllFilesFromFolder(fileDir, true, "*.exe", "*.dll", "*.sys"));
                    }
                    else
                    {
                        throw new Exception("指定的目录不存在");
                    }
                }
            });
            keyValuePairs.Add("-om", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.OutputManifestFile = data[0];
            });
            keyValuePairs.Add("-od", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.OutputPDBFolder = data[0];
            });
            keyValuePairs.Add("-ss", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.SymbolServer = data[0];
            });
            keyValuePairs.Add("-sps", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.Socks5Server = data[0];
            });
            keyValuePairs.Add("-spp", (data, i, s) =>
            {
                if (data.Length < 1)
                {
                    throw new Exception("非预期的值");
                }
                var s1 = s as StartInfo1;
                s1.Socks5Port = int.Parse(data[0]);
            });
            return keyValuePairs;
        }
    }
}
