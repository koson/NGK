using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public enum HTMLHelpCommand: int
        {
            HH_DISPLAY_TOPIC = 0, 
            HH_DISPLAY_TOC = 1,
            HH_DISPLAY_INDEX = 2, 
            HH_DISPLAY_SEARCH = 3, 
            HH_HELP_CONTEXT = 0x000F,
            HH_TP_HELP_CONTEXTMENU = 0x0010, 
            HH_TP_HELP_WM_HELP = 0x0011, // для Pop-up
            HH_CLOSE_ALL = 0x0012 
        }

        [DllImport("hhctrl.ocx", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr HtmlHelp(
            IntPtr hWndCaller, 
            string pszFile, 
            int uCommand, 
            int dwData);
        [DllImport("hhctrl.ocx", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        public static extern IntPtr HtmlHelp(
            IntPtr hWndCaller,
            string pszFile,
            int uCommand,
            //[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_I4)]int[] dwData);
            [MarshalAs(UnmanagedType.LPArray, SizeConst=4)]int[] dwData);
        
        // Возвращает ID контрола по его дескриптору, или NULL - если всё плохо.
        [DllImport("user32.dll")]
        static extern int GetDlgCtrlID(IntPtr hwndCtl);


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();


        private String[] cmd;
        
        public Form1()
        {
            InitializeComponent();

        }

        public Form1(String[] cmdLine)
        {
            InitializeComponent();
            cmd = cmdLine;
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {
            return;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Rectangle rec = Screen.PrimaryScreen.Bounds;
            //Size s = System.Windows.Forms.SystemInformation.PrimaryMonitorSize;
            labelResolutionOfScreen.Text = "Разрешение экрана: " + Screen.PrimaryScreen.Bounds.Size.ToString();

            // Set the SystemEvents class to receive event notification when a user  
            // preference changes, the palette changes, or when display settings change.
            // Данное событие происходит при изменении разрешения экрана. Необходимо подключить
            // Microsoft.Win32;
            SystemEvents.DisplaySettingsChanged +=
                new EventHandler(SystemEvents_DisplaySettingsChanged);
            SystemEvents.PaletteChanged += 
                new EventHandler(SystemEvents_PaletteChanged);
            // Происходит, например, при изменении цвета окна
            SystemEvents.UserPreferenceChanged += 
                new UserPreferenceChangedEventHandler(SystemEvents_UserPreferenceChanged);
            // For demonstration purposes, this application sits idle waiting for events.
            //Console.WriteLine("This application is waiting for system events.");
            //Console.WriteLine("Press <Enter> to terminate this application.");
            //Console.ReadLine();

            if (AllocConsole())
            {
                //Console.WriteLine("Приведко,я консолько");
                //Console.ReadLine();
                //FreeConsole();
            }

            ConsoleTraceListener console = new ConsoleTraceListener();
            console.Name = "console";
            Trace.Listeners.Add(console);
            Trace.Listeners["console"].TraceOutputOptions |= (TraceOptions.Callstack | TraceOptions.Timestamp);

            XmlWriterTraceListener xmltracer = new XmlWriterTraceListener("logxml.xml", "xmltracer");
            Trace.Listeners.Add(xmltracer);
            Trace.Listeners["xmltracer"].TraceOutputOptions |= (TraceOptions.Callstack | TraceOptions.Timestamp);
            
            Trace.AutoFlush = true;
            return;
        }

        void SystemEvents_UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
        {
            MessageBox.Show(e.ToString());
        }

        void SystemEvents_PaletteChanged(object sender, EventArgs e)
        {
            MessageBox.Show("PaletteChanged");
        }

        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            labelResolutionOfScreen.Text = "Разрешение экрана: " + Screen.PrimaryScreen.Bounds.Size.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < cmd.Length; i++)
			{
                sb.Append(cmd[i]);
                sb.Append("\n");
			}

            MessageBox.Show(this, sb.ToString(), "Аргументы коммандной строки",
                MessageBoxButtons.OK, MessageBoxIcon.Information); 
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            labelLocal.Text = DateTime.Now.ToLocalTime().ToLongTimeString();
            labelUtc.Text = DateTime.Now.ToUniversalTime().ToLongTimeString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            String pathHelp = Application.StartupPath + @"\NGKDevicesTerminalHelp.chm";
            //Help.ShowHelp(this, pathHelp, HelpNavigator.TableOfContents);
            //Help.ShowHelp(this, pathHelp);
            //Help.ShowHelp(this, pathHelp, HelpNavigator.Topic, @"HTML\Introduction.html");
            //HtmlHelp(this.Handle, pathHelp + "::/HTML/DescriptionOfWork.html#OffLine", (Int32)HTMLHelpCommand.HH_DISPLAY_TOPIC, 0);
            //Help.ShowHelp(this, pathHelp, HelpNavigator.Topic, @"HTML\DescriptionOfWork.html#OffLine");
            //Help.ShowHelp(this, pathHelp, HelpNavigator.TopicId, "7"); // см Map.h для создания файля справки
            //HtmlHelp(this.Handle, pathHelp, (Int32)HTMLHelpCommand.HH_HELP_CONTEXT, 7);
            
            Int32 id = GetDlgCtrlID(button1.Handle);
            Int32[] ids = new int[] {id, 0x22, 0};
            IntPtr hendl;
            //hendl = HtmlHelp(this.Handle, pathHelp + @"::/popups\Popups.txt", (Int32)HTMLHelpCommand.HH_TP_HELP_WM_HELP, ids);
            hendl = HtmlHelp(this.Handle, pathHelp + @"::/Popups.txt", (Int32)HTMLHelpCommand.HH_TP_HELP_CONTEXTMENU, ids);

            //Help.ShowPopup(button1, "Всплывающая подсказка", this.PointToScreen(button1.Location));
            return;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            #if DEBUG
                Trace.WriteLine("Вывод в окно консоли");
                Trace.TraceError("Ошибочка");
                Trace.Flush();
            #else
                Console.WriteLine("В режиме RELEASE");               
            #endif
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Закрываем консоль
            FreeConsole();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Method();
            //try
            //{
            //    Method();
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLine(ex.StackTrace);
            //}
        }

        private void Method()
        {
            int x, y, z;
            y = 10;
            z = 0;
            x = y / z;
            //throw new Exception("Моё исключение");
        }

    }
}