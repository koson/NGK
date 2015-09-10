using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave
{
    //===============================================================================
    /// <summary>
    /// ��������� ������ ������ ����
    /// </summary>
    public enum ErrorCategory
    {
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ������ �� ����������
        /// </summary>
        Unknown,
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� � ������ ������� ����������
        /// </summary>
        DataLinkLayerError,
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� � ������ ����������� ����
        /// </summary>
        ControllerError,
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ �������� � ������ ����������
        /// </summary>
        DeviceError
        //---------------------------------------------------------------------------
    }
    //===============================================================================
    /// <summary>
    /// ����� ��� �������� ���������� ������� NetworkErrorOccurredEventHandler
    /// </summary>
    public class NetworkErrorEventArgs: EventArgs
    {
        //---------------------------------------------------------------------------
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
            get { return this._Category; }
            set { this._Category = value; }
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
            get { return this._ErrorDescription; }
            set { this._ErrorDescription = value; }
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
            get { return this._InnerException; }
            set { this._InnerException = value; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        public NetworkErrorEventArgs()
        {
            this._Category = ErrorCategory.Unknown;
            this._ErrorDescription = String.Empty;
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
            this._Category = Category;
            
            if (description == null)
            {
                this._ErrorDescription = String.Empty;
            }

            this._ErrorDescription = description;
            this._InnerException = innerException;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
    /// <summary>
    /// ������� ��� �������� ������� ������������� ������ � ������ ����
    /// </summary>
    /// <param name="sender">����������� �������</param>
    /// <param name="args">��������� �������</param>
    public delegate void NetworkErrorOccurredEventHandler(Object sender, 
        NetworkErrorEventArgs args);
    //===============================================================================
}
//===================================================================================
// End of file