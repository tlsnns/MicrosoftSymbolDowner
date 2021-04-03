using tlsnn.NetCore.ArgsNS;
using tlsnn.NetCore.ArgsNS.CommandNS;
using MihaZupan;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using tlsnn.NetCore.ConsoleUnit;
using System.Threading;
using System.Collections.Concurrent;

namespace MicrosoftSymbolDowner.CommandNS
{
    class DownloadPDBCommand : Command
    {
        public DownloadPDBCommand(StartInfo startInfo, string commandName) : base(startInfo, commandName)
        {
            IsOnly = true;
            Order = CommandPriorityType.First;
        }
        public override void Execute()
        {
            var s1 = StartInfo as StartInfo1;
            if (string.IsNullOrWhiteSpace(s1.OutputPDBFolder))
            {
                Console.WriteLine("没有指定输出pdb文件夹");
                return;
            }
            if (!string.IsNullOrWhiteSpace(s1.InputManifestFile))
            {
                if (!File.Exists(s1.InputManifestFile))
                {
                    Console.WriteLine("没有找到指定的清单文件");
                    return;
                }
                ManifestTask.ReadFromManifestFile(s1.InputManifestFile);
            }
            else if (s1.InputPEFilePaths.Count != 0)
            {
                s1.InputPEFilePaths.ForEach(item =>
                {
                    if (!ManifestTask.TryInsertTask(item, out string strErrorMessage))
                    {
                        Console.WriteLine(strErrorMessage);
                    }
                });
            }
            else
            {
                throw new Exception("没有指定输入pe或清单文件");
            }

            PDBDownLoader pdbDownLoader = new PDBDownLoader(s1.SymbolServer, s1.Socks5Server, s1.Socks5Port);

            ConcurrentQueue<PDBEntity> needRetryTask = new ConcurrentQueue<PDBEntity>();
            Parallel.ForEach(ManifestTask.Tasks, item =>
            {
                needRetryTask.Enqueue(new PDBEntity(s1.OutputPDBFolder, item));
            });


            pdbDownLoader.DownLoad(needRetryTask);
        }
    }
}
