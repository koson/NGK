using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.View
{
    public partial class PivotTableView : TemplateView, IPivotTableView
    {
        public PivotTableView()
        {
            InitializeComponent();
        }

        #region EventHandler

        private void EventHandler_PivotTableView_Load(object sender, EventArgs e)
        {
        }

        #endregion

        #region Field And Properties

        #endregion

        #region IView Members

        void IView.Show()
        {
            this.Show();
        }

        void IView.Close()
        {
            this.Close();
        }

        #endregion

        #region IPivotTableView Members

        event EventHandler<ButtonClickEventArgs> IButtonsPanel.ButtonClick
        {
            add
            {
                lock (SyncRoot)
                {
                    ButtonClick += value;
                }
            }
            remove 
            {
                lock (SyncRoot)
                {
                    ButtonClick -= value;
                }
            }
        }

        #endregion

        #region IView Members

        string IView.Name
        {
            get { return Name; }
            set { Name = value; }
        }

        #endregion
    }
}