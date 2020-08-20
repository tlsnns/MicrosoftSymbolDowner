using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tlsnn.NetCore.ConsoleUnit
{
    public class ProgressBarSingleton
    {
        private static readonly object SingletonLocker = new object();
        public static ProgressBarSingleton Instance { get; private set; }
        public static ProgressBarSingleton GetInstance(int count, int width = 50)
        {
            if (Instance == null)
            {
                lock (SingletonLocker)
                {
                    if (Instance == null)
                    {
                        Instance = new ProgressBarSingleton(count, width);
                    }
                }
            }
            return Instance;
        }

        ConcurrentQueue<string> FlushSignal;

        //已经完成的项目数
        int HandleCount = 0;

        int Width;
        int Count;

        DateTime StartTime;

        ManualResetEvent manualResetEvent;
        ProgressBarSingleton(int count, int width)
        {
            Width = width;
            Count = count;

            StartTime = DateTime.Now;
            Console.CursorVisible = false;
            Console.WriteLine();

            OriginalPoint = new Coordinate();
            OriginalPoint.DispalyLine($"正在进行任务...任务总数：{Count}\t开始时间：{StartTime}");

            ProgressBarPoint = new Coordinate();
            Console.WriteLine();
            SpeedPoint = new Coordinate();
            Console.WriteLine();
            RemainderNumberPoint = new Coordinate();
            Console.WriteLine();
            RemainderTimePoint = new Coordinate();
            Console.WriteLine();
            EndPoint = new Coordinate();
            Console.WriteLine();
            Console.WriteLine();
            InfoPoint = new Coordinate();

            manualResetEvent = new ManualResetEvent(false);
            FlushSignal = new ConcurrentQueue<string>();
            Thread thread = new Thread(DispalyWorker);
            thread.Start();
        }

        private void DispalyWorker()
        {
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
                    var speed = HandleCount / (DateTime.Now - StartTime).TotalSeconds;//速度
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
                        InfoPoint.AppendDispalyLine(strInfo);
                    }
                }
            }
            manualResetEvent.Set();
        }

        Coordinate OriginalPoint;
        Coordinate ProgressBarPoint;
        Coordinate SpeedPoint;
        Coordinate RemainderNumberPoint;
        Coordinate RemainderTimePoint;
        Coordinate EndPoint;
        Coordinate InfoPoint;

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
            Console.CursorVisible = true;
        }
    }
}
