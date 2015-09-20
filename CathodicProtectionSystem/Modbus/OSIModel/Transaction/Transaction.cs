using System;
using System.Diagnostics;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.Transaction
{
    //===============================================================================
    /// <summary>
    /// ����� ��� �������� ������ ���������� "������-�����"
    /// </summary>
    public class Transaction: ICloneable
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��� modbus-���������� "������-�����"
        /// </summary>
        private TransactionType _Type;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ��� ������� ����������
        /// </summary>
        public TransactionType TransactionType
        {
            get { return _Type; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ����������
        /// </summary>
        private TransactionStatus _Status;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ����������
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ��������� ����������
        /// </summary>
        public Boolean IsRunning
        {
            get
            {
                Boolean result;
                switch (_Status)
                {
                    case TransactionStatus.Aborted:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Completed:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.NotInitialized:
                        {
                            result = false; break;
                        }
                    case TransactionStatus.Running:
                        {
                            result = true; break;
                        }
                    default:
                        {
                            throw new NotImplementedException(
                                "������ ��������� ���������� �� �������������� � ������ ������ ��");
                        }
                }
                return result;
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private Int32 _TimeOfStart;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public Int32 TimeOfStart
        {
            get { return _TimeOfStart; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private Int32 _TimeOfEnd;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public Int32 TimeOfEnd
        {
            get { return _TimeOfEnd; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������������ ����������, ����
        /// </summary>
        public Int32 TimeOfTransaction
        {
            get 
            {
                Int32 result;

                if (IsRunning)
                {
                    result = Environment.TickCount - _TimeOfStart;
                }
                else
                {
                    result = _TimeOfEnd - _TimeOfStart;
                }
                return result;
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ �� ������� ����
        /// </summary>
        private Message.Message _Request;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������ �� ������� ����
        /// </summary>
        public Message.Message Request
        {
            get { return _Request; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� �� �������� ����������
        /// </summary>
        private Message.Message _Answer;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� �� �������� ����������
        /// </summary>
        public Message.Message Answer
        {
            get { return _Answer; }
            set { _Answer = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������������� ����������
        /// </summary>
        private Int32 _Identifier;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public Int32 Identifier
        {
            get { return _Identifier; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        private String _DescriptionError;
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        public String DescriptionError
        {
            get { return _DescriptionError; }
        }        
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����������� ���� ���������� ��������������� �����
        /// </summary>
        private static Random _Random = new Random(Environment.TickCount);
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        public Transaction() 
        {
            _Identifier = Transaction.GetRandomNumber();

            _Answer = null;
            _Request = null;
            _Status = TransactionStatus.NotInitialized;
            _TimeOfEnd = 0;
            _TimeOfStart = 0;
            _Type = TransactionType.Undefined;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="type">��� modbus ���������</param>
        /// <param name="request">������ �� ������� ����</param>
        public Transaction(TransactionType type, 
            Message.Message request)
        {
            _Identifier = Transaction.GetRandomNumber();
            
            _Type = type;
            _Answer = null;
            _Request = request;
            _Status = TransactionStatus.NotInitialized;
            _TimeOfEnd = 0;
            _TimeOfStart = 0;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ��������� �����
        /// </summary>
        /// <returns>��������� �����</returns>
        public static Int32 GetRandomNumber()
        {
            return Transaction._Random.Next();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        public void Start()
        {
            if (IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - ������� ��������� ��� ���������� ����������",
                    Identifier));
            }
            else
            {
                _Status = TransactionStatus.Running;
                _TimeOfStart = Environment.TickCount;

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ������ ����������: {1} ����",
                //    Identifier.ToString(), _TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// ����������� ������� ����������
        /// </summary>
        /// <param name="answer">����� slave-����������</param>
        public void Stop(Message.Message answer)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� ��������� �� ������� ����������",
                    Identifier));
            }
            else
            {

                switch (TransactionType)
                {
                    case TransactionType.UnicastMode:
                        {
                            if (answer != null)
                            {
                                _Answer = answer;
                            }
                            else
                            {
                                throw new NullReferenceException(
                                    "������� ���������� � null �������� ��������� ��� ���������� " +
                                    "���������� ������������� �������");
                            }
                            break;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if (answer != null)
                            {
                                throw new InvalidOperationException(
                                    "������� ���������� �������� ��������� ��� ���������� ���������� " +
                                    "������������������ �������");
                            }
                            break;
                        }
                    case TransactionType.Undefined:
                        {
                            _Answer = answer;
                            break;
                        }
                }

                _TimeOfEnd = Environment.TickCount;
                _Status = TransactionStatus.Completed;
                
                // ���������� ������� ��������� ����������.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ����� ����������: {1}; ����� ����������: {2}",
                //    Identifier, _TimeOfEnd, TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        /// <param name="description">��������� �������� ������ ������� ����������</param>
        public void Abort(String description)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� �������� �� ������� ����������",
                    Identifier));
            }
            else
            {
                _TimeOfEnd = Environment.TickCount;
                if (description != null)
                {
                    _DescriptionError = description;
                }
                else
                {
                    _DescriptionError = String.Empty;
                }
                _Status = TransactionStatus.Aborted;
                // ���������� �������
                OnTransactionWasEnded();
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ����������� ������� ���������� ����������
        /// </summary>
        private void OnTransactionWasEnded()
        {
            EventHandler handler = TransactionWasEnded;
            EventArgs args = new EventArgs();

            if (handler != null)
            {
                foreach (EventHandler singleCast in handler.GetInvocationList())
                {
                    System.ComponentModel.ISynchronizeInvoke syncInvoke =
                        singleCast.Target as System.ComponentModel.ISynchronizeInvoke;
                    if (syncInvoke != null)
                    {
                        if (syncInvoke.InvokeRequired)
                        {
                            syncInvoke.Invoke(singleCast, new Object[] { this, args });
                        }
                        else
                        {
                            singleCast(this, args);
                        }
                    }
                    else
                    {
                        singleCast(this, args);
                    }
                }
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ���������� ����� ������ ��������� (�������� �����������)
        /// </summary>
        /// <returns>����� �������</returns>
        public Transaction DeepCopy()
        {
            Transaction copy = (Transaction)MemberwiseClone();

            if (_Request != null)
            {
                copy._Request = new Modbus.OSIModel.Message.Message(_Request.Address,
                    _Request.PDUFrame.Function, _Request.PDUFrame.Data);
            }
            if (_Answer != null)
            {
                copy._Answer = new Modbus.OSIModel.Message.Message(_Answer.Address,
                    _Answer.PDUFrame.Function, _Answer.PDUFrame.Data);
            }

            return copy;
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Events
        //---------------------------------------------------------------------------
        /// <summary>
        /// ������� ��������� ����� ���������� ����������;
        /// </summary>
        public event EventHandler TransactionWasEnded; 
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region ICloneable Members
        //---------------------------------------------------------------------------
        object ICloneable.Clone()
        {
            return DeepCopy();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file