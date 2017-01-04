using System;
using System.Collections.Generic;
using System.Text;
using Mvp.WinApplication;
using Infrastructure.Api.Managers;
using System.Timers;

namespace NGK.Plugins.ApplicationServices
{
    /// <summary>
    /// ����������� ���������� ������� ���
    /// </summary>
    public class SystemParametersRecorderService : ApplicationServiceBase
    {
        #region Internal Entities  
        /// <summary>
        /// ����� ������ �������
        /// </summary>
        public enum WorkMode
        {
            // ������ ���������� �� �������
            ByTime,
            // ������ ���������� �� ������� (�������)
            ByEvent
        }

        #endregion

        #region Constructors
        /// <summary>
        /// 
        /// </summary>
        /// <param name="managers"></param>
        /// <param name="mode">����� ������ ������������</param>
        /// <param name="interval">������� ������� (���) ��� ������ ���������� �� �������</param>
        public SystemParametersRecorderService(IManagers managers, WorkMode mode, ushort interval)
            :
            base("SystemParametersRecorderService")
        {
            _Managers = managers;
            _Mode = mode;

            if (_Mode == WorkMode.ByTime)
            {
                _Timer = new Timer(Convert.ToDouble(interval) * 1000);
                _Timer.Elapsed += new ElapsedEventHandler(EventHandler_Timer_Elapsed);
            }
        }

        #endregion

        #region Fields And Properties
        
        private readonly IManagers _Managers;
        private readonly WorkMode _Mode;
        private Timer _Timer;
        /// <summary>
        /// ����� ������ ������������
        /// </summary>
        public WorkMode Mode { get { return _Mode; } }

        #endregion

        #region Methods

        public override void Initialize(object context)
        {
            //_Managers.CanNetworkService.Devices[0].

            base.Initialize(context);
        }

        public override void Start()
        {
            if (_Mode == WorkMode.ByTime)
                _Timer.Start();
            
            base.Start();
        }

        public override void Stop()
        {
            if (_Mode == WorkMode.ByTime)
                _Timer.Stop();

            base.Stop();
        }

        public override void Dispose()
        {
            if (_Timer != null)
            {
                _Timer.Dispose();
            }
            base.Dispose();
        }

        #endregion

        #region Event Handlers

        private void EventHandler_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // ��������� ��������� ����������
        }

        #endregion
    }
}
