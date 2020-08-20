using tlsnn.NetCore.ArgsNS;
using tlsnn.NetCore.ArgsNS.CommandNS;
using System;
using System.Threading.Tasks;
using System.IO;

namespace MicrosoftSymbolDowner.CommandNS
{
    class CreateManifestListCommand : Command
    {
        public CreateManifestListCommand(StartInfo startInfo, string commandName) : base(startInfo, commandName)
        {
            IsOnly = true;
            Order = CommandPriorityType.First;
        }

        public override void Execute()
        {
            var s1 = StartInfo as StartInfo1;
            if (string.IsNullOrWhiteSpace(s1.OutputManifestFile))
            {
                Console.WriteLine("没有指定输出清单路径");
                return;
            }
            if (s1.InputPEFilePaths.Count == 0)
            {
                Console.WriteLine("没有指定输入pe");
                return;
            }
            s1.InputPEFilePaths.ForEach(item =>
            {
                if (!ManifestTask.TryInsertTask(item, out string strErrorMessage))
                {
                    Console.WriteLine(strErrorMessage);
                }
            });
            ManifestTask.WriteToManifestFile(s1.OutputManifestFile);
        }
    }
}
