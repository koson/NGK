using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.WinApplication;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class NavigationMenuView : Form, INavigationMenuView
    {
        #region Constructors

        public NavigationMenuView()
        {
            InitializeComponent();

            SelectedMenuItem = NavigationMenuItems.NoSelection;
        }

        #endregion

        #region Fields And Properties

        NavigationMenuItems _SelectedMenuItem;

        public NavigationMenuItems SelectedMenuItem
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

        public ViewType ViewType { get { return ViewType.Window; } }

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
                SelectedMenuItem = NavigationMenuItems.NoSelection;
            }
            else if (btn.Equals(_ButtonPivotTable))
            {
                SelectedMenuItem = NavigationMenuItems.PivoteTable;
            }
            else if (btn.Equals(_ButtonDeviceList))
            {
                SelectedMenuItem = NavigationMenuItems.DeviceList;
            }
            else if (btn.Equals(_ButtonDeviceDetail))
            {
                SelectedMenuItem = NavigationMenuItems.DeviceDetail;
            }
            else if (btn.Equals(_ButtonLogViewer))
            {
                SelectedMenuItem = NavigationMenuItems.LogViewer;
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