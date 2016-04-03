using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NGK.CorrosionMonitoringSystem.Views;
using Mvp.View;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class DevicesView : UserControl, IDeviceListView
    {
        #region Constructors

        public DevicesView()
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

            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
            headerCellStyle.ForeColor = Color.Blue;
            headerCellStyle.WrapMode = DataGridViewTriState.True;
            _DataGridView.ColumnHeadersDefaultCellStyle = headerCellStyle;

            _BindingSourceDevices = new BindingSource();
            _BindingSourceDevices.AllowNew = false;
            _BindingSourceDevices.CurrentItemChanged += 
                new EventHandler(EventHandler_BindingSourceDevices_CurrentItemChanged);
            _BindingSourceDevices.ListChanged += 
                new System.ComponentModel.ListChangedEventHandler(
                EventHandler_BindingSourceDevices_ListChanged);

            _DataGridView.DataSource = _BindingSourceDevices;
        }

        #endregion

        #region Fields And Properties

        ISystemButtons _ButtonsPanel;

        public ISystemButtons ButtonsPanel
        {
            get { return _ButtonsPanel; }
            set
            {
                _ButtonsPanel = value;
            }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        BindingSource _BindingSourceDevices;

        public BindingList<NgkCanDevice> Devices
        {
            set 
            {
                _BindingSourceDevices.DataSource = null;
                _BindingSourceDevices.DataSource = value;
            }
        }

        public NgkCanDevice SelectedDevice 
        {
            get
            {
                return _BindingSourceDevices.Current == null ? null :
                        (NgkCanDevice)_BindingSourceDevices.Current;
            }
        }

        #endregion

        #region Methods

        public void Close()
        {
            Dispose();
        }

        void OnSelectedDeviceChanged()
        {
            if (SelectedDeviceChanged != null)
                SelectedDeviceChanged(this, new EventArgs());
        }

        #endregion

        #region Event Handlers

        void EventHandler_BindingSourceDevices_CurrentItemChanged(object sender, EventArgs e)
        {
            OnSelectedDeviceChanged();
        }

        void EventHandler_BindingSourceDevices_ListChanged(
            object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            return;
        }

        #endregion

        #region Event

        public event EventHandler SelectedDeviceChanged;
        
        #endregion
    }
}
