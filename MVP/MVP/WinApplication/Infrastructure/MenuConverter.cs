using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.Controls;

namespace Mvp.WinApplication.Infrastructure
{
    public static class MenuConverter
    {
        public static BindableToolStripMenuItem ConvertTo(Menu menu)
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
                foreach (Menu subMenu in menu.SubMenuItems)
                {
                    rootItem.DropDownItems.Add(MenuConverter.ConvertTo(subMenu));
                }
            }

            return rootItem;
        }
    }
}
