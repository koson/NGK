using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Reflection;
using Mvp.View;

namespace Mvp.Presenter
{
    public class Presenter<T> : IPresenter, IDisposable 
        where T: IView
    {
        #region Constructor

        public Presenter(T view)
        {
            _View = view;

            _Timer = new Timer(300);
            _Timer.AutoReset = true;
            _Timer.Elapsed += 
                new ElapsedEventHandler(EventHandler_Timer_Elapsed);
            _Timer.Start();
        }

        #endregion

        #region Fields And Properties

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
        /// ѕровер€ем в классе (в его наследника) наличие полей с
        /// атрибутоа CommandAttribute и если такой найден, то
        /// запускаем в нЄм проверку услови€ выполени€ команды
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

        public void Dispose()
        {
            if (_Timer != null)
            {
                _Timer.Stop();
                _Timer.Dispose();
                _Timer = null;
            }
        }

        #endregion
    }
}
