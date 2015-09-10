using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.DataLinkLayer.Message;

namespace NGK.CAN.ApplicationLayer.Transactions
{
    /// <summary>
    /// Класс для определения сетевой транзакции в сети CAN
    /// </summary>
    public class Transaction: ICloneable
    {
        #region Fields And Properties
        /// <summary>
        /// Запрос к удалённому устройству
        /// </summary>
        private Frame? _Request;
        /// <summary>
        /// Запрос к удалённому устройству
        /// </summary>
        public Frame? Request
        {
            get { return _Request; }
            //set { _Request = value; }
        }
        /// <summary>
        /// Ответ от удалённого устройства
        /// </summary>
        private Frame? _Answer;
        /// <summary>
        /// Ответ от удалённого устройства
        /// </summary>
        public Frame? Answer
        {
            get { return _Answer; }
            //set { _Answer = value; }
        }
        private TransactionStatus _Status;
        /// <summary>
        /// Состояние сетевой транзакции
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
        /// <summary>
        /// Возвращает состояние транзакции
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
                                "Данное состояние транзакции не поддерживается в данной версии ПО");
                        }
                }
                return result;
            }
        }
        /// <summary>
        /// Время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private DateTime _TimeOfStart;
        /// <summary>
        /// Возвращает время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public DateTime TimeOfStart
        {
            get { return _TimeOfStart; }
        }
        /// <summary>
        /// Время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private DateTime _TimeOfEnd;
        /// <summary>
        /// Возвращает время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public DateTime TimeOfEnd
        {
            get { return _TimeOfEnd; }
        }
        /// <summary>
        /// Длительность транзакции, мсек
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
        /// Идентификатор транзакции
        /// </summary>
        private Guid _Identifier;
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        public Guid Identifier
        {
            get { return _Identifier; }
        }
        /// <summary>
        /// Описание причины звершения транзакции при вызове метода Abort()
        /// </summary>
        private String _DescriptionError;
        /// <summary>
        /// Возвращает описание причины звершения транзакции при вызове метода Abort()
        /// </summary>
        public String DescriptionError
        {
            get { return _DescriptionError; }
        }
        private TransactionType _TransactionType;
        /// <summary>
        /// Тип сетевой транзакции
        /// </summary>
        public TransactionType TransactionType
        {
            get { return _TransactionType; }
            set { _TransactionType = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
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
        /// Начинает новую транзакцию
        /// </summary>
        public void Start(TransactionType type, Frame request)
        {
            string msg;

            if (this.IsRunning)
            {
                msg = String.Format(
                    "Transaction ID: {0}; Попытка запустить уже запущенную (активную) транзакцию",
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
                                "Transaction ID: {0}; Попытка начать сетевую транзакцию неопределённого типа",
                                _Identifier);
                            Abort(null, msg);
                            throw new InvalidOperationException(msg);
                        }
                    default:
                        {
                            msg = String.Format("Transaction ID: {0}; Попытка начать сетевую транзакцию " +
                                "тип которой не поддерживается - {1}", _Identifier, _TransactionType);
                            Abort(null, msg);
                            throw new NotImplementedException(msg);
                        }
                }
                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Начало транзакции: {1} мсек",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// Заканчивает текущую транзакцию (удачное завершение)
        /// </summary>
        /// <param name="answer">Ответ slave-устройства</param>
        public void Stop(Frame? answer)
        {
            string msg;

            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка завершить не начатую транзакцию",
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
                                    "Transaction ID: {0}; Попытка установить в null ответное сообщение для завершения " +
                                    "транзакции адресованного запроса", _Identifier);
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
                                    "Transaction ID: {0}; Попытка установить ответное сообщение для завершения транзакции " +
                                    "широковещательного запроса", _Identifier);
                                Abort(answer, msg);
                                throw new InvalidOperationException(msg);
                            }
                            break;
                        }
                    default:
                        {
                            msg = String.Format("Transaction ID: {0}; Попытка остановить сетевую транзакцию " +
                                "тип которой не поддерживается - {1}", _Identifier, _TransactionType);
                            Abort(answer ,msg);
                            throw new NotImplementedException(msg);
                        }
                }

                this._TimeOfEnd = DateTime.Now;
                this._Status = TransactionStatus.Completed;

                // Генерируем событие окончания транзакции.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Конец транзакции: {1}; Время транзакции: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// Прерывает текущую транзакцию, произошла ошибка
        /// </summary>
        /// <param name="answer">Ответ, если он есть</param>
        /// <param name="description">Описывает ситуацию отмены текущей транзакции</param>
        public void Abort(Frame? answer, String description)
        {
            string msg;

            if (!IsRunning)
            {
                msg = String.Format("Transaction ID: {0}; Попытка отменить не начатую транзакцию",
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
            // Генерируем событие
            OnTransactionWasEnded();
            return;
        }
        /// <summary>
        /// Повторяет данную транзакцию, если она была завершена
        /// </summary>
        public void Repeat()
        {
            Start(_TransactionType, _Request.Value);
        }
        /// <summary>
        /// Метод гененрирует событие завершения транзакции
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
        /// Возвращает копию данной транзации (глубокое копирование)
        /// </summary>
        /// <returns>Копия объекта</returns>
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
        /// Событие возникает после завершения транзакции;
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
