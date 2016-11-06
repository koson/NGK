using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Infrastructure.Api.Models;

namespace NGK.Plugins.Views
{
    public partial class SystemEventsLog : UserControl
    {
        #region Constructors
        
        public SystemEventsLog()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;

            _DataGridViewSystemEventsLog.AllowUserToAddRows = false;
            _DataGridViewSystemEventsLog.AllowUserToDeleteRows = false;
            _DataGridViewSystemEventsLog.AllowUserToOrderColumns = false;
            _DataGridViewSystemEventsLog.AutoGenerateColumns = true;
            _DataGridViewSystemEventsLog.AutoSize = true;
            _DataGridViewSystemEventsLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _DataGridViewSystemEventsLog.ColumnHeadersVisible = true;
            _DataGridViewSystemEventsLog.Dock = DockStyle.Fill;
            _DataGridViewSystemEventsLog.MultiSelect = false;
            _DataGridViewSystemEventsLog.RowHeadersVisible = false;
            _DataGridViewSystemEventsLog.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _DataGridViewSystemEventsLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //_DataGridViewSystemEventsLog.DataBindingComplete +=
            //    new DataGridViewBindingCompleteEventHandler(EventHandler_DataGridView_DataBindingComplete);
            //_DataGridView.CellFormatting += 
            //    new DataGridViewCellFormattingEventHandler(_DataGridView_CellFormatting);

            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
            headerCellStyle.ForeColor = Color.Blue;
            headerCellStyle.WrapMode = DataGridViewTriState.True;
            _DataGridViewSystemEventsLog.ColumnHeadersDefaultCellStyle = headerCellStyle;

            _BindingSourceSystemEvents = new BindingSource();
            _BindingSourceSystemEvents.AllowNew = false;
            //_BindingSourceSystemEvents.CurrentItemChanged +=
            //    new EventHandler(EventHandler_BindingSourceDevices_CurrentItemChanged);
            //_BindingSourceSystemEvents.ListChanged += new ListChangedEventHandler(
            //    EventHandler_BindingSourceDevices_ListChanged);

            _DataGridViewSystemEventsLog.DataSource = _BindingSourceSystemEvents;
        }

        #endregion
        
        #region Fields And Properties

        private BindingSource _BindingSourceSystemEvents;

        public DataTable SystemEvents
        {
            set
            {
                _BindingSourceSystemEvents.DataSource = null;
                _BindingSourceSystemEvents.DataSource = value;
            }
        }

        #endregion

        #region Methods
            
        public void Close()
        {
            Dispose();
        }

        #endregion
    }
}
