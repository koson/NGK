using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Controls;

namespace Mvp.WinApplication.Infrastructure
{
    public static class NavigationMenuItemConverter
    {
        public static BindableToolStripMenuItem ConvertTo(NavigationMenuItem menu)
        {
            BindableToolStripMenuItem rootItem;
            rootItem = new BindableToolStripMenuItem();
            rootItem.Name = "_MenuItem" + menu.Uid.ToString();
            rootItem.Text = menu.Text;

            if (menu.SubMenuItems.Count == 0)
            {
                rootItem.Action = menu.Action;
            }
            else
            {
                foreach (NavigationMenuItem subMenu in menu.SubMenuItems)
                {
                    rootItem.DropDownItems.Add(NavigationMenuItemConverter.ConvertTo(subMenu));
                }
            }

            return rootItem;
        }
    }
}
