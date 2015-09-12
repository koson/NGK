using System;
using System.Collections.Generic;
using System.Windows.Forms;
// 
using NGK.CAN.OSIModel.DataLinkLayer.CanPort.Fastwel.NIM351;
using NGK.CAN.OSIModel.DataLinkLayer.CanPort.Fastwel.NIM351.Forms;

namespace TestCanPortNim351
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormTestPort(new CanPort(1)));
        }
    }
}