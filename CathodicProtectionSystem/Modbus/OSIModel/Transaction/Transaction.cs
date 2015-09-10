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
            get { return this._Type; }
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
                switch (this._Status)
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

                if (this.IsRunning)
                {
                    result = Environment.TickCount - this._TimeOfStart;
                }
                else
                {
                    result = this._TimeOfEnd - this._TimeOfStart;
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
            get { return this._Request; }
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
            this._Identifier = Transaction.GetRandomNumber();

            this._Answer = null;
            this._Request = null;
            this._Status = TransactionStatus.NotInitialized;
            this._TimeOfEnd = 0;
            this._TimeOfStart = 0;
            this._Type = TransactionType.Undefined;
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
            this._Identifier = Transaction.GetRandomNumber();
            
            this._Type = type;
            this._Answer = null;
            this._Request = request;
            this._Status = TransactionStatus.NotInitialized;
            this._TimeOfEnd = 0;
            this._TimeOfStart = 0;
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
            if (this.IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - ������� ��������� ��� ���������� ����������",
                    this.Identifier));
            }
            else
            {
                this._Status = TransactionStatus.Running;
                this._TimeOfStart = Environment.TickCount;

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ������ ����������: {1} ����",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����������� ������� ����������
        /// </summary>
        /// <param name="answer">����� slave-����������</param>
        public void Stop(Message.Message answer)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� ��������� �� ������� ����������", 
                    this.Identifier));
            }
            else
            {

                switch (this.TransactionType)
                {
                    case TransactionType.UnicastMode:
                        {
                            if (answer != null)
                            {
                                this._Answer = answer;
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
                            this._Answer = answer;
                            break;
                        }
                }

                this._TimeOfEnd = Environment.TickCount;
                this._Status = TransactionStatus.Completed;
                
                // ���������� ������� ��������� ����������.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ����� ����������: {1}; ����� ����������: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        /// <param name="description">��������� �������� ������ ������� ����������</param>
        public void Abort(String description)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� �������� �� ������� ����������",
                    this.Identifier));
            }
            else
            {
                this._TimeOfEnd = Environment.TickCount;
                if (description != null)
                {
                    this._DescriptionError = description;
                }
                else
                {
                    this._DescriptionError = String.Empty;
                }
                this._Status = TransactionStatus.Aborted;
                // ���������� �������
                this.OnTransactionWasEnded();
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// ����� ����������� ������� ���������� ����������
        /// </summary>
        private void OnTransactionWasEnded()
        {
            EventHandler handler = this.TransactionWasEnded;
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
            Transaction copy = (Transaction)this.MemberwiseClone();

            if (this._Request != null)
            {
                copy._Request = new Modbus.OSIModel.Message.Message(this._Request.Address,
                    this._Request.PDUFrame.Function, this._Request.PDUFrame.Data);
            }
            if (this._Answer != null)
            {
                copy._Answer = new Modbus.OSIModel.Message.Message(this._Answer.Address,
                    this._Answer.PDUFrame.Function, this._Answer.PDUFrame.Data);
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
            return this.DeepCopy();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file