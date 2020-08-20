using System;

namespace tlsnn.NetCore.ConsoleUnit
{
    class Coordinate
    {
        public Coordinate() : this(Console.CursorLeft, Console.CursorTop)
        { }
        public Coordinate(int left, int top)
        {
            Left = left;
            Top = top;
            NextTop = Top;
        }
        public int Left { get; private set; }
        public int Top { get; private set; }
        public int NextTop { get; private set; }


        public void Dispaly(string strValue)
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(strValue);
            ClearLine();
        }
        public void DispalyLine(string strValue)
        {
            Console.SetCursorPosition(Left, Top);
            Console.Write(strValue);
            ClearLine();
            Console.WriteLine();
        }
        public void AppendDispalyLine(string strValue)
        {
            Console.SetCursorPosition(Left, NextTop);
            Console.Write(strValue);
            ClearLine();
            NextTop++;
        }
        //清除本行剩余列
        void ClearLine()
        {
            int startLeft = Console.CursorLeft;
            for (int i = startLeft; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }
        }
    }
}
