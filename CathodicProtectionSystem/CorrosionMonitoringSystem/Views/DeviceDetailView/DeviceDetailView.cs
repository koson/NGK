using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views.DeviceDetailView
{
    public partial class DeviceDetailView : UserControl, IDeviceDetailView
    {
        #region Constructors

        public DeviceDetailView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;

            _DataGridView.AllowUserToAddRows = false;
            _DataGridView.AllowUserToDeleteRows = false;
            _DataGridView.AllowUserToOrderColumns = false;
            _DataGridView.AutoGenerateColumns = true;
            _DataGridView.AutoSize = true;
            _DataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridView.ColumnHeadersVisible = true;
            _DataGridView.Dock = DockStyle.Fill;
            _DataGridView.MultiSelect = false;
            _DataGridView.RowHeadersVisible = false;
            _DataGridView.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _DataGridView.DataBindingComplete += 
                new DataGridViewBindingCompleteEventHandler(EventHandler_DataGridView_DataBindingComplete);

            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
            headerCellStyle.ForeColor = Color.Blue;
            headerCellStyle.WrapMode = DataGridViewTriState.True;
            _DataGridView.ColumnHeadersDefaultCellStyle = headerCellStyle;

            _BindingSourceParameters = new BindingSource();
            _BindingSourceParameters.AllowNew = false;
            _BindingSourceParameters.CurrentItemChanged += 
                new EventHandler(EventHandler_BindingSourceDevices_CurrentItemChanged);
            _BindingSourceParameters.ListChanged += new ListChangedEventHandler(
                EventHandler_BindingSourceDevices_ListChanged);

            _DataGridView.DataSource = _BindingSourceParameters;
        }



        #endregion

        #region Fields And Properties

        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        BindingSource _BindingSourceParameters;

        public BindingList<Parameter> Parameters
        {
            set 
            {
                _BindingSourceParameters.DataSource = null;
                _BindingSourceParameters.DataSource = value; 
            }
        }

        #endregion

        #region Methods

        public void Close()
        {
            Dispose();
        }

        #endregion

        #region Event Handlers
        
        void EventHandler_BindingSourceDevices_ListChanged(object sender, ListChangedEventArgs e)
        {
            return;
        }

        void EventHandler_BindingSourceDevices_CurrentItemChanged(object sender, EventArgs e)
        {
            return;
        }

        void EventHandler_DataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView control = (DataGridView)sender;

            foreach (DataGridViewColumn column in control.Columns)
            {
                if (column.ValueType == typeof(DateTime))
                {
                    column.DefaultCellStyle.Format = "G";
                }
            }
        }

        #endregion
    }
}
