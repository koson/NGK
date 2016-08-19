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
    public class Menu
    {
        #region Constructors

        public Menu(string name, Command action)
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
        private List<Menu> _SubMenuItems = new List<Menu>();

        public Guid Uid { get { return _Uid; } }
        /// <summary>
        /// �������� ����
        /// </summary>
        public string Text { get { return _Text; } }
        /// <summary>
        /// �������� ��� ������ ����
        /// </summary>
        public Command Action { get { return _Command; } }
        /// <summary>
        /// ��������� ����
        /// </summary>
        public List<Menu> SubMenuItems { get { return _SubMenuItems; } }

        #endregion
    }
}
