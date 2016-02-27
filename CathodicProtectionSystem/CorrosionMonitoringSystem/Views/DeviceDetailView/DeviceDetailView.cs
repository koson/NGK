using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class DeviceDetailView : TemplateView, IDeviceDetailView
    {
        #region Constructors

        public DeviceDetailView()
        {
            InitializeComponent();

            _DataGridViewParametersViewer.AutoGenerateColumns = true;
        }

        #endregion

        #region Fields And Properties
        
        BindingSource _ParametersContext;
        public BindingSource ParametersContext
        {
            set 
            { 
                _ParametersContext = value;
                _DataGridViewParametersViewer.DataSource = _ParametersContext;
            }
        }
        
        #endregion
    }
}