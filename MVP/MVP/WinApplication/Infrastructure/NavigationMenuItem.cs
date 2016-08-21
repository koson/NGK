using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Input;

namespace Mvp.WinApplication.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NavigationMenuItem
    {
        #region Constructors

        public NavigationMenuItem(string name, Command action)
        {
            _Uid = Guid.NewGuid();
            _Text = name;
            _Command = action;
        }

        #endregion

        #region Fields And Properties

        private Guid _Uid; 
        private string _Text;
        private Command _Command;
        private List<NavigationMenuItem> _SubMenuItems = new List<NavigationMenuItem>();

        public Guid Uid { get { return _Uid; } }
        /// <summary>
        /// Название меню
        /// </summary>
        public string Text { get { return _Text; } }
        /// <summary>
        /// Действие при вызове меню
        /// </summary>
        public Command Action { get { return _Command; } }
        /// <summary>
        /// Вложенные меню
        /// </summary>
        public List<NavigationMenuItem> SubMenuItems { get { return _SubMenuItems; } }

        #endregion
    }
}
