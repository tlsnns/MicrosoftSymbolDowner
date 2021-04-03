using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tlsnn.NetCore.Logs
{
    class Loger
    {
        LogLevel SaveLogLevel;
        LogProvider LogProvider;
        public Loger(LogProvider logProvider, LogLevel saveLogLevel = LogLevel.Error)
        {
            LogProvider = logProvider;
            SaveLogLevel = saveLogLevel;
        }

        public void LogDebug(string EventName, string info, int EventId = 0)
        {
            Log(EventName, EventId, LogLevel.Debug, info);
        }
        public void LogInfo(string EventName, string info, int EventId = 0)
        {
            Log(EventName, EventId, LogLevel.Info, info);
        }
        public void LogWarning(string EventName, string info, int EventId = 0)
        {
            Log(EventName, EventId, LogLevel.Warning, info);
        }
        public void LogError(string EventName, string info, int EventId = 0)
        {
            Log(EventName, EventId, LogLevel.Error, info);
        }

        void Log(string name, int Id, LogLevel logLevel, string info)
        {
            if (logLevel < SaveLogLevel)
            {
                return;
            }
            FormatLog(name, Id, logLevel, info);
        }

        void FormatLog(string name, int Id, LogLevel logLevel, string info)
        {
            LogProvider.SaveLog($"{logLevel}: {name}[{Id}] {info}");
        }
    }

    enum LogLevel : byte
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
    }
}
