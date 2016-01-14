using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Reflection;
using Mvp.View;
using Mvp.Input;

namespace Mvp.Presenter
{
    [CommandsControlMode(TimersPeriod=200, 
        UpdatingCanExecuteMode=CommandsControlModeAttribute.Mode.Manual)]
    public class Presenter<T> : IPresenter, IDisposable 
        where T: IView
    {
        #region Constructor

        public Presenter(T view)
        {
            // ����������� ����� �������� ����������� ������
            Type type = this.GetType();
            object[] attrs = 
                type.GetCustomAttributes(typeof(CommandsControlModeAttribute), true);
            CommandsControlModeAttribute attr = (CommandsControlModeAttribute)attrs[0];
            CommandsControlMode = attr.UpdatingCanExecuteMode;
            TimersPeriod = attr.TimersPeriod;

            _View = view;

            switch (CommandsControlMode)
            {
                case CommandsControlModeAttribute.Mode.ByTimer:
                    {
                        _Timer = new Timer(TimersPeriod);
                        _Timer.AutoReset = true;
                        _Timer.Elapsed +=
                            new ElapsedEventHandler(EventHandler_Timer_Elapsed);
                        _Timer.Start();
                        break;
                    }
                case CommandsControlModeAttribute.Mode.Manual:
                    {
                        break;
                    }
                default:
                    {
                        throw new NotSupportedException();
                    }
            }
        }

        #endregion

        #region Fields And Properties

        /// <summary>
        /// ����� ���������� ����������� ������ 
        /// </summary>
        public readonly CommandsControlModeAttribute.Mode CommandsControlMode;
        /// <summary>
        /// ������ ���������� �� ����������� ������ �� ������� 
        /// (��� �������������� ������)
        /// </summary>
        public readonly double TimersPeriod;
        
        ICommand[] _Commands;
        /// <summary>
        /// ������ ������������������ ������� �������������
        /// </summary>
        public ICommand[] Commands
        {
            get { return _Commands; }
        }

        Timer _Timer;
        protected T _View;

        public IView View
        {
            get { return _View; }
        }

        protected string _Name;

        public string Name
        {
            get { return _Name; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// ��������� � ������ (� ��� ����������) ������� ����� �
        /// ��������� CommandAttribute � ���� ����� ������, ��
        /// ��������� � �� �������� ������� ��������� �������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Timer tmr = (Timer)sender;
            tmr.Stop();

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

            tmr.Start();
        }

        public void UpdateStatusCommands()
        {
            if (CommandsControlMode == CommandsControlModeAttribute.Mode.Manual)
            { }
            else
            {
                throw new InvalidOperationException(
                    "����������� ����� ������ ������� ���������� ����������� ������, ��� �� ������ ������");
            }
        }

        public void Dispose()
        {
            if (_Timer != null)
            {
                _Timer.Stop();
                _Timer.Dispose();
                _Timer = null;
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
            _Commands = commands.ToArray();
        }

        #endregion
    }
}
