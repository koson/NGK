using System;
using System.Text;

//==================================================================================
namespace NGK.Devices
{
    //==============================================================================
    /// <summary>
    /// Делегат для создания события наступающего перед изменением какого-либо
    /// свойства и посзоляющего отменить это изменение
    /// </summary>
    /// <param name="sender">Объект отправитель события</param>
    /// <param name="args">Аргументы события</param>
    public delegate void PropertyChangingEventHandler(Object sender, PropertyChangingEventArgs args);
    //==============================================================================
    /// <summary>
    /// Аргументы при событии PropertyChangingEventHandler
    /// </summary>
    public class PropertyChangingEventArgs : System.ComponentModel.PropertyChangingEventArgs
    {
        //--------------------------------------------------------------------------
        /// <summary>
        /// Поле хранит
        /// </summary>
        private Boolean _Cancel = false;
        //--------------------------------------------------------------------------
        /// <summary>
        /// Позволяет отменить измениение значение свойства если установить в true
        /// </summary>
        public Boolean Cancel
        {
            get { return _Cancel; }
            set { _Cancel = value; }
        }
        //--------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="propertyName">Наименование изменияемого свойства</param>
        public PropertyChangingEventArgs(String propertyName)
            : base(propertyName)
        { }
        //--------------------------------------------------------------------------
    }
    //==============================================================================
}
//==================================================================================
// End of file