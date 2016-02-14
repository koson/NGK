using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Presenter;

namespace NGK.CorrosionMonitoringSystem.Services
{
    public interface IWindowsService
    {
        DialogResult ShowDialog(IPresenter presenter);
        void Show(IPresenter presenter);
        Form CurrentWindow { get; }
    }
}
