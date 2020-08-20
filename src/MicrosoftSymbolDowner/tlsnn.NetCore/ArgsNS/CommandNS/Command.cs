

namespace tlsnn.NetCore.ArgsNS.CommandNS
{
    public abstract class Command
    {
        protected StartInfo StartInfo { get; set; }
        // Constructors 
        protected Command(StartInfo startInfo, string commandName)
        {
            StartInfo = startInfo;
            CommandName = commandName;
        }
        public string CommandName { get; set; }
        public bool IsOnly { get; set; }
        public CommandPriorityType Order { get; set; }
        // 命令执行方法
        public abstract void Execute();
    }
}
