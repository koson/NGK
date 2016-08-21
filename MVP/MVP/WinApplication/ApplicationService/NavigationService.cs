using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication.Infrastructure;

namespace Mvp.WinApplication.ApplicationService
{
    /// <summary>
    /// Класс реализует навигационное меню по приложению
    /// </summary>
    public static class NavigationService
    {
        static NavigationService()
        {
            Menu = new List<NavigationMenuItem>();
        }

        public static List<NavigationMenuItem> Menu;
    }
}
