using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NGK.CorrosionMonitoringSystem.Views;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class DevicesView : UserControl, IDeviceListView
    {
        #region Constructors

        public DevicesView()
        {
            InitializeComponent();
        }

        #endregion

        #region Fields And Properties

        IButtonsPanel _ButtonsPanel;

        public IButtonsPanel ButtonsPanel
        {
            get { return _ButtonsPanel; }
            set
            {
                _ButtonsPanel = value;
            }
        }

        #endregion

        #region IDeviceListView Members

        public BindingSource Devices
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion

        #region IView Members


        public ViewType ViewType
        {
            get { return ViewType.Control; }
        }

        public void Close()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IButtonsPanel Members

        public bool ButtonF3IsAccessible
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool ButtonF4IsAccessible
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool ButtonF5IsAccessible
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public string ButtonF3Text
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public string ButtonF4Text
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public string ButtonF5Text
        {
            set { throw new Exception("The method or operation is not implemented."); }
        }

        public event EventHandler<ButtonClickEventArgs> ButtonClick;

        #endregion
    }
}
