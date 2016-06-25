using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Mvp.View;
using NGK.CorrosionMonitoringSystem.Models;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class PivotTableView : UserControl, IPivotTableView
    {
        #region Constructors

        public PivotTableView()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;

            _DataGridView.AutoGenerateColumns = true;
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
            _DataGridView.ReadOnly = true;
            _DataGridView.AllowUserToResizeRows = false;
            _DataGridView.RowHeadersWidthSizeMode =
                DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            _DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.Font = new Font(DataGridView.DefaultFont, FontStyle.Bold);
            headerCellStyle.ForeColor = Color.Blue;
            headerCellStyle.WrapMode = DataGridViewTriState.True;
            _DataGridView.ColumnHeadersDefaultCellStyle = headerCellStyle;

            _DataGridView.DefaultCellStyle.NullValue = "---";
            _DataGridView.DefaultCellStyle.DataSourceNullValue = null;

            //_DataGridView.DataBindingComplete += 
            //    new DataGridViewBindingCompleteEventHandler(EventHandler_DataGridView_DataBindingComplete);

            _BindingSourceParameters = new BindingSource();
            _BindingSourceParameters.AllowNew = false;
            //_BindingSourceParameters.CurrentItemChanged +=
            //    new EventHandler(EventHandler_BindingSourceParameters_CurrentItemChanged);
            //_BindingSourceParameters.ListChanged +=
            //    new System.ComponentModel.ListChangedEventHandler(
            //    EventHandler_BindingSourceParameters_ListChanged);

            _DataGridView.DataSource = _BindingSourceParameters;

        }

        #endregion

        #region Fields And Properties

        BindingSource _BindingSourceParameters;

        public ViewType ViewType
        {
            get { return ViewType.Region; }
        }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        public DataTable Parameters
        {
            set 
            {
                _BindingSourceParameters.DataSource = null;
                _BindingSourceParameters.DataSource = value;
            }
        }

        public BindingList<IDeviceSummaryParameters> DeviceParameters
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

        public void HideColumn(string columnName)
        {
            if (_DataGridView.Columns.Contains(columnName))
            {
                _DataGridView.Columns[columnName].Visible = false;
            }
        }

        #endregion

        #region EventHandlers
        
        void EventHandler_DataGridView_DataBindingComplete(
            object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            // Меняем заголовки столбцов
            //DataTable table = (DataTable)((BindingSource)dgv.DataSource).DataSource;

            //foreach (DataGridViewColumn column in dgv.Columns)
            //{
            //    column.HeaderText = table.Columns[column.Name].Caption;
            //}
        }

        #endregion

        #region IPivotTableView Members

        #endregion
    }
}
