using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Windows.Forms;
using System.ComponentModel;
using Infrastructure.Dal.DbEntity;

namespace NGK.Plugins.Views
{
    public class SystemEventsLogArchiveView: PartialView<SystemEventsLogArchive>
    {
        #region Fields And Properties

        public DockStyle Dock
        {
            get { return base.Control.Dock; }
            set { base.Control.Dock = value; }
        }

        public BindingList<ISystemEventMessage> SystemEvents
        {
            set { Control.SystemEvents = value; }
        }

        public int PageNumber
        {
            set { Control.PageNumber = value; }
        }

        public int TotalPages
        {
            set { Control.TotalPages = value; }
        }

        #endregion 
    }
}
