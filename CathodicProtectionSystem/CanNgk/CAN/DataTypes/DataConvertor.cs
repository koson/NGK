using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Базовый класс для создания типов данных
    /// </summary>
    [Serializable]
    public abstract class DataConvertor: IXmlSerializable
    {
        #region Fields And Properties
        protected decimal _Scaler;
        /// <summary>
        /// Множитель числа. Задаёт маштаб числа
        /// (См struct Scaler)
        /// </summary>
        public decimal Scaler 
        { 
            get { return _Scaler; } 
        }
        protected Boolean _Signed;
        /// <summary>
        /// Число знаковое или беззнаковое
        /// </summary>
        public Boolean Signed 
        { 
            get { return _Signed; } 
        }
        /// <summary>
        /// Булев тип
        /// </summary>
        public abstract Boolean IsBoolean
        { get; }

        #endregion

        #region Constructors
  
        #endregion

        #region Methods
        /// <summary>
        /// Преобразует базовое число (значение передаваемое по сети) 
        /// в конечное число 
        /// </summary>
        /// <param name="value">Неопределённое значение полученное в ответе  
        /// (знаковое/беззнаковое, целое/вещественное)</param>
        /// <returns></returns>
        public abstract ValueType ConvertToTotalValue(UInt32 basis);
        /// <summary>
        /// Преобразует значение в базис для отправки по сети 
        /// или для хранения в словаре объектов
        /// </summary>
        /// <param name="totalValue"></param>
        /// <returns></returns>
        public abstract UInt32 ConvertToBasis(ValueType totalValue);
        /// <summary>
        /// Преобразует бизис в массив для передачи в сеть
        /// </summary>
        public abstract Byte[] ToArray(UInt32 basis);
        /// <summary>
        /// Преобразует массив байт в базис 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Basis of value</returns>
        public abstract UInt32 ConvertFromArray(Byte[] array);

        #endregion

        #region IXmlSerializable Members

        public virtual System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public virtual void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
