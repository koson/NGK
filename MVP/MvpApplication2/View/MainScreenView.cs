using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace MvpApplication2.View
{
    public partial class MainScreenView : Form, IMainScreenView
    {
        public MainScreenView()
        {
            InitializeComponent();
        }

        #region IMainScreenView Members

        public void SomeMethod()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IMainScreenView Members

        void IMainScreenView.SomeMethod()
        {
            throw new Exception("The method or operation is not implemented.");
        }

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
    }
}