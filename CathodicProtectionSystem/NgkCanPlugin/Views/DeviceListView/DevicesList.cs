using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.Plugins.Models;

namespace NGK.Plugins.Views
{
    public partial class DevicesList : UserControl
    {
        #region Constructors

        public DevicesList()
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
            _DataGridView.RowPostPaint += 
                new DataGridViewRowPostPaintEventHandler(EventHandler_DataGridView_RowPostPaint);

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

        //ISystemButtons _ButtonsPanel;

        //public ISystemButtons ButtonsPanel
        //{
        //    get { return _ButtonsPanel; }
        //    set
        //    {
        //        _ButtonsPanel = value;
        //    }
        //}

        //IViewRegion[] _ViewRegions = new IViewRegion[0];

        //public IViewRegion[] ViewRegions
        //{
        //    get { return _ViewRegions; }
        //}

        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        BindingSource _BindingSourceDevices;

        public BindingList<IDeviceInfo> Devices
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

        private Color _DeviceIsStoppedColor = Color.Orange;
        public Color DeviceIsStoppedColor
        {
            get { return _DeviceIsStoppedColor; }
            set { _DeviceIsStoppedColor = value; }
        }

        private Color _DeviceIsInPreoperationalModeColor = Color.Yellow;
        public Color DeviceIsInPreoperationalModeColor
        {
            get { return _DeviceIsInPreoperationalModeColor; }
            set { _DeviceIsInPreoperationalModeColor = value; }
        }

        private Color _DeviceIsInOperationalModeColor = DataGridView.DefaultBackColor;
        public Color DeviceIsInOperationalModeColor
        {
            get { return _DeviceIsInOperationalModeColor; }
            set { _DeviceIsInOperationalModeColor = value; }
        }

        private Color _DeviceIsInCommunicationErrorColor = Color.Red;
        public Color DeviceIsInCommunicationErrorColor
        {
            get { return _DeviceIsInCommunicationErrorColor; }
            set { _DeviceIsInCommunicationErrorColor = value; }
        }

        private Color _DeviceIsInConfigurationErrorColor = DataGridView.DefaultBackColor;
        public Color DeviceIsInConfigurationErrorColor
        {
            get { return _DeviceIsInConfigurationErrorColor; }
            set { _DeviceIsInConfigurationErrorColor = value; }
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

        void EventHandler_DataGridView_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView control = (DataGridView)sender;
            DataGridViewRow row = control.Rows[e.RowIndex];

            DeviceStatus status = (DeviceStatus)row.Cells["Status"].Value;
            switch (status)
            {
                case DeviceStatus.Stopped:
                    {
                        if (row.DefaultCellStyle.BackColor != DeviceIsStoppedColor)
                            row.DefaultCellStyle.BackColor = DeviceIsStoppedColor;
                        break;
                    }
                case DeviceStatus.Preoperational:
                    {
                        if (row.DefaultCellStyle.BackColor != DeviceIsInPreoperationalModeColor)
                            row.DefaultCellStyle.BackColor = DeviceIsInPreoperationalModeColor;
                        break;
                    }
                case DeviceStatus.Operational:
                    {
                        if (row.DefaultCellStyle.BackColor != DeviceIsInOperationalModeColor)
                            row.DefaultCellStyle.BackColor = DeviceIsInOperationalModeColor;
                        break;
                    }
                case DeviceStatus.CommunicationError:
                    {
                        if (row.DefaultCellStyle.BackColor != DeviceIsInCommunicationErrorColor)
                            row.DefaultCellStyle.BackColor = DeviceIsInCommunicationErrorColor;
                        break;
                    }
                case DeviceStatus.ConfigurationError:
                    {
                        if (row.DefaultCellStyle.BackColor != DeviceIsInConfigurationErrorColor)
                            row.DefaultCellStyle.BackColor = DeviceIsInConfigurationErrorColor;
                        break;
                    }
            }
        }

        #endregion

        #region Event

        public event EventHandler SelectedDeviceChanged;

        #endregion
    }
}
