using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.ApplicationLayer.Transactions
{
    /// <summary>
    /// ����� ��� ����������� ������� ���������� � ���� CAN
    /// </summary>
    public class Transaction: ICloneable
    {
        #region Fields And Properties
        /// <summary>
        /// ������ � ��������� ����������
        /// </summary>
        private Frame? _Request;
        /// <summary>
        /// ������ � ��������� ����������
        /// </summary>
        public Frame? Request
        {
            get { return _Request; }
            //set { _Request = value; }
        }
        /// <summary>
        /// ����� �� ��������� ����������
        /// </summary>
        private Frame? _Answer;
        /// <summary>
        /// ����� �� ��������� ����������
        /// </summary>
        public Frame? Answer
        {
            get { return _Answer; }
            //set { _Answer = value; }
        }
        private TransactionStatus _Status;
        /// <summary>
        /// ��������� ������� ����������
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
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
        /// <summary>
        /// ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _TimeOfStart;
        /// <summary>
        /// ���������� ����� ������ ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public DateTime TimeOfStart
        {
            get { return _TimeOfStart; }
        }
        /// <summary>
        /// ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        private DateTime _TimeOfEnd;
        /// <summary>
        /// ���������� ����� ��������� ����������, ����
        /// </summary>
        /// <remarks>���������� ���� �� ������ �������</remarks>
        public DateTime TimeOfEnd
        {
            get { return _TimeOfEnd; }
        }
        /// <summary>
        /// ������������ ����������, ����
        /// </summary>
        public TimeSpan TimeOfTransaction
        {
            get
            {
                TimeSpan result;

                if (this.IsRunning)
                {
                    result = DateTime.Now - _TimeOfStart; ;
                }
                else
                {
                    result = _TimeOfEnd - _TimeOfStart;
                }
                return result;
            }
        }
        /// <summary>
        /// ������������� ����������
        /// </summary>
        private Guid _Identifier;
        /// <summary>
        /// ������������� ����������
        /// </summary>
        public Guid Identifier
        {
            get { return _Identifier; }
        }
        /// <summary>
        /// �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        private String _DescriptionError;
        /// <summary>
        /// ���������� �������� ������� ��������� ���������� ��� ������ ������ Abort()
        /// </summary>
        public String DescriptionError
        {
            get { return _DescriptionError; }
        }
        private TransactionType _TransactionType;
        /// <summary>
        /// ��� ������� ����������
        /// </summary>
        public TransactionType TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        public Transaction()
        {
            _Identifier = Guid.NewGuid();
            _Status = TransactionStatus.NotInitialized;
            _TransactionType = TransactionType.Undefined;

            _Answer = null;
            _Request = null;
            _TimeOfStart = DateTime.Now;
            _TimeOfEnd = _TimeOfStart;
        }

        #endregion

        #region Methods
        /// <summary>
        /// �������� ����� ����������
        /// </summary>
        public void Start(TransactionType type, Frame request)
        {
            string msg;

            if (this.IsRunning)
            {
                msg = String.Format(
                    "Transaction ID: {0}; ������� ��������� ��� ���������� (��������) ����������",
                    _Identifier);
                throw new InvalidOperationException(msg);
            }
            else
            {
                _Request = request;
                _Status = TransactionStatus.Running;

                switch (type)
                {
                    case TransactionType.BroadcastMode:
                    case TransactionType.UnicastMode:
                        {
                            _TimeOfStart = DateTime.Now;
                            this._TransactionType = type;
                            break;
                        }
                    case TransactionType.Undefined:
                        {
                            msg = String.Format(
                                "Transaction ID: {0}; ������� ������ ������� ���������� �������������� ����",
                                _Identifier);
                            Abort(null, msg);
                            throw new InvalidOperationException(msg);
                        }
                    default:
                        {
                            msg = String.Format("Transaction ID: {0}; ������� ������ ������� ���������� " +
                                "��� ������� �� �������������� - {1}", _Identifier, _TransactionType);
                            Abort(null, msg);
                            throw new NotImplementedException(msg);
                        }
                }
                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ������ ����������: {1} ����",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// ����������� ������� ���������� (������� ����������)
        /// </summary>
        /// <param name="answer">����� slave-����������</param>
        public void Stop(Frame? answer)
        {
            string msg;

            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; ������� ��������� �� ������� ����������",
                    _Identifier));
            }
            else
            {
                switch (_TransactionType)
                {
                    case TransactionType.UnicastMode:
                        {
                            if (answer.HasValue)
                            {
                                _Answer = answer;
                            }
                            else
                            {
                                msg = String.Format(
                                    "Transaction ID: {0}; ������� ���������� � null �������� ��������� ��� ���������� " +
                                    "���������� ������������� �������", _Identifier);
                                Abort(answer, msg);
                                throw new NullReferenceException(msg);
                            }
                            break;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if (answer != null)
                            {
                                msg = String.Format(
                                    "Transaction ID: {0}; ������� ���������� �������� ��������� ��� ���������� ���������� " +
                                    "������������������ �������", _Identifier);
                                Abort(answer, msg);
                                throw new InvalidOperationException(msg);
                            }
                            break;
                        }
                    default:
                        {
                            msg = String.Format("Transaction ID: {0}; ������� ���������� ������� ���������� " +
                                "��� ������� �� �������������� - {1}", _Identifier, _TransactionType);
                            Abort(answer ,msg);
                            throw new NotImplementedException(msg);
                        }
                }

                this._TimeOfEnd = DateTime.Now;
                this._Status = TransactionStatus.Completed;

                // ���������� ������� ��������� ����������.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - ����� ����������: {1}; ����� ����������: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// ��������� ������� ����������, ��������� ������
        /// </summary>
        /// <param name="answer">�����, ���� �� ����</param>
        /// <param name="description">��������� �������� ������ ������� ����������</param>
        public void Abort(Frame? answer, String description)
        {
            string msg;

            if (!IsRunning)
            {
                msg = String.Format("Transaction ID: {0}; ������� �������� �� ������� ����������",
                    this.Identifier);
                throw new InvalidOperationException(msg);
            }
            else
            {
                _Answer = answer;
                _TimeOfEnd = DateTime.Now;
                _Status = TransactionStatus.Aborted;
                _DescriptionError = String.IsNullOrEmpty(description) ? String.Empty : description; 
            }
            // ���������� �������
            OnTransactionWasEnded();
            return;
        }
        /// <summary>
        /// ��������� ������ ����������, ���� ��� ���� ���������
        /// </summary>
        public void Repeat()
        {
            Start(_TransactionType, _Request.Value);
        }
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
        /// <summary>
        /// ���������� ����� ������ ��������� (�������� �����������)
        /// </summary>
        /// <returns>����� �������</returns>
        public Transaction DeepCopy()
        {
            Transaction copy = (Transaction)this.MemberwiseClone();
            
            copy._Request = _Request.HasValue ? (Frame?)_Request.Value.Clone() : null;
            copy._Answer = _Answer.HasValue ? (Frame?)_Answer.Value.Clone() : null;

            return copy;
        }
        
        #endregion
        
        #region Events
        /// <summary>
        /// ������� ��������� ����� ���������� ����������;
        /// </summary>
        public event EventHandler TransactionWasEnded;

        #endregion

        #region ICloneable Members
        
        public object Clone()
        {
            return this.DeepCopy();
        }
        
        #endregion
    }
}
