using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;
using System.ComponentModel;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// Класс формирует сводную таблицу значений объектов словаря каждого устройства сети
    /// (только для КИП). Следит за изменениями этих параметров.
    /// </summary>
    public class ParametersPivotTable
    {
        #region Constructors

        public ParametersPivotTable(BindingList<NgkCanDevice> devices)
        {
            _Devices = devices;
            
            Devices = new BindingList<IDeviceSummaryParameters>();
            
            foreach (NgkCanDevice device in devices)
            {
                Devices.Add(device);
            }

            //InitTable();
        }

        #endregion

        #region Fields And Properties

        /// <summary>
        /// Сводная таблица
        /// </summary>
        private DataTable _PivotTable;
        /// <summary>
        /// Сводная таблица
        /// </summary>
        public DataTable PivotTable
        {
            get { return _PivotTable; }
            set { _PivotTable = value; }
        }

        /// <summary>
        /// Список устройств в сети относящихся к КИП 
        /// </summary>
        private BindingList<NgkCanDevice> _Devices;

        public readonly BindingList<IDeviceSummaryParameters> Devices;

        #endregion

        #region Methods
        /// <summary>
        /// Обновляет данные в сводной таблице
        /// </summary>
        //public void Update()
        //{
        //    for (int i = 0; i < _Devices.Count; i++)
        //    {
        //        UpdateDivice(_Devices[i]);
        //    }
        //    // Генерируем событие
        //    OnTableWasUpdated();
        //}
        /// <summary>
        /// Обновляет данные из указанного устройства
        /// </summary>
        /// <param name="device">Сетевое устройство</param>
        //private void UpdateDivice(NgkCanDevice device)
        //{
        //    DataRow row;

        //    row = null;

        //    if (device == null)
        //    {
        //        return;
        //    }

        //    // Ищем строку с указанным устройством
        //    foreach (DataRow item in _PivotTable.Rows)
        //    {
        //        if (device.NodeId == System.Convert.ToByte(item["NodeId"]))
        //        {
        //            // Строка с нужным устройством найдена
        //            row = item;
        //            break;
        //        }
        //    }
        //    //index = 0x2008;
        //    row["PolarisationPotential_2008"] = (bool)device.Parameters["polarisation_pot_en"].Value ? 
        //        device.Parameters["polarization_pot"].Value : DBNull.Value;  
        //    //index = 0x2009;
        //    row["ProtectionPotential_2009"] = (bool)device.Parameters["protection_pot_en"].Value ?
        //        device.Parameters["protection_pot"].Value : DBNull.Value;
        //    //index = 0x200B;
        //    row["ProtectionCurrent_200B"] = (bool)device.Parameters["protection_cur_en"].Value ?
        //        device.Parameters["protection_cur"].Value : DBNull.Value;
        //    //index = 0x200C;
        //    row["PolarisationCurrent_200С"] = (bool)device.Parameters["polarisation_cur_en"].Value ?
        //        device.Parameters["polarization_cur"].Value : DBNull.Value;
        //    //index = 0x200F;
        //    row["Corrosion_depth_200F"] = device.Parameters["corrosion_depth"].Value;
        //    //index = 0x2010;
        //    row["Corrosion_speed_2010"] = device.Parameters["corrosion_speed"].Value;
        //    //index = 0x2015;
        //    row["Tamper_2015"] = device.Parameters["tamper"].Value; 
        //}
        /// <summary>
        /// Инициализирует структуру сводной таблицы (т.е. создаются столбцы соотвествующие
        /// индексам объектов устройств)
        /// </summary>
        //private void InitTable()
        //{
        //    DataColumn column;

        //    _PivotTable = new DataTable("PitovTable");

        //    column = new DataColumn();
        //    column.ColumnName = "NodeId";
        //    column.AllowDBNull = false;
        //    column.Caption = "Сетевой адрес";
        //    column.DataType = typeof(Byte);
        //    column.ReadOnly = false;
        //    column.Unique = true;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "Location";
        //    column.AllowDBNull = false;
        //    column.Caption = "Расположение";
        //    column.DataType = typeof(String);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = String.Empty;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "PolarisationPotential_2008";
        //    column.AllowDBNull = true; // Null - устанавливается если данное измерение отключено в КИП 
        //    column.Caption = "Поляризационный потенциал, B";
        //    column.DataType = typeof(float);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "ProtectionPotential_2009";
        //    column.AllowDBNull = true; // Null - устанавливается если данное измерение отключено в КИП
        //    column.Caption = "Защитный потенциал, B";
        //    column.DataType = typeof(float);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "ProtectionCurrent_200B";
        //    column.AllowDBNull = true; // Null - устанавливается если данное измерение отключено в КИП
        //    column.Caption = "Ток катодной защиты, A";
        //    column.DataType = typeof(float);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "PolarisationCurrent_200С";
        //    column.AllowDBNull = true; // Null - устанавливается если данное измерение отключено в КИП
        //    column.Caption = "Ток поляризации, mA";
        //    column.DataType = typeof(float);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "Corrosion_depth_200F";
        //    column.AllowDBNull = false;
        //    column.Caption = "Глубина коррозии, мкм";
        //    column.DataType = typeof(UInt16);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "Corrosion_speed_2010";
        //    column.AllowDBNull = false;
        //    column.Caption = "Скорость коррозии, мкм/год";
        //    column.DataType = typeof(UInt16);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = 0;
        //    _PivotTable.Columns.Add(column);

        //    column = new DataColumn();
        //    column.ColumnName = "Tamper_2015";
        //    column.AllowDBNull = false;
        //    column.Caption = "Вскрытие";
        //    column.DataType = typeof(Boolean);
        //    column.ReadOnly = false;
        //    column.Unique = false;
        //    column.DefaultValue = false;
        //    _PivotTable.Columns.Add(column);

        //    // Добавляем строки устройств в таблицу
        //    foreach (NgkCanDevice device in _Devices)
        //    {
        //        DataRow row = _PivotTable.NewRow();
        //        row["NodeId"] = device.NodeId;
        //        row["Location"] = device.Location;
        //        _PivotTable.Rows.Add(row);
        //    }

        //    Update();

        //    return;
        //}
        /// <summary>
        /// Генерирует событие обновления данных в сводной таблице
        /// </summary>
        //private void OnTableWasUpdated()
        //{
        //    if (this.TableWasUpdated != null)
        //    {
        //        this.TableWasUpdated(this, new EventArgs());
        //    }
        //    return;
        //}

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при обновлении таблицы.
        /// </summary>
        public event EventHandler TableWasUpdated;
        
        #endregion
    }
}