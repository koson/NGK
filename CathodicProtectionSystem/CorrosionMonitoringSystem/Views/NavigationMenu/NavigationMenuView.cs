using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.WinApplication;
using Mvp.View;
using Mvp.Input;

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
            get { return true; }
            set { return; }
        }

        public bool DeviceListMenuEnabled
        {
            get { return true; }
            set { return; }
        }

        public bool DeviceDetailMenuEnabled
        {
            get { return true; }
            set { return; }
        }

        public bool LogViewerMenuEnabled
        {
            get { return true; }
            set { return; }
        }

        ICommand[] _MenuCommands;

        public ICommand[] MenuItems 
        {
            set 
            { 
                _MenuCommands = value;
                BuidMenu();
            }
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
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            //if (btn.Equals(_ButtonExit))
            //{
            //    //SelectedMenuItem = ViewMode.NoSelection;
            //}
            //else if (btn.Equals(_ButtonPivotTable))
            //{
            //    SelectedMenuItem = ViewMode.PivoteTable;
            //}
            //else if (btn.Equals(_ButtonDeviceList))
            //{
            //    SelectedMenuItem = ViewMode.DeviceList;
            //}
            //else if (btn.Equals(_ButtonDeviceDetail))
            //{
            //    SelectedMenuItem = ViewMode.DeviceDetail;
            //}
            //else if (btn.Equals(_ButtonLogViewer))
            //{
            //    SelectedMenuItem = ViewMode.LogViewer;
            //}
            //else
            //{
            //    throw new InvalidOperationException(string.Format(
            //        "Íåò êîäà äëÿ îáðàáîòêè êíîïêè ìåíþ {0}", btn.Name));
            //}
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

        void BuidMenu()
        {
            if ((_MenuCommands == null) || (_MenuCommands.Length == 0))
            {
                List<Control> controlsToRemove = new List<Control>();
                
                foreach (Control control in _ÒableLayoutPanel.Controls)
                {
                    if ((control is Button) && (!control.Equals(_ButtonExit)))
                    {
                        controlsToRemove.Add(control);
                    }
                }

                foreach (Control control in controlsToRemove)
                {
                    _ÒableLayoutPanel.Controls.Remove(control);
                }

                _ÒableLayoutPanel.RowCount = _ÒableLayoutPanel.Controls.Count;
            }
            else
            {
                Button btn;

                foreach (ICommand cmd in _MenuCommands)
                {
                    btn = new Button();
                    btn.Text = cmd.Name;
                    btn.Tag = cmd;
                    btn.Size = _ButtonExit.Size;
                    btn.Click += new EventHandler(EventHandler_Button_Click);
                    _ÒableLayoutPanel.RowCount++;
                    _ÒableLayoutPanel.Controls.Add(btn);
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler MenuClosed;

        #endregion

    }
}