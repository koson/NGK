using System;
using System.Collections.Generic;
using System.Text;

//===================================================================================
namespace Modbus.OSIModel.ApplicationLayer.Slave.DataModel.DataTypes
{
    //===============================================================================
    /// <summary>
    /// Класс для реализации файла модели данных modbus
    /// </summary>
    public class File
    {
        //---------------------------------------------------------------------------
        #region Fields and Properties
        //---------------------------------------------------------------------------
        /// <summary>
        /// Номер файла
        /// </summary>
        private UInt16 _Number;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Номер файла
        /// </summary>
        public UInt16 Number
        {
            get { return this._Number; }
            set 
            {
                if (value != 0)
                {
                    this._Number = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Number", "Номер файла не может быть равен 0");
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Коллекция записей содержащаяся в данном файле
        /// </summary>
        private RecordsCollection _RecordsCollection;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Записи в файле
        /// </summary>
        public RecordsCollection Records
        {
            get { return this._RecordsCollection; }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание данного файла
        /// </summary>
        private String _Description;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Описание данного файла
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set 
            {
                if (value == null)
                {
                    this._Description = String.Empty;
                }
                else
                {
                    this._Description = value;
                }
            }
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Устройство владелец данного файла
        /// </summary>
        private Device _Device;
        //---------------------------------------------------------------------------
        /// <summary>
        /// Возвращает устройство, которому принадлежит данный файл
        /// </summary>
        public Device Device
        {
            get { return this._Device; }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Constructors
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public File()
        {
            this._Number = 0;
            this._Description = String.Empty;
            this._Device = null;
            this._RecordsCollection = new RecordsCollection();
        }
        //---------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="number">Номер файла</param>
        /// <param name="description">Описание файла</param>
        public File(UInt16 number, String description)
        {
            this.Number = number;

            if (description == null)
            {
                this._Description = String.Empty;
            }
            else
            {
                this._Description = description;
            }

            this._Device = null;
            this._RecordsCollection = new RecordsCollection();
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
        #region Methods
        //---------------------------------------------------------------------------
        /// <summary>
        /// Метод вызывается при добавлении в коллекцию, для установки свойства
        /// _Device. Данный объект modbus-устройства, является владельцем данной
        /// коллекции регистров-хранения. Если владелец не равен null, то данная
        /// коллекция уже принадлежит другой коллекции. При это вызывается
        /// исключение
        /// </summary>
        /// <param name="owner">Владелец данного файла</param>
        internal void SetOwner(Device owner)
        {
            if (this._Device == null)
            {
                this._Device = owner;
                this._RecordsCollection.SetOwner(owner);
            }
            else
            {
                if (owner == null)
                {
                    // Освобождаем параметр от владельца
                    this._Device = owner;
                }
                else
                {
                    // Если устройство, которому принадлежит данный файл
                    // эквивалентен устанавливаемому, тогда ничего не делаем. 
                    // Здесь нет ошибки. В противном случае, генерируем исключение
                    if (this._Device.Equals(owner) == false)
                    {
                        throw new InvalidOperationException(
                            "Данный файл уже принадлежит другому устройству");
                    }
                }
            }
        }
        //---------------------------------------------------------------------------
        #endregion
        //---------------------------------------------------------------------------
    }
    //===============================================================================
}
//===================================================================================
// End of file