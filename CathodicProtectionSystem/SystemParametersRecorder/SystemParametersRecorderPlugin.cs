using System;
using System.Collections.Generic;
using System.Text;
using Infrastructure.Api.Plugins;
using Infrastructure.Api.Managers;

namespace SystemParametersRecorder
{
    public class SystemParametersRecorderPlugin : Plugin
    {
        #region Constructors

        public SystemParametersRecorderPlugin() 
        {
            Name = @"��������� �����������";
        }

        #endregion

        #region Fields And Properties
        
        #endregion

        #region Methods

        public override void Initialize(IManagers managers, object state)
        {
            base.Initialize(managers, state);
            
            // ������ ������� ���������� � ������������ ��
            try
            {
                //_SystemEventLogService = new SystemEventLogService(Managers);
                //_SystemEventLogService.Initialize(null);
                //_SystemEventLogService.PageSize = 20;
                //base.ApplicationServices.Add(_SystemEventLogService);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    String.Format("������ ��� ������������� ������� {0}", Name), ex);
            }
        }

        #endregion

        #region Commands
        #endregion
    }
}
