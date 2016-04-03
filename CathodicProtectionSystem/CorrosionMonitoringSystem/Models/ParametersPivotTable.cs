using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
//
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.DataTypes;
using NGK.CAN.ApplicationLayer.Network.Devices;
using NGK.CAN.ApplicationLayer.Network.Devices.ObjectDictionary;

namespace NGK.CorrosionMonitoringSystem.Models
{
    /// <summary>
    /// Класс формирует сводную таблицу значений объектов словаря каждого устройства сети
    /// (только для КИП). Следит за изменениями этих параметров.
    /// </summary>
    public class ParametersPivotTable
    {
        #region Constructors
        
        public ParametersPivotTable(DeviceBase[] devices)
        {
            // Создаём таблицу
            this.InitTable();
            // Формируем списко КИП-ов
            this.InitKipList(devices);
            return;
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
        private DeviceBase[] _KipList;

        #endregion

        #region Methods
        /// <summary>
        /// Обновляет данные в сводной таблице
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < _KipList.Length; i++)
			{
                UpdateDivice(_KipList[i]);
			}
            // Генерируем событие
            this.OnTableWasUpdated();

            return;
        }
        /// <summary>
        /// Инициализирует список устройств типа КИП из устройств доступных в сети
        /// </summary>
        private void InitKipList(DeviceBase[] devices)
        {
            DeviceType deviceType;
            DataRow row;

            List<DeviceBase> result = new List<DeviceBase>();

            if (devices != null)
            {                
                for (int i = 0; i < devices.Length; i++)
                {
                    deviceType = devices[i].DeviceType;
                    
                    if ((deviceType == DeviceType.KIP_BATTERY_POWER_v1) ||
                        (deviceType == DeviceType.KIP_MAIN_POWERED_v1))
                    {
                        result.Add(devices[i]);
                        //devices[i].DataWasChanged += 
                        //    new EventHandler(EventHandler_Device_DataWasChanged);
                        row = this._PivotTable.NewRow();
                        row["NodeId"] = devices[i].NodeId;
                        row["Location"] = devices[i].LocationName;
                        this._PivotTable.Rows.Add(row);
                        // Обновляем данные
                        //this.UpdateDivice(devices[i]);
                    }
                }
                this._KipList = result.ToArray();
            }
            else
            {
                throw new NullReferenceException();
            }
            return;
        }
        /// <summary>
        /// Обновляет данные из указанного устройства
        /// </summary>
        /// <param name="device">Сетевое устройство</param>
        private void UpdateDivice(DeviceBase device)
        {
            Boolean result;
            UInt16 index;
            DataRow row;

            row = null;

            if (device == null)
            {
                return;
            }

            // Ищем строку с указанным устройством
            foreach (DataRow item in this._PivotTable.Rows)
            {
                if (device.NodeId == System.Convert.ToByte(item["NodeId"]))
                {
                    // Строка с нужным устройством найдена
                    row = item;
                    break;
                }
            }

            index = 0x2008;
            row["PolarisationPotential_2008"] = device.GetObject(index);
            index = 0x2009;
            row["ProtectionPotential_2009"] = device.GetObject(index);
            index = 0x200B;
            row["ProtectionCurrent_200B"] = device.GetObject(index);
            index = 0x200C;
            row["PolarisationCurrent_200С"] = device.GetObject(index);
            index = 0x200F;
            row["Corrosion_depth_200F"] = device.GetObject(index);
            index = 0x2010;
            row["Corrosion_speed_2010"] = device.GetObject(index);
            index = 0x2015;
            row["Tamper_2015"] = device.GetObject(index);
        }
        /// <summary>
        /// Обработчик события изменения данных устройства из списка.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void EventHandler_Device_DataWasChanged(object sender, EventArgs e)
        //{
        //    IDevice device = (IDevice)sender;

        //    // Обновляем данные в сводной таблице
        //    this.UpdateDivice(device);
        //    return;
        //}
        /// <summary>
        /// Инициализирует структуру сводной таблицы (т.е. создаются столбцы соотвествующие
        /// индексам объектов устройств)
        /// </summary>
        private void InitTable()
        {
            DataColumn column;
            
            this._PivotTable = new DataTable("PitovTable");

            column = new DataColumn();
            column.ColumnName = "NodeId";
            column.AllowDBNull = false;
            column.Caption = "Сетевой адрес";
            column.DataType = typeof(Byte);
            column.ReadOnly = false;
            column.Unique = true;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "Location";
            column.AllowDBNull = false;
            column.Caption = "Расположение";
            column.DataType = typeof(String);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = String.Empty;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "PolarisationPotential_2008";
            column.AllowDBNull = false;
            column.Caption = "Поляризационный потенциал, B";
            column.DataType = typeof(float);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "ProtectionPotential_2009";
            column.AllowDBNull = false;
            column.Caption = "Защитный потенциал, B";
            column.DataType = typeof(float);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "ProtectionCurrent_200B";
            column.AllowDBNull = false;
            column.Caption = "Ток катодной защиты, A";
            column.DataType = typeof(float);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "PolarisationCurrent_200С";
            column.AllowDBNull = false;
            column.Caption = "Ток поляризации, mA";
            column.DataType = typeof(float);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "Corrosion_depth_200F";
            column.AllowDBNull = false;
            column.Caption = "Глубина коррозии, мкм";
            column.DataType = typeof(UInt16);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "Corrosion_speed_2010";
            column.AllowDBNull = false;
            column.Caption = "Скорость коррозии, мкм/год";
            column.DataType = typeof(UInt16);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = 0;
            this._PivotTable.Columns.Add(column);

            column = new DataColumn();
            column.ColumnName = "Tamper_2015";
            column.AllowDBNull = false;
            column.Caption = "Вскрытие";
            column.DataType = typeof(Boolean);
            column.ReadOnly = false;
            column.Unique = false;
            column.DefaultValue = false;
            this._PivotTable.Columns.Add(column);

            return;
        }
        /// <summary>
        /// Генерирует событие обновления данных в сводной таблице
        /// </summary>
        private void OnTableWasUpdated()
        {
            if (this.TableWasUpdated != null)
            {
                this.TableWasUpdated(this, new EventArgs());
            }
            return;
        }

        #endregion

        #region Events
        /// <summary>
        /// Событие происходит при обновлении таблицы.
        /// </summary>
        public event EventHandler TableWasUpdated;
        
        #endregion
    }// End Of Class
}// End Of Namespace