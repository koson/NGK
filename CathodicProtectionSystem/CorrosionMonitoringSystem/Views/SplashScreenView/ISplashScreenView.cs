using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public interface ISplashScreenView : IView
    {
        void WriteLine(string text);
        event EventHandler ViewShown;
    }
}
