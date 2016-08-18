using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using System.Windows.Forms;

namespace TestApplication
{
    public class MyApplication: WindowsFormsApplicationBase
    {
        #region Constructors
        
        public MyApplication(Form mainForm, bool isSingleInstance)
        {
            _MainForm = mainForm;
            base.IsSingleInstance = isSingleInstance;
        }

        #endregion

        #region Fields And Properties

        private Form _MainForm;
        

        #endregion

        #region Methods

        protected override bool OnInitialize(System.Collections.ObjectModel.ReadOnlyCollection<string> commandLineArgs)
        {
            return base.OnInitialize(commandLineArgs);
        }

        protected override void OnCreateMainForm()
        {
            base.OnCreateMainForm();

            //base.MainForm = new FormMain();
            base.MainForm = _MainForm;
        }

        #endregion
    }
}
