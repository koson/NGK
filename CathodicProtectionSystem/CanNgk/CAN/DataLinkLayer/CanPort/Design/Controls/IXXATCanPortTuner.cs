using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.DataLinkLayer.CanPort.IXXAT;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Design.Controls
{
    //====================================================================================
    public sealed partial class IXXATCanPortTuner : UserControl, ICanPortEditor
    {
        //--------------------------------------------------------------------------------
        #region Fields And Properties
        //--------------------------------------------------------------------------------
        /// <summary>
        /// CAN-порт производства IXXAT Gmb
        /// </summary>
        private CanPort.IXXAT.CanPort _CanPort;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// CAN-порт
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [DisplayName("CAN-порт")]
        [Category("Аппаратура")]
        [Description("CAN порт производства  IXXAT Automation GmbH")]
        public CanPort.IXXAT.CanPort CanPort
        {
            get { return _CanPort; }
            set { _CanPort = value; }
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор
        /// </summary>
        public IXXATCanPortTuner()
        {
            InitializeComponent();
            this.Load += new EventHandler(EventHandler_IXXATCanPortTuner_Load);
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_IXXATCanPortTuner_Load(object sender, EventArgs e)
        {
            // Выводим список доступных устройств
            this.UpdateListOfDevices();

            this.treeViewAdapters.AfterSelect += 
                new TreeViewEventHandler(EventHandler_treeViewAdapters_AfterSelect);
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события после выбора узла дерева CAN-адаптеров
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_treeViewAdapters_AfterSelect(
            object sender, TreeViewEventArgs e)
        {
            TreeView treeView = (TreeView)sender;

            if ((e.Action == TreeViewAction.ByKeyboard) || (e.Action == TreeViewAction.ByMouse))
            {
                // Получаем выдленный node
                if (e.Node != treeView.TopNode)
                {
                    // Отображаем свойсвта устройства
                    this.ShowDeviceProperties(e.Node.Text, ref this.propertyGridAdapter);
                }
            }
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Обновляет список подключенных устройств IXXAT
        /// </summary>
        private void UpdateListOfDevices()
        {
            this.InitTreeView(ref this.treeViewAdapters);
            this.treeViewAdapters.ExpandAll();
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Инициализирует дерево, записывая в него все найденные 
        /// устройства IXXAT
        /// </summary>
        /// <param name="control"></param>
        private void InitTreeView(ref TreeView control)
        {
            TreeNode node;
            // Инициализируем дерево
            // Получаем список устройств
            string[] devices = IXXAT.CanPort.GetDevices();

            // Очищаем дерево
            control.Nodes.Clear();
            control.TopNode = null;
            // Создаём корневой элемент дерева
            node = new TreeNode();
            node.Name = "NodeRoot";
            node.Text = "Устройства";
            control.Nodes.Add(node);
            control.TopNode = node;

            for (int i = 0; i < devices.Length; i++)
            {
                node = new TreeNode();
                node.Name = devices[i];
                node.Text = devices[i];
                //node.ContextMenuStrip = this.contextMenuStripDevice;
                control.TopNode.Nodes.Add(node);
            }
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Отображает свойства адаптера CAN-порта в указаном контроле
        /// </summary>
        /// <param name="serialNumber">Уникальный серийный номер адаптера</param>
        /// <param name="control">Контрол для отображения настроек адаптера</param>
        private void ShowDeviceProperties(String serialNumber, ref PropertyGrid control)
        {
            String sn;
            Object obj;
            DeviceInfo[] devicelist;
            // Получили описания на оборудование
            devicelist = IXXAT.CanPort.GetDevicesInfo();
            // Находим нужное устройство и отображаем данные
            foreach (DeviceInfo item in devicelist)
            {
                obj = item.UniqueHardwareId;
                sn = IXXAT.CanPort.GetSerialNumber(ref obj);
                if (sn == serialNumber)
                {
                    IXXAT.CanPort port = new IXXAT.CanPort(serialNumber);
                    this._CanPort = port;
                    control.SelectedObject = null;
                    control.SelectedObject = port;
                }
            }
            return;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region ICanPortEditor Members
        //--------------------------------------------------------------------------------
        ICanPort ICanPortEditor.CanPort
        {
            get
            {
                return (ICanPort)this._CanPort;
            }
            set
            {
                if (value is IXXAT.CanPort)
                {
                    this.CanPort = (IXXAT.CanPort)value;
                }
                else
                {
                    // Это CAN-порт другого производителя
                    this.CanPort = null;
                }
            }
        }
        //--------------------------------------------------------------------------------
        bool ICanPortEditor.CanEditCanPort(ICanPort canPort)
        {
            if (canPort is IXXAT.CanPort)
            {
                return true;
            }
            else
            {
                // Это CAN-порт другого производителя
                return false;
            }
        }
        //--------------------------------------------------------------------------------
        #endregion
    }
    //====================================================================================
}
//========================================================================================
// End Of File