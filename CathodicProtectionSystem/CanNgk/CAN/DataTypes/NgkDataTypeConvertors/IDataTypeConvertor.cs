using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Базовый класс для создания типов данных
    /// </summary>
    public interface ICanDataTypeConvertor: IXmlSerializable
    {
        #region Fields And Properties

        /// <summary>
        /// Множитель числа. Задаёт маштаб числа
        /// (См struct Scaler)
        /// </summary>
        decimal Scaler { get; }
        
        /// <summary>
        /// Число знаковое или беззнаковое
        /// </summary>
        Boolean Signed { get; }
        
        /// <summary>
        /// Булев тип
        /// </summary>
        Boolean IsBoolean { get; }

        /// <summary>
        /// Тип данных TotalValue
        /// </summary>
        Type OutputDataType { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Преобразует базовое число (значение передаваемое по сети) 
        /// в конечное число 
        /// </summary>
        /// <param name="value">Неопределённое значение полученное в ответе  
        /// (знаковое/беззнаковое, целое/вещественное)</param>
        /// <returns></returns>
        ValueType ConvertToOutputValue(UInt32 basis);
        /// <summary>
        /// Преобразует значение в базис для отправки по сети 
        /// или для хранения в словаре объектов
        /// </summary>
        /// <param name="totalValue"></param>
        /// <returns></returns>
        UInt32 ConvertToBasis(ValueType outputValue);
        /// <summary>
        /// Преобразует бизис в массив для передачи в сеть
        /// </summary>
        Byte[] ToArray(UInt32 basis);
        /// <summary>
        /// Преобразует массив байт в базис 
        /// </summary>
        /// <param name="array"></param>
        /// <returns>Basis of value</returns>
        UInt32 ConvertFromArray(Byte[] array);

        #endregion
    }
}
