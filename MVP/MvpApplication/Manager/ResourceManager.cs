using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MvpApplication.Manager
{
    public class ResourceManager: IResourceManager
    {
        #region Methods

        public Bitmap GetImage(string name, System.Drawing.Image defaultImage)
        {
            throw new NotImplementedException();
        }

        public Bitmap GetLogo
        {
            get { return Properties.Resources.Logo; }
        }


        #endregion
    }
}
