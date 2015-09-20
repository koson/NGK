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
            get { return _Type; }
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
        /// Запрос от мастера сети
        /// </summary>
        private Message.Message _Request;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Запрос от мастера сети
        /// </summary>
        public Message.Message Request
        {
            get { return _Request; }
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
        /// Конструктор
        /// </summary>
        /// <param name="type">Тип modbus транзации</param>
        /// <param name="request">Запрос от мастера сети</param>
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
            if (IsRunning)
            {
                throw new InvalidOperationException(String.Format(
                    "Transaction ID: {0} - Попытка запустить уже запущенную транзакцию",
                    Identifier));
            }
            else
            {
                _Status = TransactionStatus.Running;
                _TimeOfStart = Environment.TickCount;

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Начало транзакции: {1} мсек",
                //    Identifier.ToString(), _TimeOfStart));
            }
            return;
        }
        /// <summary>
        /// Заканчивает текущую транзакцию
        /// </summary>
        /// <param name="answer">Ответ slave-устройства</param>
        public void Stop(Message.Message answer)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка завершить не начатую транзакцию",
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
                            _Answer = answer;
                            break;
                        }
                }

                _TimeOfEnd = Environment.TickCount;
                _Status = TransactionStatus.Completed;
                
                // Генерируем событие окончания транзакции.
                OnTransactionWasEnded();

                //Debug.WriteLine(String.Format(
                //    "Transaction ID: {0} - Конец транзакции: {1}; Время транзакции: {2}",
                //    Identifier, _TimeOfEnd, TimeOfTransaction));
            }
            return;
        }
        /// <summary>
        /// Прерывает текущую транзакцию
        /// </summary>
        /// <param name="description">Описывает ситуацию отмены текущей транзакции</param>
        public void Abort(String description)
        {
            if (!IsRunning)
            {
                throw new InvalidOperationException(
                    String.Format("Transaction ID: {0}; Попытка отменить не начатую транзакцию",
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
                // Генерируем событие
                OnTransactionWasEnded();
            }
            return;
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод гененрирует событие завершения транзакции
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
        /// Возвращает копию данной транзации (глубокое копирование)
        /// </summary>
        /// <returns>Копия объекта</returns>
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