using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mvp.View
{
    public class ViewRegion: IViewRegion 
    {
        public ViewRegion(Panel panel)
        {
            _Control = panel;
        }

        #region Fields And Properties

        Control _Control;

        public string RegionName
        {
            get { return _Control.Name; }
        }

        public void Show(IView partialView)
        {
            if (partialView.ViewType != ViewType.Region)
            {
                throw new ArgumentException(
                    "Попытка отобразить представление в регионе, не являющееся частичным", 
                    "partialView");
            }
            Control control = (Control)partialView;
            _Control.Controls.Clear();
            _Control.Controls.Add(control);
        }

        #endregion
    }
}
