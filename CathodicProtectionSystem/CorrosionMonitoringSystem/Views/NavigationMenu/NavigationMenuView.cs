using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.WinApplication;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class NavigationMenuView : Form, INavigationMenuView
    {
        #region Constructors

        public NavigationMenuView()
        {
            InitializeComponent();
    
            SelectedMenuItem = ViewMode.NoSelection;
        }

        #endregion

        #region Fields And Properties

        ViewMode _SelectedMenuItem;

        public ViewMode SelectedMenuItem
        {
            get { return _SelectedMenuItem; }
            set { _SelectedMenuItem = value; }
        }

        public bool PivoteTableMenuEnabled
        {
            get { return _ButtonPivotTable.Enabled; }
            set { _ButtonPivotTable.Enabled = value; }
        }

        public bool DeviceListMenuEnabled
        {
            get { return _ButtonDeviceList.Enabled; }
            set { _ButtonDeviceList.Enabled = value; }
        }

        public bool DeviceDetailMenuEnabled
        {
            get { return _ButtonDeviceDetail.Enabled; }
            set { _ButtonDeviceDetail.Enabled = value; }
        }

        public bool LogViewerMenuEnabled
        {
            get { return _ButtonLogViewer.Enabled; }
            set { _ButtonLogViewer.Enabled = value; }
        }

        public ViewType ViewType { get { return ViewType.Dialog; } }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }

        #endregion

        #region Event Handlers

        void EventHandler_NavigationMenuForm_Load(object sender, EventArgs e)
        {
            _ButtonExit.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonPivotTable.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonDeviceList.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonDeviceDetail.Click += new EventHandler(EventHandler_Button_Click);
            _ButtonLogViewer.Click += new EventHandler(EventHandler_Button_Click);
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Equals(_ButtonExit))
            {
                SelectedMenuItem = ViewMode.NoSelection;
            }
            else if (btn.Equals(_ButtonPivotTable))
            {
                SelectedMenuItem = ViewMode.PivoteTable;
            }
            else if (btn.Equals(_ButtonDeviceList))
            {
                SelectedMenuItem = ViewMode.DeviceList;
            }
            else if (btn.Equals(_ButtonDeviceDetail))
            {
                SelectedMenuItem = ViewMode.DeviceDetail;
            }
            else if (btn.Equals(_ButtonLogViewer))
            {
                SelectedMenuItem = ViewMode.LogViewer;
            }
            else
            {
                throw new InvalidOperationException(string.Format(
                    "Нет кода для обработки кнопки меню {0}", btn.Name));
            }
        }

        void EventHandler_NavigationMenuView_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            OnMenuClosed();
        }

        #endregion

        #region Methods

        void OnMenuClosed()
        {
            if (MenuClosed != null)
            {
                MenuClosed(this, new EventArgs());
            }
        }

        #endregion

        #region Events

        public event EventHandler MenuClosed;

        #endregion

    }
}