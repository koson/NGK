using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public interface ISplashScreenView : IView
    {
        void WriteLine(string text);
        event EventHandler ViewShown;
    }
}
