using System;
using System.Collections.Generic;
using System.Text;
using NGK.CAN.ApplicationLayer.Network.Devices.Profiles.ObjectDictionary;

namespace NGK.CorrosionMonitoringSystem.Models
{
    public class CompositeParameter
    {
        #region Fields And Properties

        Dictionary<UInt16, Object> _Objects;
        public Dictionary<UInt16, Object> Objects 
        { 
            get { return _Objects; } 
        }

        public UInt16[] Indexes
        {
            get 
            {
                UInt16[] array = new ushort[_Objects.Keys.Count];
                _Objects.Keys.CopyTo(array, 0);
                return array; 
            }
        }

        IObjectsCombiner _Combiner;

        Object _Value;
        public Object Value
        {
            get { return _Value; }
            private set { _Value = value; } 
        }

        #endregion

        public void Combine()
        {
            if (_Combiner != null)
                Value = _Combiner.Combine(_Objects);
            else
                Value = _Objects[Indexes[0]];
        }

        public bool SetObjectValue(UInt16 index, Object value)
        {
            if (_Objects.ContainsKey(index))
            {
                if (_Objects[index].GetType() == value.GetType())
                {
                    _Objects[index] = value;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="description"></param>
        /// <param name="displayedName"></param>
        /// <param name="info"></param>
        public CompositeParameter(String parameterName, String description,
            String displayedName, String measureUnit, ObjectInfo[] objectInfos, IObjectsCombiner combiner)
        {
            _Combiner = combiner;

            if ((objectInfos.Length > 0) && (_Combiner == null))
            {
                throw new ArgumentNullException("combiner", String.Format(
                    "�������� �������� {0}. IObjectsCombiner �� ����� ���� null, " +
                    "����� �������� ������� ����� ��� ������ ������� �������", parameterName)); 
            }

            _Objects = new Dictionary<ushort, object>();

            foreach (ObjectInfo info in objectInfos)
            {
                _Objects.Add(info.Index, Activator.CreateInstance(info.DataTypeConvertor.OutputDataType));
            }

            //_Name = parameterName;
            //_Description = description;
            //_Visible = info.Visible;
            //_DisplayedName = displayedName;
            //_MeasureUnit = measureUnit == null ? String.Empty : measureUnit;
            //_Category = info.Category;
            //_Modified = DateTime.Now;
            //_Status = ObjectStatus.NoError;
        }
    }
}
