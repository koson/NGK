using System;
using System.Diagnostics;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.Transaction
{
    //===============================================================================
    /// <summary>
    /// Класс для хранения данных транзакции "Запрос-ответ"
    /// </summary>
    public class Transaction: ICloneable
    {
        //---------------------------------------------------------------------------
        #region Fields And Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Тип modbus-транзацкии "запрос-ответ"
        /// </summary>
        private TransactionType _Type;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает тип текущей транзакции
        /// </summary>
        public TransactionType TransactionType
        {
            get { return this._Type; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Состояние транзакции
        /// </summary>
        private TransactionStatus _Status;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Состояние транзакции
        /// </summary>
        public TransactionStatus Status
        {
            get { return _Status; }
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        /// <summary>
        /// Время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private Int32 _TimeOfStart;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает время начала транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public Int32 TimeOfStart
        {
            get { return _TimeOfStart; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        private Int32 _TimeOfEnd;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает время окончания транзакции, мсек
        /// </summary>
        /// <remarks>Количество мсек от старта системы</remarks>
        public Int32 TimeOfEnd
        {
            get { return _TimeOfEnd; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Длительность транзакции, мсек
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
        /// Запрос от мастера сети
        /// </summary>
        private Message.Message _Request;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запрос от мастера сети
        /// </summary>
        public Message.Message Request
        {
            get { return this._Request; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ответ от ведомого устройства
        /// </summary>
        private Message.Message _Answer;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Ответ от ведомого устройства
        /// </summary>
        public Message.Message Answer
        {
            get { return _Answer; }
            set { _Answer = value; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        private Int32 _Identifier;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Идентификатор транзакции
        /// </summary>
        public Int32 Identifier
        {
            get { return _Identifier; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание причины звершения транзакции при вызове метода Abort()
        /// </summary>
        private String _DescriptionError;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает описание причины звершения транзакции при вызове метода Abort()
        /// </summary>
        public String DescriptionError
        {
            get { return _DescriptionError; }
        }        
        //---------------------------------------------------------------------------
        /// <summary>
        /// Статическое поле генератора псеводслучайных чисел
        /// </summary>
        private static Random _Random = new Random(Environment.TickCount);
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
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
        /// Конструктор
        /// </summary>
        /// <param name="type">Тип modbus транзации</param>
        /// <param name="request">Запрос от мастера сети</param>
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
        /// Возвращает случайное число
        /// </summary>
        /// <returns>Случайное число</returns>
        public static Int32 GetRandomNumber()
        {
            return Transaction._Random.Next();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Начинает новую транзакцию
        /// </summary>
        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - Попытка запустить уже запущенную транзакцию",
                    this.Identifier));
            }
            else
            {
                this._Status = TransactionStatus.Running;
                this._TimeOfStart = Environment.TickCount;

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Начало транзакции: {1} мсек",
                //    this.Identifier.ToString(), this._TimeOfStart));
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Заканчивает текущую транзакцию
        /// </summary>
        /// <param name="answer">Ответ slave-устройства</param>
        public void Stop(Message.Message answer)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка завершить не начатую транзакцию", 
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
                                    "Попытка установить в null ответное сообщение для завершения " +
                                    "транзакции адресованного запроса");
                            }
                            break;
                        }
                    case TransactionType.BroadcastMode:
                        {
                            if (answer != null)
                            {
                                throw new InvalidOperationException(
                                    "Попытка установить ответное сообщение для завершения транзакции " +
                                    "широковещательного запроса");
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
                
                // Генерируем событие окончания транзакции.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Конец транзакции: {1}; Время транзакции: {2}",
                //    this.Identifier, this._TimeOfEnd, this.TimeOfTransaction));
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Прерывает текущую транзакцию
        /// </summary>
        /// <param name="description">Описывает ситуацию отмены текущей транзакции</param>
        public void Abort(String description)
        {
            if (!this.IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка отменить не начатую транзакцию",
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
                // Генерируем событие
                this.OnTransactionWasEnded();
            }
            return;
        }
        //---------------------------------------------------------------------------
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
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает копию данной транзации (глубокое копирование)
        /// </summary>
        /// <returns>Копия объекта</returns>
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
        /// Событие возникает после завершения транзакции;
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