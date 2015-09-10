using System;
using System.Collections.Generic;
using System.Text;
using NGK.CorrosionMonitoringSystem.Forms.Controls;

namespace NGK.CorrosionMonitoringSystem.Forms
{
    public delegate void SystemButtonClickEventHandler(object sender, SystemButtonClickEventArgs args); 
    
    public class SystemButtonClickEventArgs: EventArgs
    {
        private String _ButtonName;

        public String Button
        {
            get { return _ButtonName; }
            set { _ButtonName = value; }
        }

        public SystemButtonClickEventArgs()
        {
            _ButtonName = String.Empty;
        }

        public SystemButtonClickEventArgs(string buttonName)
        {
            _ButtonName = buttonName;
        }
    }
}
