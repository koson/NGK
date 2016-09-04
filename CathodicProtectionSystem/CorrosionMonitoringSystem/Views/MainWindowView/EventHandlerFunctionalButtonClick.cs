using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public class EventArgsFunctionalButtonClick : EventArgs
    {
        #region Constructors

        public EventArgsFunctionalButtonClick(Keys button)
        {
            _Button = button;
        }

        #endregion

        #region Fields And Properties

        private Keys _Button;

        public Keys Button
        {
            get { return _Button; }
        }

        #endregion
    }
}
