using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.DataLinkLayer.CanPort;
using NGK.CAN.DataLinkLayer.CanPort.Design.Controls;

//========================================================================================
namespace NGK.CAN.DataLinkLayer.CanPort.Design
{
    //====================================================================================
    /// <summary>
    /// Форма для выбора или редактировани CAN-порта
    /// </summary>
    public partial class ICanPortUITypeEditorDialogForm : Form
    {
        //--------------------------------------------------------------------------------
        #region Fields And Properties
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Объект CAN порта
        /// </summary>
        private ICanPort _ICanPort;
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Возвращает/устанавливает CAN-порт для редактирования
        /// </summary>
        public ICanPort ICanPort
        {
            get { return _ICanPort; }
            set 
            {
                _ICanPort = value;
            }
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Список узлов дерева производительей CAN-портов, которые предстваляю
        /// собой конкретные модели потов.
        /// </summary>
        private List<TreeNode> _TreeNodesOfCanPortsList;
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public ICanPortUITypeEditorDialogForm()
        {
            InitializeComponent();
            this.Load += 
                new EventHandler(EventHandler_ICanPortUITypeEditorDialogForm_Load);
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
        private void EventHandler_ICanPortUITypeEditorDialogForm_Load(object sender, EventArgs e)
        {
            float percent;

            // Настраиваем окно
            this.Icon = Properties.Resources.faviconMy;
            this.Text = "Редактор CAN-Порта";
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;

            SplitContainer splitContainer;
            splitContainer = new SplitContainer();
            splitContainer.Name = "_SplitContainerMainWindows";
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Horizontal;
            percent = splitContainer.Height / 100;
            splitContainer.Panel2MinSize = Convert.ToInt32(percent * 5);
            splitContainer.SplitterDistance = Convert.ToInt32(percent * 95);
            splitContainer.IsSplitterFixed = true;
            this.Controls.Add(splitContainer);

            splitContainer = new SplitContainer();
            splitContainer.Name = "_SplitContainerManufactures";
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Orientation = Orientation.Vertical;
            percent = splitContainer.Height / 100;
            splitContainer.Panel1MinSize = Convert.ToInt32(percent * 35);
            splitContainer.SplitterDistance = Convert.ToInt32(percent * 40);
            //splitContainer.IsSplitterFixed = false;
            ((SplitContainer)this.Controls["_SplitContainerMainWindows"]).Panel1.Controls.Add(splitContainer);

            Button button;
            button = new Button();
            button.Name = "_ButtonOK";
            button.Text = "OK";
            button.Click += new EventHandler(EventHandler_ButtonOK_Click);
            button.Dock = DockStyle.Fill;
            splitContainer = (SplitContainer)this.Controls["_SplitContainerMainWindows"];
            splitContainer.Panel2.Controls.Add(button);

            _TreeNodesOfCanPortsList = new List<TreeNode>(2);

            TreeView treeView;
            treeView = new TreeView();
            treeView.Name = "_TreeViewManufacturers";
            treeView.Dock = DockStyle.Fill;
            treeView.AfterSelect +=
                new TreeViewEventHandler(EventHandler_TreeViewManufacturers_AfterSelect);
            splitContainer = (SplitContainer)this.Controls["_SplitContainerMainWindows"];
            splitContainer = (SplitContainer)splitContainer.Panel1.Controls["_SplitContainerManufactures"];
            splitContainer.Panel1.Controls.Add(treeView);

            TreeNode node;
            node = new TreeNode();
            node.Name = "_TreeNodeTop";
            node.Text = "Производители";
            treeView.Nodes.Add(node);
            treeView.TopNode = node;

            node = new TreeNode();
            node.Name = "_TreeNodeIXXATGmb";
            node.Text = "IXXATGmb";
            treeView.TopNode.Nodes.Add(node);
            // Загружаем данный узел в список моделей портов
            this._TreeNodesOfCanPortsList.Add(node);

            node = new TreeNode();
            node.Name = "_TreeNodeFastwel";
            node.Text = "Fastwel";
            treeView.TopNode.Nodes.Add(node);

            node = new TreeNode();
            node.Name = "TreeNodeFastwelNim351";
            node.Text = "NIM-351";
            (treeView.TopNode.Nodes["_TreeNodeFastwel"]).Nodes.Add(node);
            // Загружаем данный узел в список моделей портов
            this._TreeNodesOfCanPortsList.Add(node);


            splitContainer = (SplitContainer)this.Controls["_SplitContainerMainWindows"];
            splitContainer = (SplitContainer)splitContainer.Panel1.Controls["_SplitContainerManufactures"];
            Panel settingsPanel = splitContainer.Panel2;

            if (this._ICanPort != null)
            {
                if (_ICanPort is IXXAT.CanPort)
                {
                    Design.Controls.IXXATCanPortTuner ixxatTuner =
                        new Design.Controls.IXXATCanPortTuner();
                    ixxatTuner.CanPort = this._ICanPort as IXXAT.CanPort;
                    ixxatTuner.Dock = DockStyle.Fill;
                    settingsPanel.Controls.Clear();
                    settingsPanel.Controls.Add(ixxatTuner);
                }
                else if (_ICanPort is Fastwel.NIM351.CanPort)
                {
                    Design.Controls.FastwelNIM351PortTuner nim351Tuner =
                        new FastwelNIM351PortTuner();
                    nim351Tuner.CanPort = this._ICanPort as CanPort.Fastwel.NIM351.CanPort;
                    nim351Tuner.Dock = DockStyle.Fill;
                    settingsPanel.Controls.Clear();
                    settingsPanel.Controls.Add(nim351Tuner);
                }
                else
                {
                    throw new ArgumentException("Данный тип CAN-порта не поддерживается");
                }
            }
            return;
        } 
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события нажатия кнопки "OK"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            SplitContainer splitContainer = (SplitContainer)this.Controls["_SplitContainerMainWindows"];
            splitContainer = (SplitContainer)splitContainer.Panel1.Controls["_SplitContainerManufactures"];
            Panel settingsPanel = splitContainer.Panel2;

            foreach (System.Windows.Forms.Control control in settingsPanel.Controls)
            {
                if (control is Design.Controls.ICanPortEditor)
                {
                    Design.Controls.ICanPortEditor editor = (Design.Controls.ICanPortEditor)control;
                    this.ICanPort = editor.CanPort;
                }

                //if (control is Design.Controls.IXXATCanPortTuner)
                //{
                //    Design.Controls.IXXATCanPortTuner ixxatTnr = 
                //        (Design.Controls.IXXATCanPortTuner)control;
                //    this._ICanPort = (ICanPort)ixxatTnr.CanPort;
                //}
                //else if (control is Design.Controls.FastwelNIM351PortTuner)
                //{
                //    Design.Controls.FastwelNIM351PortTuner nim351Tnr =
                //        (Design.Controls.FastwelNIM351PortTuner)control;
                //    this._ICanPort = (ICanPort)nim351Tnr.CanPort;
                //}
            } 


            this.Close();
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Обработчик события выбора узла дерева производителей CAN-портов. Если узел
        /// представляет конкретную модель CAN порта, то отображаем для него настройки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TreeViewManufacturers_AfterSelect(
            object sender, TreeViewEventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            
            if ((e.Action == TreeViewAction.ByKeyboard) || (e.Action == TreeViewAction.ByMouse))
            {
                // Ищем данный узел в списке моделей портов и если находим, то выводим
                // для него найстройки
                this.ShowPortSettings(e.Node);
            }
            return;
        }
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Метод ищёт в списке моделей портов данный узел и если находит отображает
        /// для него настройки. Если не находит ничего не делает
        /// </summary>
        /// <param name="node">Узел с моделью CAN-порта</param>
        private void ShowPortSettings(TreeNode node)
        {
            SplitContainer splitContainer = (SplitContainer)this.Controls["_SplitContainerMainWindows"];
            splitContainer = (SplitContainer)splitContainer.Panel1.Controls["_SplitContainerManufactures"];
            Panel settingsPanel = splitContainer.Panel2;

            // Ищем узел в списке
            int index = this._TreeNodesOfCanPortsList.IndexOf(node);
            
            if (index >= 0)
            {
                // Узел найден в списке, отображаем настройки для данного типа оборудования
                switch (node.Text)
                {
                    case "IXXATGmb":
                        {
                            Design.Controls.IXXATCanPortTuner ixxatTuner = 
                                new Design.Controls.IXXATCanPortTuner();
                            
                            if (this._ICanPort is IXXAT.CanPort)
                            {
                                ixxatTuner.CanPort = this._ICanPort as IXXAT.CanPort;
                            }

                            ixxatTuner.Dock = DockStyle.Fill;

                            settingsPanel.Controls.Clear();
                            settingsPanel.Controls.Add(ixxatTuner);
                            break; 
                        }
                    case "NIM-351":
                        {
                            Design.Controls.FastwelNIM351PortTuner nim351Tuner =
                                new FastwelNIM351PortTuner();
                            
                            if (this._ICanPort is CanPort.Fastwel.NIM351.CanPort)
                            {
                                nim351Tuner.CanPort = this._ICanPort as CanPort.Fastwel.NIM351.CanPort;
                            }

                            nim351Tuner.Dock = DockStyle.Fill;

                            settingsPanel.Controls.Clear();
                            settingsPanel.Controls.Add(nim351Tuner);
                            break; 
                        }
                    default:
                        {
                            throw new Exception(
                                "Найден узел с портом, который не поддерживается данным методом");
                        }
                }
            }
        }
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
        #region Events
        //--------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File