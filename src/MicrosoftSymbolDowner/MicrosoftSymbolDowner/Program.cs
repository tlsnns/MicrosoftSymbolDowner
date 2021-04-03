using System;
using tlsnn.NetCore.ArgsNS.ArgsNS;
using tlsnn.NetCore.ArgsNS.CommandNS;

namespace MicrosoftSymbolDowner
{
    class Program
    {
        static void Main(string[] args)
        {
            
                InvokeCommander invokeCommander = new InvokeCommander();
                StartInfo1 startInfo1 = new StartInfo1();
                var ar = new ArgRouter(invokeCommander, startInfo1, ArgActionsFactory.CreateActions());
                ArgsParser.Parse(args, ar);
                invokeCommander.ExecCommand();
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
