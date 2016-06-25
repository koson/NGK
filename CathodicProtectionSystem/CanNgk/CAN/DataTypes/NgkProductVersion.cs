using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// Структура для харнения верисии продкуции НГК.
    /// </summary>
    /// <remarks>
    /// Версия формируется из двух частей: мажорная и минорная. Имеет следующий формат:
    /// DDD.DD , где целая часть - Мажорная версия, а дробная - Минорная версия;
    /// Мажорная часть формируется: 100 * текущий номер. Диапазон допустимых значений 100…65500;
    /// Минорная  часть формируется: 1 * текущий номер. Диапазон допустимых значений 1…99;
    /// </remarks>
    public struct NgkProductVersion: IEquatable<NgkProductVersion>
    {
        #region Fields And Properties
        /// <summary>
        /// Хранит версию продукта в формате DDD.DD [Major.Minor]
        /// </summary>
        private UInt16 _Version;
        /// <summary>
        /// Возвращает или устанавливает версию продукта НГК закодированную как челое
        /// число содержащее мажорную и минорную часть 
        /// </summary>
        public UInt16 TotalVersion
        {
            get { return _Version; }
            set
            {
                _Version = value;
            }
        }
        /// <summary>
        /// Возвращает или устанавливаем мажорную часть верисии
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Недопустимое значение мажорной части версии продукта НГК</exception>
        public UInt16 Major
        {
            get { return System.Convert.ToUInt16((this._Version / 100)); }
            set
            {
                String msg;
                if (value > 655)
                {
                    msg = String.Format(
                        "Недопустимое значение мажорной части версии {0}, не может быть больше 655",
                        value.ToString());
                    throw new ArgumentOutOfRangeException("Major", msg);
                }
                else
                {
                    // Очищаем мажорную часть
                    this._Version &= 0x7F; // минорная часть кодируется 7 младшими битами числа
                    this._Version += (System.Convert.ToUInt16(value * 100));
                }
            }
        }
        /// <summary>
        /// Возвращает или устанавливаем мажорную часть верисии
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Недопустимое значение минорной части версии продукта НГК</exception>
        public Byte Minor
        {
            get { return System.Convert.ToByte((this._Version % 100)); }
            set
            {
                String msg;
                if (value > 99)
                {
                    msg = String.Format(
                        "Недопустимое значение минорной части версии {0}, не может быть больше 99",
                        value.ToString());
                    throw new ArgumentOutOfRangeException("Minor", msg);
                }
                else
                {
                    // Очищаем минорную часть
                    this._Version &= 0xF80; // минорная часть кодируется 7 младшими битами числа
                    this._Version += value;
                }
            }
        }
        /// <summary>
        /// Возвращает или устанавливает значение в виде System.Version
        /// </summary>
        public Version Version
        {
            get { return new Version(Convert.ToInt32(Major), Convert.ToInt32(Minor)); }
            set 
            {
                Minor = Convert.ToByte(value.Minor);
                Major = Convert.ToUInt16(value.Major);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="versionBasis">Верисия продукта в формате DDD.DD [Major.Minor]</param>
        public NgkProductVersion(UInt16 versionBasis)
        {
            this._Version = versionBasis;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        public NgkProductVersion(Version version)
        {
            _Version = 0;
            Major = Convert.ToUInt16(version.Major);
            Minor = Convert.ToByte(version.Minor);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Возвращает версию прдукта НГК в формате вещественного числа DDD,DD
        /// </summary>
        /// <returns>Версия прдукта</returns>
        public float ToFloat()
        {
            return this._Version / 100;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}",
                this.Major.ToString("D3"),
                this.Minor.ToString("D2")); ;
        }

        public static bool operator ==(NgkProductVersion v1, NgkProductVersion v2)
        {
            return v1.TotalVersion == v2.TotalVersion;
        }

        public static bool operator !=(NgkProductVersion v1, NgkProductVersion v2)
        {
            return v1.TotalVersion != v2.TotalVersion;
        }

        public override bool Equals(object obj)
        {
            return obj is NgkProductVersion ?
                this.TotalVersion == ((NgkProductVersion)obj).TotalVersion : false;
            //return base.Equals(obj);
        }

        #endregion

        #region IEquatable<NgkProductVersion> Members

        public bool Equals(NgkProductVersion other)
        {
            return this.TotalVersion == other.TotalVersion;
        }

        #endregion
    }
}
