using System;
using System.Collections.Generic;
using System.Text;

namespace SystemParametersRecorder.Models
{
    /// <summary>
    /// Параметр устройства для сохранения в БД
    /// </summary>
    public abstract class Parameter<T> where T: struct
    {
        #region Fields And Propeties

        private T _Value;
        /// <summary>
        /// Набор фильтров при срабатывании которых происходит запись
        /// значения в БД.
        /// </summary>
        private List<Filter> _Filters;

        public T Value 
        { 
            get { return _Value; }
            set { _Value = value; }
        }

        #endregion 
    }
}
