using System;
using System.Collections.Generic;
using System.Text;

namespace SystemParametersRecorder.Models
{
    /// <summary>
    /// �������� ���������� ��� ���������� � ��
    /// </summary>
    public abstract class Parameter<T> where T: struct
    {
        #region Fields And Propeties

        private T _Value;
        /// <summary>
        /// ����� �������� ��� ������������ ������� ���������� ������
        /// �������� � ��.
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
