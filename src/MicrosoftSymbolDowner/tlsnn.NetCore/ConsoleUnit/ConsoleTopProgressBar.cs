using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using tlsnn.NetCore.Logs;

namespace tlsnn.NetCore.ConsoleUnit
{
    public class ConsoleTopProgressBar
    {
        ConcurrentQueue<string> FlushSignal;

        //已经完成的项目数
        int HandleCount = 0;

        int Count;
        int Width;

        DateTime StartTime;

        LogProvider LogProvider;
        Loger Loger;
        string Subject;

        ManualResetEvent manualResetEvent;
        public ConsoleTopProgressBar(int count, int width = 50, string strSubject = "")
        {
            Count = count;
            Width = width;

            LogProvider = new FileLogProvider(Path.Combine("Logs", DateTime.Now.ToString("yyyy-MM-dd HH：mm：ss.ffff") + ".txt"));
            Loger = new Loger(LogProvider);

            Subject = strSubject;

            StartTime = DateTime.Now;
            Console.CursorVisible = false;
            Console.WriteLine();

            OriginalPoint = new FixedCoordinate();
            OriginalPoint.DispalyLine($"正在进行任务：{strSubject}...任务总数：{Count}\t开始时间：{StartTime}");

            ProgressBarPoint = new FixedCoordinate();
            Console.WriteLine();
            SpeedPoint = new FixedCoordinate();
            Console.WriteLine();
            RemainderNumberPoint = new FixedCoordinate();
            Console.WriteLine();
            RemainderTimePoint = new FixedCoordinate();
            Console.WriteLine();
            EndPoint = new FixedCoordinate();
            Console.WriteLine();
            Console.WriteLine();
            InfoPoint = new FixedCoordinate();

            manualResetEvent = new ManualResetEvent(false);
            FlushSignal = new ConcurrentQueue<string>();
            Thread thread = new Thread(DispalyWorker);
            thread.Start();
        }

        private void DispalyWorker()
        {
            DateTime lastRefreshDateTime = StartTime;
            while (HandleCount < Count)
            {
                if (FlushSignal.TryDequeue(out string strInfo))
                {
                    HandleCount++;
                    //进度条相关
                    double dValue = HandleCount * Width / Count;
                    int iValue = (int)Math.Round(dValue);
                    int SpaceValue = Width - iValue;
                    var ratio = HandleCount * 1.0 / Count;
                    string strProgressBarMessage = $"[{new string('*', iValue) + new string(' ', SpaceValue)}]\t{ratio:p}";
                    //速度相关
                    var n = DateTime.Now;
                    var speed = 1 / (n - lastRefreshDateTime).TotalSeconds;//速度
                    lastRefreshDateTime = n;
                    string strspeedMessage = $"当前速度：{speed:f}个/s";
                    //剩余个数相关
                    int remainderNumber = Count - HandleCount;
                    string strRemainderNumberMessage = $"剩余项目：{remainderNumber}";
                    //剩余时间相关
                    var dRemainderSeconds = remainderNumber / speed;
                    var iRemainderSeconds = (int)Math.Round(dRemainderSeconds);
                    TimeSpan remainderTime = new TimeSpan(0, 0, iRemainderSeconds);
                    string strRemainderTimeMessage = $"剩余时间：{remainderTime}";


                    // 绘制进度条进度
                    ProgressBarPoint.Dispaly(strProgressBarMessage);
                    //显示速度
                    SpeedPoint.Dispaly(strspeedMessage);
                    //显示剩余项目数
                    RemainderNumberPoint.Dispaly(strRemainderNumberMessage);
                    //显示剩余时间数
                    RemainderTimePoint.Dispaly(strRemainderTimeMessage);
                    if (!string.IsNullOrWhiteSpace(strInfo))
                    {
                        //显示信息
                        Loger.LogError(Subject, strInfo);
                    }
                }
            }
            manualResetEvent.Set();
        }

        FixedCoordinate OriginalPoint;
        FixedCoordinate ProgressBarPoint;
        FixedCoordinate SpeedPoint;
        FixedCoordinate RemainderNumberPoint;
        FixedCoordinate RemainderTimePoint;
        FixedCoordinate EndPoint;
        FixedCoordinate InfoPoint;

        public void Dispaly(string strInfo = null)
        {
            FlushSignal.Enqueue(strInfo);
        }
        public void Over()
        {
            manualResetEvent.WaitOne();
            DateTime overTime = DateTime.Now;
            TimeSpan countTime = overTime - StartTime;
            EndPoint.DispalyLine($"任务结束...结束时间：{overTime}\t耗时：{countTime.Hours}:{countTime.Minutes}:{countTime.Seconds}\t平均速度：{Count / countTime.TotalSeconds:f}个/s");
            Console.SetCursorPosition(InfoPoint.Left, InfoPoint.NextTop);
            LogProvider.Close();
            Console.CursorVisible = true;
        }
    }
}
