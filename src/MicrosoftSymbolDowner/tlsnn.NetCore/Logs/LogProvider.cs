using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tlsnn.NetCore.Logs
{
    abstract class LogProvider
    {
        volatile bool RunFlag;
        protected ConcurrentQueue<string> FlushSignal;
        public LogProvider()
        {
            RunFlag = true;
            FlushSignal = new ConcurrentQueue<string>();
            Thread thread = new Thread(StartSave);
            thread.Start();
        }

        public void SaveLog(string strInfo)
        {
            FlushSignal.Enqueue(strInfo);
        }
        private void StartSave()
        {
            while (true)
            {
                if (FlushSignal.TryDequeue(out string strInfo))
                {
                    SaveWork(strInfo);
                }
                else
                {
                    if (RunFlag)
                    {
                        Thread.Sleep(500);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            CClose();
        }

        public void Close()
        {
            RunFlag = false;
        }

        protected abstract void SaveWork(string strInfo);
        abstract protected void CClose();
    }
}
