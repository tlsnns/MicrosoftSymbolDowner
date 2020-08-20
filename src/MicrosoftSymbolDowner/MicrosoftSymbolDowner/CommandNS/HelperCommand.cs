using System;
using System.Collections.Generic;
using System.Text;
using tlsnn.NetCore.ArgsNS;
using tlsnn.NetCore.ArgsNS.CommandNS;

namespace MicrosoftSymbolDowner.CommandNS
{
    class HelperCommand : Command
    {
        public HelperCommand(StartInfo startInfo, string commandName) : base(startInfo, commandName)
        {
            IsOnly = true;
            Order = CommandPriorityType.First;
        }
        public override void Execute()
        {
            string str =@"
--h   显示帮助信息
--cm    创建清单文件
--dp    下载pdb文件
-im     输入清单文件
-if     输入pe文件
-id     输入pe目录
-idr    输入pe目录，及其子目录
-om     输出清单文件
-od     输出pdb文件夹
-ss     符号服务器
-sps    socks5代理地址
-spp    socks5代理端口
";
            Console.WriteLine(str);
        }
    }
}
