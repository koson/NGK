using System;
using System.Collections.Generic;
using System.Text;

namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    /// <summary>
    /// ��������� ������ ������ ����
    /// </summary>
    public enum ErrorCategory
    {
        /// <summary>
        /// ��������� ������ �� ����������
        /// </summary>
        Unknown,
        /// <summary>
        /// ������ �������� � ������ ������� ����������
        /// </summary>
        DataLinkLayerError,
        /// <summary>
        /// ������ �������� � ������ ����������� ����
        /// </summary>
        ControllerError,
        /// <summary>
        /// ������ �������� � ������ ����������
        /// </summary>
        DeviceError
    }

    /// <summary>
    /// ����� ��� �������� ���������� ������� NetworkErrorOccurredEventHandler
    /// </summary>
    public class NetworkErrorEventArgs: EventArgs
    {
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ������
        /// </summary>
        private ErrorCategory _Category;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ������
        /// </summary>
        public ErrorCategory Category
        {
            get { return _Category; }
            set { _Category = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������
        /// </summary>
        private String _ErrorDescription;
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������
        /// </summary>
        public String ErrorDescription
        {
            get { return _ErrorDescription; }
            set { _ErrorDescription = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ���������� 
        /// </summary>
        private Exception _InnerException;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ���������� ��������� ������������� ������
        /// </summary>
        public Exception InnerException
        {
            get { return _InnerException; }
            set { _InnerException = value; }
        }
        //---------------------------------------------------------------------------
        #endregion
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        public NetworkErrorEventArgs()
        {
            _Category = ErrorCategory.Unknown;
            _ErrorDescription = String.Empty;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="category">��������� ������</param>
        /// <param name="description">�������� ������</param>
        /// <param name="innerException">���������� ��� ������������� ������</param>
        public NetworkErrorEventArgs(ErrorCategory category, String description, 
            Exception innerException)
        {
            _Category = Category;
            
            if (description == null)
            {
                _ErrorDescription = String.Empty;
            }

            _ErrorDescription = description;
            _InnerException = innerException;
        }
        //---------------------------------------------------------------------------
        #endregion
    }
    /// <summary>
    /// ������� ��� �������� ������� ������������� ������ � ������ ����
    /// </summary>
    /// <param name="sender">����������� �������</param>
    /// <param name="args">��������� �������</param>
    public delegate void NetworkErrorOccurredEventHandler(Object sender, 
        NetworkErrorEventArgs args);
}
