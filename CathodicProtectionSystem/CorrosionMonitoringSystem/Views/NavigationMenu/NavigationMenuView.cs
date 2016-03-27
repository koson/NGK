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

            _TableLayoutPanel = new TableLayoutPanel();
            _TableLayoutPanel.Name = "_TableLayoutPanel";
            //_TableLayoutPanel.Size = Size;
            _TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _TableLayoutPanel.Dock = DockStyle.Fill;
            _TableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            
            this.Controls.Add(_TableLayoutPanel);

            _ButtonExit = new Button();
            _ButtonExit.Name = "_ButtonExit";
            _ButtonExit.Text = "Закрыть";
            _ButtonExit.Dock = DockStyle.Fill;
            _ButtonExit.Click += new EventHandler(EventHandler_ButtonExit_Click);
        }

        #endregion

        #region Fields And Properties

        TableLayoutPanel _TableLayoutPanel;
        Button _ButtonExit;

        public ViewType ViewType { get { return ViewType.Dialog; } }

        IViewRegion[] _ViewRegions = new IViewRegion[0];

        public IViewRegion[] ViewRegions
        {
            get { return _ViewRegions; }
        }
        ViewMode _SelectedMenuItem;

        public ViewMode SelectedMenuItem
        {
            get { return _SelectedMenuItem; }
            set { _SelectedMenuItem = value; }
        }

        ICommand[] _MenuCommands;

        public ICommand[] MenuItems 
        {
            set 
            { 
                _MenuCommands = value;
                BuildMenu();
            }
        }

        #endregion

        #region Event Handlers

        void EventHandler_NavigationMenuForm_Load(object sender, EventArgs e)
        {
        }

        void EventHandler_ButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        void EventHandler_Button_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn.Tag != null)
            {
                if (btn.Tag is ICommand)
                {
                    ((ICommand)btn.Tag).Execute();
                    Close();
                }
            }
        }

        void EventHandler_NavigationMenuView_FormClosed(
            object sender, FormClosedEventArgs e)
        {
            OnMenuClosed();
        }

        void EventHandler_cmd_CanExecuteChanged(object sender, EventArgs e)
        {
            ICommand cmd = (ICommand)sender;
            foreach (Button btn in _TableLayoutPanel.Controls)
            {
                if (cmd.Equals(btn.Tag))
                {
                    btn.Enabled = cmd.Status;
                    break;
                }
            }
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

        void BuildMenu()
        {
            _TableLayoutPanel.SuspendLayout();

            _TableLayoutPanel.Controls.Clear();
            _TableLayoutPanel.RowStyles.Clear();

            if ((_MenuCommands != null))
            {
                Button btn;
                int rowHeight = 100 / (_MenuCommands.Length + 1);

                _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowHeight));
                _TableLayoutPanel.Controls.Add(_ButtonExit);

                this.Height += _TableLayoutPanel.Height;

                foreach (ICommand cmd in _MenuCommands)
                {
                    cmd.CanExecuteChanged += new EventHandler(EventHandler_cmd_CanExecuteChanged);

                    _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, rowHeight));
                    
                    btn = new Button();
                    btn.Text = cmd.Name;
                    btn.Tag = cmd;
                    btn.Enabled = cmd.Status;
                    btn.Dock = DockStyle.Fill;
                    btn.Click += new EventHandler(EventHandler_Button_Click);
                    _TableLayoutPanel.Controls.Add(btn);
                }
            }
            else
            {
                _TableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                _TableLayoutPanel.Controls.Add(_ButtonExit);
            }

            _TableLayoutPanel.ResumeLayout();
        }

        #endregion

        #region Events

        public event EventHandler MenuClosed;

        #endregion

    }
}