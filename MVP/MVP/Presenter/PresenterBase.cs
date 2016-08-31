using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Mvp.Input;

namespace Mvp.Presenter
{
    /// <summary>
    /// ������� ���������������� - ��������� �������
    /// </summary>
    public abstract class PresenterBase
    {
        #region Constructor

        public PresenterBase()
        {
            _Commands = new List<ICommand>();
        }

        #endregion

        #region Fields And Properties

        protected List<ICommand> _Commands;
        /// <summary>
        /// ������ ������������������ ������� �������������
        /// </summary>
        public ICommand[] Commands
        {
            get { return _Commands.ToArray(); }
        }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            protected set { _Name = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ��������� ��������� ������
        /// </summary>
        protected void UpdateStatusCommands()
        {
            foreach (ICommand cmd in _Commands)
            {
                cmd.CanExecute();
            }
        }
        /// <summary>
        /// ��������� � ������ (� ��� ����������) ������� ����� �
        /// ��������� CommandAttribute � ���� ����� ������, ��
        /// ��������� � �� �������� ������� ��������� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FindCommands()
        {
            FieldInfo[] fields = this.GetType().GetFields(
                BindingFlags.Instance | 
                BindingFlags.NonPublic |
                BindingFlags.GetField);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(Command))
                {
                    Command cmd = (Command)field.GetValue(this);
                    MethodInfo method = cmd.GetType().GetMethod("CheckCondition", 
                        BindingFlags.NonPublic |
                        BindingFlags.InvokeMethod |
                        BindingFlags.Instance);
                    method.Invoke(cmd, new object[0]);
                }
                
                //if (field.FieldType.IsDefined(typeof(CommandAttribute), false))
                //{ 
                //}
            }
        }

        protected void InitializeCommands()
        {
            List<ICommand> commands = new List<ICommand>();

            FieldInfo[] fields = this.GetType().GetFields(
                BindingFlags.Instance |
                BindingFlags.NonPublic |
                BindingFlags.GetField);

            foreach (FieldInfo field in fields)
            {
                if (field.FieldType == typeof(Command))
                {
                    Command cmd = (Command)field.GetValue(this);
                    commands.Add(cmd);
                }
            }
            _Commands = commands;
        }
        /// <summary>
        /// ���������� ���
        /// </summary>
        public abstract void Show();
        /// <summary>
        /// ��������� ���
        /// </summary>
        public abstract void Close();

        #endregion
    }
}
