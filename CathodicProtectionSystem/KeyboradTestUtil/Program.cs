using System;
using System.Collections.Generic;
using System.Text;

namespace KeyboradTestUtil
{
    public class Program
    {
        static void Main(string[] args)
        {
            ConsoleKeyInfo keyInfo;
            int rowPosition;

            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.CreateSpecificCulture("ru-RU");
            Console.OutputEncoding = System.Text.Encoding.GetEncoding(1251);
            Console.InputEncoding = System.Text.Encoding.GetEncoding(1251);
            System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
            proc.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding(1251);


            Console.CursorVisible = false;
            Console.CancelKeyPress += 
                new ConsoleCancelEventHandler(EventHandler_Console_CancelKeyPress);

            Console.Title = "������� ��� ������������ ���������";
            Console.WriteLine("��������� ��� ������������ ����������");
            Console.WriteLine("��� ������ ������� CNTR+C ��� CNTR+BREAK...");
            Console.WriteLine("������� ������� ��� � �����������...");
            Console.WriteLine("");

            do
            {

                //rowPosition = Console.CursorTop;
                keyInfo = Console.ReadKey(true);
                Console.Clear();
                Console.WriteLine("��������� ��� ������������ ����������");
                Console.WriteLine("��� ������ ������� CNTR+C ��� CNTR+BREAK...");
                Console.WriteLine("������� ������� ��� � �����������...");
                Console.WriteLine("");
                PrintKey(keyInfo);
                //Console.SetCursorPosition(0, rowPosition);
            }
            while(true);
        }

        private static void EventHandler_Console_CancelKeyPress(
            object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = false;
            return;
        }

        private static void PrintKey(ConsoleKeyInfo keyInfo)
        {
            if (Char.IsControl(keyInfo.KeyChar))
            {
                Console.WriteLine("������: {0}", keyInfo.Key);
            }
            else
            {
                Console.WriteLine("������: {0}", keyInfo.KeyChar);
            }
            Console.WriteLine("��� �������: {0}", (int)keyInfo.Key);
            Console.WriteLine("");
            return;
        }
    }
}
