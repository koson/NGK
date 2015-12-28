using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class NavigationMenuForm : Form, INavigationMenuView
    {
        public NavigationMenuForm()
        {
            InitializeComponent();
        }

        #region INavigationMenuView Members

        public void ShowPivotTableScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowDeviceListScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void ShowDeviceDetailScreen()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}