using System;
using System.Collections.Generic;
using tlsnn.NetCore.ArgsNS.CommandNS;

namespace tlsnn.NetCore.ArgsNS.ArgsNS
{
    public class ArgRouter
    {
        InvokeCommander InvokeCommander { get; set; }
        StartInfo StartInfo { get; set; }

        Dictionary<string, Action<string[], InvokeCommander, StartInfo>> Actions;
        public ArgRouter(InvokeCommander invokeCommander, StartInfo startInfo, Dictionary<string, Action<string[], InvokeCommander, StartInfo>> actions)
        {
            InvokeCommander = invokeCommander;
            StartInfo = startInfo;
            Actions = actions;
        }
        public void Route(string strOption, string[] strDatas)
        {
            try
            {
                var item = Actions[strOption];
                item.Invoke(strDatas, InvokeCommander, StartInfo);
            }
            catch (KeyNotFoundException)
            {
                throw new Exception("未知的选项或命令");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void InsertAction(string strKey, Action<string[], InvokeCommander, StartInfo> action)
        {
            Actions.Add(strKey, action);
        }
    }
}