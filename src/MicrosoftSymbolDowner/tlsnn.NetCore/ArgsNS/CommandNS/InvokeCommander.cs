using System;
using System.Collections.Generic;
using System.Linq;

namespace tlsnn.NetCore.ArgsNS.CommandNS
{
    public class InvokeCommander
    {
        private List<Command> Commands = new List<Command>();
        /// 添加命令
        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }
        //添加一组命令
        public void AddCommand(Command[] command)
        {
            Commands.AddRange(command);
        }
        // 移除命令
        public void CancelCommand(Command command)
        {
            Commands.Remove(command);
        }

        //移除全部
        public void CancelCommand()
        {
            Commands.Clear();
        }

        /// 执行命令
        public void ExecCommand()
        {
            if (Commands.Count == 0)
            {
                throw new Exception("请输入命令");
            }
            else if (Commands.Count == 1)
            {
                Commands[0].Execute();
            }
            else
            {
                if (Commands.Exists(a => a.IsOnly == true))
                {
                    throw new Exception("此命令单独存在");
                }
                else
                {
                    List<Command> tmpCommands = Commands.OrderBy(a => a.Order).ToList();
                    foreach (Command command in tmpCommands)
                    {
                        command.Execute();
                    }
                }
            }
        }
    }
}
