using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class DeviceListView : TemplateView, IDeviceListView
    {
        #region Constructors

        public DeviceListView()
        {
            InitializeComponent();
            Initialize();
        }

        #endregion

        #region Fields And Properties

        public BindingSource Devices
        {
            set
            {
                _DataGridViewDevices.DataSource = null;
                _DataGridViewDevices.DataSource = value;
            }
        }

        #region Event Handlers

        private void EventHandler_DeviceListView_Load(object sender, EventArgs e)
        {
        }

        #endregion

        #endregion

        #region Methods
        
        void Initialize()
        {
            _DataGridViewDevices.AllowUserToAddRows = false;
            _DataGridViewDevices.AllowUserToDeleteRows = false;
            _DataGridViewDevices.ReadOnly = true;
            _DataGridViewDevices.AutoGenerateColumns = true;
            _DataGridViewDevices.AutoSize = true;
            _DataGridViewDevices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewDevices.MultiSelect = false;
            _DataGridViewDevices.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridViewDevices.RowHeadersVisible = false;
            _DataGridViewDevices.DataSourceChanged += new EventHandler(EventHandler_DataGridViewDevices_DataSourceChanged);
        }

        void CustomizeColumns()
        {
            foreach (DataGridViewColumn column in _DataGridViewDevices.Columns)
            {
                 //_DataGridViewDevices.DataSource
            }
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_DataGridViewDevices_DataSourceChanged(object sender, EventArgs e)
        {
            CustomizeColumns();
        }

        #endregion

        #region Event        
        #endregion
    }
}