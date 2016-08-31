using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Mvp.View;

namespace NGK.CorrosionMonitoringSystem.Views
{
    public partial class SplashScreenForm : Form
    {
        #region Data Type

        internal class Content
        {
            private const int MAXLINES = 3;
            private Queue<string> _Strings;

            internal Content()
            {
                _Strings = new Queue<string>();
            }

            internal void WriteLine(string text)
            {
                if (_Strings.Count >= MAXLINES)
                {
                    _Strings.Dequeue();
                }
                _Strings.Enqueue(text);
            }

            public override string ToString()
            {
                if (_Strings.Count == 0)
                {
                    return String.Empty;
                }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (string x in _Strings)
                    {
                        sb.Append(x + Environment.NewLine);
                    }
                    return sb.ToString();
                }
            }
        }

        #endregion

        #region Constructors

        public SplashScreenForm()
        {
            InitializeComponent();
            _Content = new Content();
        }

        #endregion

        #region Fields And Properties

        private Content _Content;
        
        public string Info
        {
            get { return _LabelOutputInfo.Text; }
            set
            {
                if (_LabelOutputInfo.InvokeRequired)
                {
                    _LabelOutputInfo.Invoke(new Action<string>(
                    delegate(string text) { _LabelOutputInfo.Text = text; }), value);
                }
                else
                { 
                    _LabelOutputInfo.Text = value;
                }
            }
        }

        #endregion

        #region EventHandler
        #endregion

        #region Methods
        
        /// <summary>
        /// Выводит текст на экран
        /// </summary>
        /// <param name="text"></param>
        public void WriteLine(string text)
        {
            _Content.WriteLine(text);

            if (_LabelOutputInfo.InvokeRequired)
                _LabelOutputInfo.Invoke(new MethodInvoker(delegate() 
                { 
                    _LabelOutputInfo.Text = _Content.ToString(); 
                }));
            else
                _LabelOutputInfo.Text = _Content.ToString();
        }

        #endregion

        #region Events
        #endregion
    }
}