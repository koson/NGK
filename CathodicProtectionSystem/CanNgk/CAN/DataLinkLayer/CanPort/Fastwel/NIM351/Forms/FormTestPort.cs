using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351;
using NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Driver;
using NGK.CAN.DataLinkLayer.Message;

//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
namespace NGK.CAN.DataLinkLayer.CanPort.Fastwel.NIM351.Forms
{
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    public partial class FormTestPort : Form
    {
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Fields And Properties
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// CanPort NIM-351
        /// </summary>
        private CanPort _CanPort;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Главная панель меню формы
        /// </summary>
        private MenuStrip _MenuStripMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Файл"
        /// </summary>
        private ToolStripMenuItem _MenuFile;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Файл"->"Выход"
        /// </summary>
        private ToolStripLabel _MenuFileExit;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"
        /// </summary>
        private ToolStripMenuItem _MenuPort;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Открыть"
        /// </summary>
        private ToolStripLabel _MenuPortOpen;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Закрыть"
        /// </summary>
        private ToolStripLabel _MenuPortClose;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Подключиться к шине"
        /// </summary>
        private ToolStripLabel _MenuPortActiveLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Отключиться от шины"
        /// </summary>
        private ToolStripLabel _MenuPortPassiveLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Reset состояние Init"
        /// </summary>
        private ToolStripLabel _MenuPortResetLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Порт"->"Отправить сообщение"
        /// </summary>
        private ToolStripLabel _MenuPortSendMessage; 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Помощь"
        /// </summary>
        private ToolStripMenuItem _MenuHelp;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Меню "Помощь"->"О программе"
        /// </summary>
        private ToolStripLabel _MenuHelpAbout;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контейнер делит окна на верхнею и нижнию панель
        /// </summary>
        private SplitContainer _SplitContainerMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контейнер для верхнего окна
        /// </summary>
        private SplitContainer _SplitContainerLeftPanel;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контейнер для нижнего окна
        /// </summary>
        private SplitContainer _SplitContainerRightPanel;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Строка состояния главного окна
        /// </summary>
        private StatusStrip _StatusStripMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Дерево системных объектов
        /// </summary>
        private TreeView _TreeViewSystem;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Корневой узел _TreeViewSystem  
        /// </summary>
        private TreeNode _TreeNodeRoot;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Узел _TreeViewSystem->"Настройки порта"
        /// </summary>
        private TreeNode _TreeNodePortSettings;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Узел _TreeViewSystem->"Статистика порта"
        /// </summary>
        private TreeNode _TreeNodeStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Грид для отображения входящих сообений в CAN-порт из сети
        /// </summary>
        private DataGridView _DataGridViewIncomingMessages;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Грид для отображения исходящих сообщений в сеть через CAN-порт
        /// </summary>
        private DataGridView _DataGridViewOutcomingMessages;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контрол для отображения настроек порта
        /// </summary>
        private PropertyGrid _PropertyGridPortSettings;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контрол для отображения статистических данных порта
        /// </summary>
        private DataGridView _DataGridViewStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контрол для вывода информации в правой панели верхнего окна
        /// </summary>
        private Label _LabelInformation;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Таймер для общего назначения.
        /// </summary>
        private System.Windows.Forms.Timer _Timer;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контекстное меню для грида входящих сообщений _DataGridViewIncomingMessages
        /// </summary>
        private ContextMenuStrip _ContextMenuStripIncomingBox;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Пункт контектсного меню _ContextMenuStripIncomingBox
        /// </summary>
        private ToolStripMenuItem _ContextMenuIncomingBoxClear;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Контекстного меню грида _DataGridViewStatistics
        /// </summary>
        private ContextMenuStrip _ContextMenuStripStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Пункт контекстного меню _ContextMenuStripStatistics
        /// </summary>
        private ToolStripMenuItem _ContexMenuStatisticsClear;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Constructors
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        private FormTestPort()
        {
            //InitializeComponent();
            //this.StartPosition = FormStartPosition.CenterScreen;
            //this.Load += new EventHandler(EventHandler_FormTestPort_Load);
            throw new NotImplementedException("Вызов запрещённого конструктора по умолчанию");
        }

        public FormTestPort(CanPort port)
        {
            InitializeComponent();

            if (port == null)
            {
                throw new NullReferenceException("Переданный параметр CanPort имеет нулевую ссылку");
            }
            else
            {
                _CanPort = port;
            }

            this.StartPosition = FormStartPosition.CenterScreen;
            this.Load += new EventHandler(EventHandler_FormTestPort_Load);
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #region Methods
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события загрузки формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormTestPort_Load(object sender, EventArgs e)
        {
            ToolStripSeparator separator;
            DataGridViewColumn column;
            DataGridViewRow row;
            DataGridViewCheckBoxColumn chxColumn;
            DataGridViewCheckBoxCell chxCell;

            this.Text = "CAN-порт Fastwel NIM-351";
            this.Icon = NGK.Properties.Resources.faviconMy;
            
            // Настраиваем элементы формы
            this._MenuStripMain = new MenuStrip();
            this._MenuFile = new ToolStripMenuItem();
            this._MenuFileExit = new ToolStripLabel();
            this._MenuPort = new ToolStripMenuItem();
            this._MenuPortOpen = new ToolStripLabel();
            this._MenuPortClose = new ToolStripLabel();
            this._MenuPortActiveLine = new ToolStripLabel();
            this._MenuPortPassiveLine = new ToolStripLabel();
            this._MenuPortResetLine = new ToolStripLabel();
            this._MenuPortSendMessage = new ToolStripLabel();
            this._MenuHelp = new ToolStripMenuItem();
            this._MenuHelpAbout = new ToolStripLabel();
            this._StatusStripMain = new StatusStrip();
            this._SplitContainerMain = new SplitContainer();
            this._StatusStripMain.SuspendLayout();
            this._SplitContainerLeftPanel = new SplitContainer();
            this._SplitContainerLeftPanel.SuspendLayout();
            this._SplitContainerRightPanel = new SplitContainer();
            this._SplitContainerRightPanel.SuspendLayout();
            this._TreeViewSystem = new TreeView();
            this._TreeNodeRoot = new TreeNode();
            this._TreeNodePortSettings = new TreeNode();
            this._TreeNodeStatistics = new TreeNode();
            this._DataGridViewIncomingMessages = new DataGridView();
            this._DataGridViewOutcomingMessages = new DataGridView();
            this._DataGridViewStatistics = new DataGridView();
            this._PropertyGridPortSettings = new PropertyGrid();
            this._LabelInformation = new Label();

            this.SuspendLayout();

            // Инициализация меню для "Файл"
            this._MenuFile.Name = "MenuFile";
            this._MenuFile.Text = "&Файл";
            this._MenuStripMain.Items.Add(this._MenuFile);

            // Инициализация подменю для "Файл"->"..."
            this._MenuFileExit.Name = "MenuFileExit";
            this._MenuFileExit.Text = "Выход";
            this._MenuFileExit.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuFile.DropDownItems.Add(this._MenuFileExit);

            // Инициализация меню для "Порт"
            this._MenuPort.Name = "MenuPort";
            this._MenuPort.Text = "&Порт";
            this._MenuStripMain.Items.Add(this._MenuPort);

            // Инициализация подменю для "Порт"->"..."
            this._MenuPortOpen.Name = "MenuPortOpen";
            this._MenuPortOpen.Text = "Открыть";
            this._MenuPortOpen.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortOpen);

            this._MenuPortClose.Name = "MenuPortClose";
            this._MenuPortClose.Text = "Закрыть";
            this._MenuPortClose.Enabled = false;
            this._MenuPortClose.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortClose);

            separator = new ToolStripSeparator();
            separator.Name = "MenuPortSeparator1";
            this._MenuPort.DropDownItems.Add(separator);
           
            this._MenuPortActiveLine.Name = "MenuPortActiveLine";
            this._MenuPortActiveLine.Text = "Подключиться к шине";
            this._MenuPortActiveLine.Enabled = false;
            this._MenuPortActiveLine.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortActiveLine);

            this._MenuPortPassiveLine.Name = "MenuPortPassiveLine";
            this._MenuPortPassiveLine.Text = "Отключиться от шины";
            this._MenuPortPassiveLine.Enabled = false;
            this._MenuPortPassiveLine.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortPassiveLine);

            this._MenuPortResetLine.Name = "MenuPortResetLine";
            this._MenuPortResetLine.Text = "Reset";
            this._MenuPortResetLine.Enabled = false;
            this._MenuPortResetLine.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortResetLine);

            separator = new ToolStripSeparator();
            separator.Name = "MenuPortSeparator2";
            this._MenuPort.DropDownItems.Add(separator);

            this._MenuPortSendMessage.Name = "MenuPortSendMessage";
            this._MenuPortSendMessage.Text = "Отправить сообщение";
            this._MenuPortSendMessage.Enabled = true;
            this._MenuPortSendMessage.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortSendMessage); 

            // Инициализация меню для "Помощь"
            this._MenuHelp.Name = "MenuHelp";
            this._MenuHelp.AutoSize = true;
            this._MenuHelp.Text = "&Помощь";
            this._MenuStripMain.Items.Add(this._MenuHelp);

            // Инициализация подменю для "Помощь"->"..."
            this._MenuHelpAbout.Name = "MenuHelpAbout";
            this._MenuHelpAbout.AutoSize = true;
            this._MenuHelpAbout.Text = "О программе";
            this._MenuHelpAbout.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuHelp.DropDownItems.Add(this._MenuHelpAbout);

            // Инициализируем строку состояния
            this._StatusStripMain.Name = "StatusStripMain";
           
            // Главный контейнер для формирования верхнего и нижнего окна
            this._SplitContainerMain.SuspendLayout();
            this._SplitContainerMain.Name = "SplitContainerMain";
            this._SplitContainerMain.Dock = DockStyle.Fill;
            this._SplitContainerMain.Orientation = Orientation.Vertical;
            this.Controls.Add(this._SplitContainerMain);

            // Контейнер для вертикального деления верхнего окна
            this._SplitContainerLeftPanel.Name = "SplitContainerLeftPanel";
            this._SplitContainerLeftPanel.Dock = DockStyle.Fill;
            this._SplitContainerLeftPanel.Orientation = Orientation.Horizontal;
            this._SplitContainerMain.Panel1.Controls.Add(this._SplitContainerLeftPanel);

            // Контейнер для вертикального деления ниженго окна
            this._SplitContainerRightPanel.Name = "SplitContainerRightPanel.Name";
            this._SplitContainerRightPanel.Dock = DockStyle.Fill;
            this._SplitContainerRightPanel.Orientation = Orientation.Horizontal;
            this._SplitContainerMain.Panel2.Controls.Add(this._SplitContainerRightPanel);

            // Инициализируем дерево объектов
            this._TreeViewSystem.Name = "TreeViewSystem";
            this._TreeViewSystem.Dock = DockStyle.Fill;
            this._TreeViewSystem.AfterSelect += new TreeViewEventHandler(EventHandler_TreeViewSystem_AfterSelect);
            this._SplitContainerLeftPanel.Panel1.Controls.Add(this._TreeViewSystem);
            
            //
            this._TreeNodeRoot.Name = "TreeNodeRoot";
            this._TreeNodeRoot.Text = "NIM-351";
            this._TreeViewSystem.Nodes.Add(this._TreeNodeRoot);
            this._TreeViewSystem.TopNode = this._TreeNodeRoot;
            //
            this._TreeNodePortSettings.Name = "TreeNodePortSettings";
            this._TreeNodePortSettings.Text = "CAN-порт";
            this._TreeViewSystem.TopNode.Nodes.Add(this._TreeNodePortSettings);
            //
            this._TreeNodeStatistics.Name = "TreeNodeStatistics";
            this._TreeNodeStatistics.Text = "Статистика";
            this._TreeViewSystem.TopNode.Nodes.Add(this._TreeNodeStatistics);

            this._TreeViewSystem.ExpandAll();

            // Настраиваем стиль для заголовков столбцов гридов
            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Font = new Font(this._DataGridViewOutcomingMessages.Font, FontStyle.Bold);
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.WrapMode = DataGridViewTriState.True;

            // Грид для отображения входящих сообщений
            this._DataGridViewIncomingMessages.Name = "DataGridViewIncomingMessages";
            this._DataGridViewIncomingMessages.AllowUserToAddRows = false;
            this._DataGridViewIncomingMessages.AllowUserToDeleteRows = false;
            this._DataGridViewIncomingMessages.AllowUserToOrderColumns = false;
            this._DataGridViewIncomingMessages.AutoGenerateColumns = false;
            this._DataGridViewIncomingMessages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewIncomingMessages.MultiSelect = false;
            this._DataGridViewIncomingMessages.RowHeadersVisible = false;
            this._DataGridViewIncomingMessages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewIncomingMessages.Dock = DockStyle.Fill;
            this._DataGridViewIncomingMessages.ColumnHeadersDefaultCellStyle = headerCellStyle;
            this._DataGridViewIncomingMessages.CellFormatting += new DataGridViewCellFormattingEventHandler(EventHandler_DataGridViewIncomingMessages_CellFormatting);

            this._SplitContainerRightPanel.Panel1.Controls.Add(this._DataGridViewIncomingMessages);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnDateTime";
            column.HeaderText = "Дата/время";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(DateTime);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            column.DefaultCellStyle.Format = "HH:mm:ss dd.MM.yy";
            this._DataGridViewIncomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnTimeStamp";
            column.HeaderText = "Time Stamp";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(UInt32);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this._DataGridViewIncomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnIndentifier";
            column.HeaderText = "Id, hex";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(UInt32);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.DefaultCellStyle.Format = "X";
            this._DataGridViewIncomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnFormatFrame";
            column.HeaderText = "Ext.";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(FrameFormat);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this._DataGridViewIncomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnFrameType";
            column.HeaderText = "RTR";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(FrameType);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this._DataGridViewIncomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnData";
            column.HeaderText = "Данные";
            column.ReadOnly = true;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.ValueType = typeof(String);
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            this._DataGridViewIncomingMessages.Columns.Add(column);

            #region Code For Debug 
            int index = this._DataGridViewIncomingMessages.Rows.Add(1);
            row = this._DataGridViewIncomingMessages.Rows[index];

            row.Cells[0].Value = DateTime.Now;
            row.Cells[1].Value = 134;
            row.Cells[2].Value = 10;
            row.Cells[3].Value = Message.FrameFormat.ExtendedFrame;
            row.Cells[4].Value = Message.FrameType.DATAFRAME;
            row.Cells[5].Value = new Byte[] { 01, 02, 03};
            #endregion

            // Контекстное меню для грида входящих сообщений
            this._ContextMenuStripIncomingBox = new ContextMenuStrip();
            this._ContextMenuStripIncomingBox.Name = "ContextMenuStripIncomingBox";
            //
            this._ContextMenuIncomingBoxClear = new ToolStripMenuItem();
            this._ContextMenuIncomingBoxClear.Name = "ContextMenuIncomingBoxClear";
            this._ContextMenuIncomingBoxClear.Text = "Очистить";
            this._ContextMenuIncomingBoxClear.Click += new EventHandler(_ContextMenuIncomingBoxClear_Click);
            this._ContextMenuStripIncomingBox.Items.Add(this._ContextMenuIncomingBoxClear);
            this._DataGridViewIncomingMessages.ContextMenuStrip = this._ContextMenuStripIncomingBox;


            // Грид для отображения исходящий сообещий
            this._DataGridViewOutcomingMessages.Name = "DataGridViewIncomingMessages";
            this._DataGridViewOutcomingMessages.AllowUserToAddRows = false;
            this._DataGridViewOutcomingMessages.AllowUserToDeleteRows = false;
            this._DataGridViewOutcomingMessages.AllowUserToOrderColumns = false;
            this._DataGridViewOutcomingMessages.AutoGenerateColumns = false;
            this._DataGridViewOutcomingMessages.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewOutcomingMessages.MultiSelect = false;
            this._DataGridViewOutcomingMessages.RowHeadersVisible = false;
            this._DataGridViewOutcomingMessages.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewOutcomingMessages.Dock = DockStyle.Fill;
            this._DataGridViewOutcomingMessages.ColumnHeadersDefaultCellStyle = headerCellStyle;
            this._DataGridViewOutcomingMessages.CellClick += 
                new DataGridViewCellEventHandler(EventHandler_DataGridViewOutcomingMessages_CellClick);
            this._DataGridViewOutcomingMessages.EditingControlShowing += 
                new DataGridViewEditingControlShowingEventHandler(
                EventHandler_DataGridViewOutcomingMessages_EditingControlShowing);
            this._DataGridViewOutcomingMessages.CellParsing += 
                new DataGridViewCellParsingEventHandler(
                EventHandler_DataGridViewOutcomingMessages_CellParsing);
            this._DataGridViewOutcomingMessages.CellFormatting += 
                new DataGridViewCellFormattingEventHandler(EventHandler_DataGridViewOutcomingMessages_CellFormatting);
            this._DataGridViewOutcomingMessages.CellEndEdit += 
                new DataGridViewCellEventHandler(EventHandler_DataGridViewOutcomingMessages_CellEndEdit);
            this._DataGridViewOutcomingMessages.DataError += 
                new DataGridViewDataErrorEventHandler(EventHandler_DataGridViewOutcomingMessages_DataError);
            this._SplitContainerRightPanel.Panel2.Controls.Add(this._DataGridViewOutcomingMessages);
            
           

            column = new DataGridViewButtonColumn();
            column.Name = "DataGridViewColumnSend";
            column.HeaderText = "";
            column.ReadOnly = false;
            column.CellTemplate = new DataGridViewButtonCell();
            column.CellTemplate.Value = "OK";
            column.CellTemplate.ValueType = typeof(String);
            column.ValueType = typeof(String);
            //column.ValueType = typeof(NGK.CAN.OSIModel.Message.FrameType);
            this._DataGridViewOutcomingMessages.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnIndentifier";
            column.HeaderText = "Id, hex";
            column.ReadOnly = false;
            column.ValueType = typeof(UInt32);
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.CellTemplate.ValueType = typeof(UInt32);
            column.CellTemplate.Value = (UInt32)1;
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            column.DefaultCellStyle.Format = "X";
            this._DataGridViewOutcomingMessages.Columns.Add(column);

            chxColumn = new DataGridViewCheckBoxColumn();
            chxColumn.Name = "DataGridViewColumnFormatFrame";
            chxColumn.HeaderText = "Ext.";
            chxColumn.ReadOnly = false;
            chxColumn.ValueType = typeof(FrameFormat);
            chxCell = new DataGridViewCheckBoxCell();
            chxCell.ValueType = typeof(FrameFormat);
            chxCell.Value = FrameFormat.StandardFrame;
            chxColumn.CellTemplate = chxCell;
            this._DataGridViewOutcomingMessages.Columns.Add(chxColumn);

            chxColumn = new DataGridViewCheckBoxColumn();
            chxColumn.Name = "DataGridViewColumnFrameType";
            chxColumn.HeaderText = "RTR";
            chxColumn.ReadOnly = false;
            chxColumn.ValueType = typeof(FrameType);
            chxCell = new DataGridViewCheckBoxCell();
            chxCell.ValueType = typeof(FrameType);
            chxColumn.CellTemplate = chxCell;
            chxColumn.ValueType = typeof(FrameType);
            this._DataGridViewOutcomingMessages.Columns.Add(chxColumn);

            column = new DataGridViewColumn();
            column.Name = "DataGridViewColumnData";
            column.HeaderText = "Данные";
            column.ReadOnly = false;
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.CellTemplate.ValueType = typeof(Byte[]);
            column.CellTemplate.Value = new Byte[0];
            column.ValueType = typeof(Byte[]);
            this._DataGridViewOutcomingMessages.Columns.Add(column);

            this._DataGridViewOutcomingMessages.Rows.Add(5);
            for (int i = 0; i < this._DataGridViewOutcomingMessages.Rows.Count; i++)
            {
                //((DataGridViewComboBoxCell)this._DataGridViewOutcomingMessages.Rows[i].Cells[2]).Items.AddRange(
                //    Enum.GetNames(typeof(NGK.CAN.OSIModel.Message.FrameFormat)));
                this._DataGridViewOutcomingMessages.Rows[i].Cells[0].Value = "Отправить";
                this._DataGridViewOutcomingMessages.Rows[i].Cells[1].Value = (UInt32)15;
                this._DataGridViewOutcomingMessages.Rows[i].Cells[2].Value = FrameFormat.StandardFrame;
                this._DataGridViewOutcomingMessages.Rows[i].Cells[3].Value = FrameType.DATAFRAME;
                this._DataGridViewOutcomingMessages.Rows[i].Cells[4].Value = new Byte[0];
            }

            //
            this._PropertyGridPortSettings.Dock = DockStyle.Fill;
            this._PropertyGridPortSettings.SelectedObject = null;
            this._PropertyGridPortSettings.Visible = false;
            this._SplitContainerLeftPanel.Panel2.Controls.Add(this._PropertyGridPortSettings);
            
            //
            this._DataGridViewStatistics.AllowUserToAddRows = false;
            this._DataGridViewStatistics.AllowUserToDeleteRows = false;
            this._DataGridViewStatistics.AllowUserToOrderColumns = false;
            this._DataGridViewStatistics.AutoGenerateColumns = false;
            this._DataGridViewStatistics.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this._DataGridViewStatistics.MultiSelect = false;
            this._DataGridViewStatistics.RowHeadersVisible = false;
            this._DataGridViewStatistics.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this._DataGridViewStatistics.Dock = DockStyle.Fill;
            this._DataGridViewStatistics.Visible = false;
            this._DataGridViewStatistics.ColumnHeadersDefaultCellStyle = headerCellStyle;
            this._SplitContainerLeftPanel.Panel2.Controls.Add(this._DataGridViewStatistics);

            column = new DataGridViewColumn();
            column.Name = "Parameter";
            column.ReadOnly = true;
            column.HeaderText = "Параметр";
            column.ValueType = typeof(String);
            column.CellTemplate = new DataGridViewTextBoxCell();
            this._DataGridViewStatistics.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "Value";
            column.ReadOnly = true;
            column.HeaderText = "Значение";
            column.ValueType = typeof(UInt32);
            column.CellTemplate = new DataGridViewTextBoxCell();
            column.DefaultCellStyle = new DataGridViewCellStyle();
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            this._DataGridViewStatistics.Columns.Add(column);

            System.Reflection.FieldInfo[] fields =
                (typeof(F_CAN_STATS)).GetFields();

            this._DataGridViewStatistics.Rows.Clear();

            for (int i = 0; i < fields.Length; i++)
            {
                row = new DataGridViewRow();
                row.Cells.Add((DataGridViewCell)this._DataGridViewStatistics.Columns[0].CellTemplate.Clone());
                row.Cells.Add((DataGridViewCell)this._DataGridViewStatistics.Columns[1].CellTemplate.Clone());
                row.Cells[0].Value = fields[i].Name;
                Object[] objs = fields[i].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                //row.Cells[0].Value = fields[i].Name;
                row.Cells[0].Value = ((System.ComponentModel.DescriptionAttribute)objs[0]).Description;
                //row.Cells[1].Value = fields[i].GetValue(statistics);
                row.Cells[1].Value = 0;
                this._DataGridViewStatistics.Rows.Add(row);
            }

            // Контекстное меню 
            this._ContextMenuStripStatistics = new ContextMenuStrip();
            this._ContextMenuStripStatistics.Name = "ContextMenuStripStatistics";
            this._ContextMenuStripStatistics.Opening += new CancelEventHandler(_ContextMenuStripStatistics_Opening);
            this._DataGridViewStatistics.ContextMenuStrip = this._ContextMenuStripStatistics;
            //
            this._ContexMenuStatisticsClear = new ToolStripMenuItem();
            this._ContexMenuStatisticsClear.Name = "ContexMenuStatisticsClear";
            this._ContexMenuStatisticsClear.Text = "Очистить статистику";
            this._ContexMenuStatisticsClear.Click += new EventHandler(EventHandler_ContexMenuStatisticsClear_Click);
            this._ContextMenuStripStatistics.Items.Add(this._ContexMenuStatisticsClear);
          
            //
            this._LabelInformation.Text = String.Empty;
            this._LabelInformation.TextAlign = ContentAlignment.MiddleCenter;
            this._LabelInformation.Dock = DockStyle.Fill;
            this._LabelInformation.Visible = false;
            this._SplitContainerLeftPanel.Panel2.Controls.Add(this._LabelInformation);
            
            //
            this.Controls.Add(this._MenuStripMain);
            this.MainMenuStrip = this._MenuStripMain;
            this.Controls.Add(this._StatusStripMain);
            //
            this._StatusStripMain.ResumeLayout(false);
            this._SplitContainerMain.ResumeLayout(false);
            this._SplitContainerLeftPanel.ResumeLayout(false);
            this._SplitContainerRightPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            // Инициализируем CAN-порт
            //this._CanPort = new CanPort();
            //this._CanPort.MessageReceived += 
            //    new EventHandler(EventHandler_CanPort_MessageReceived);
            //this._CanPort.PortChangedStatus += 
            //    new EventHandlerPortChangesStatus(EventHandler_CanPort_PortChangedStatus);
            //this._PropertyGridPortSettings.SelectedObject = this._CanPort;
            if (_CanPort != null)
            {
                this._CanPort.MessageReceived +=
                    new EventHandler(EventHandler_CanPort_MessageReceived);
                this._CanPort.PortChangedStatus +=
                    new EventHandlerPortChangesStatus(EventHandler_CanPort_PortChangedStatus);
                this._PropertyGridPortSettings.SelectedObject = this._CanPort; 
            }

            // Инициализируем таймер
            this._Timer = new Timer();
            this._Timer.Interval = 500;
            this._Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            this._Timer.Start();

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события вызова контекстного меню грида "Статистика"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ContextMenuStripStatistics_Opening(object sender, CancelEventArgs e)
        {
            // При открытии контекстного меню блокируем и разблокируем пункты меню
            // в зависимости от текущего состояния порта
            if (this._CanPort.IsOpen)
            {
                this._ContexMenuStatisticsClear.Enabled = true;
            }
            else
            {
                this._ContexMenuStatisticsClear.Enabled = false;
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик клика по пункту "Очистить статистику" контекстного меню грида "Статистика"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ContexMenuStatisticsClear_Click(object sender, EventArgs e)
        {
            if (this._CanPort.IsOpen)
            {
                this._CanPort.ClearStatistics();
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события клика по контекстному меню
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ContextMenuIncomingBoxClear_Click(object sender, EventArgs e)
        {
            // Очищаем грид входящих сообщений
            this._DataGridViewIncomingMessages.Rows.Clear();
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события отображения данных в ячейке грида входящих сообщения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewIncomingMessages_CellFormatting(object sender, 
            DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewCell cell;
            DataGridView dgv = (DataGridView)sender;

            switch (e.ColumnIndex)
            {
                case 5:
                    {
                        // Отображаем данные
                        cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        Byte[] data = (Byte[])cell.Value;
                        StringBuilder sb = new StringBuilder(data.Length);
                        for (int i = 0; i < data.Length; i++)
                        {
                            sb.Append(data[i].ToString("X2"));
                            sb.Append(" ");
                        }
                        e.Value = sb.ToString().Trim();
                        break;
                    }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события отображения значения ячейки грида на экрне.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_CellFormatting(object sender, 
            DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewCell cell;
            DataGridView dgv = (DataGridView)sender;

            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        break;
                    }
                case 2:
                    {
                        cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        FrameFormat frameFormat = (FrameFormat)cell.Value;
                        switch (frameFormat)
                        {
                            case FrameFormat.ExtendedFrame:
                                {
                                    e.Value = true;
                                    e.FormattingApplied = true;
                                    break; 
                                }
                            case FrameFormat.StandardFrame:
                                {
                                    e.Value = false;
                                    e.FormattingApplied = true;
                                    break; 
                                }
                            default:
                                {
                                    throw new Exception();
                                }
                        }
                        break;
                    }
                case 3:
                    {
                        cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                        FrameType frameType =(FrameType)cell.Value;
                        switch (frameType)
                        {
                            case FrameType.DATAFRAME:
                                {
                                    e.Value = false;
                                    e.FormattingApplied = true;
                                    break;
                                }
                            case FrameType.REMOTEFRAME:
                                {
                                    e.Value = true;
                                    e.FormattingApplied = true;
                                    break;
                                }
                            default:
                                {
                                    throw new Exception();
                                }
                        }
                        break;
                    }
                case 4:
                    {
                        // В зависимости от типа сообщения показываем или скрываем
                        // поле данных (для удалённого запроса RTR=true)
                        cell = dgv.Rows[e.RowIndex].Cells[3];
                        
                        switch ((Message.FrameType)cell.Value)
                        {
                            case FrameType.DATAFRAME:
                                {
                                    // Отображаем данные
                                    cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                                    Byte[] data = (Byte[])cell.Value;
                                    StringBuilder sb = new StringBuilder(data.Length);
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        sb.Append(data[i].ToString("X2"));
                                        sb.Append(" ");
                                    }
                                    e.Value = sb.ToString().Trim();
                                    break;
                                }
                            case FrameType.REMOTEFRAME:
                                {
                                    // Скрываем поле данных
                                    e.Value = String.Empty;
                                    break;
                                }
                            default:
                                {
                                    throw new Exception();
                                }
                        }
                        e.FormattingApplied = true;
                        break;
                    }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_CellParsing(object sender,
            DataGridViewCellParsingEventArgs e)
        {
            DataGridViewRow row;
            DataGridViewCell cell;
            DataGridView dgv = (DataGridView)sender;

            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        // Проверяем значение идентификатора
                        UInt32 id;
                        
                        if (UInt32.TryParse(e.Value.ToString(), 
                            System.Globalization.NumberStyles.HexNumber, null, out id) == true)
                        {
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];
                            cell.Value = id;
                            e.Value = id;
                            e.ParsingApplied = true;
                        }
                        else
                        {
                            // Не удалось преобразовать формат кадра.
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];
                            cell.ErrorText = "Введённое значение не является шестнадцатеричным числом";
                            MessageBox.Show(this, "Введённое значение не является шестнадцатеричным числом",
                                "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            e.ParsingApplied = false;
                        }
                        break;                                
                    }
                case 2: //"DataGridViewColumnFormatFrame"
                    {
                        FrameFormat frameFormat;
                        if ((Boolean)e.Value)
                        {
                            frameFormat = FrameFormat.ExtendedFrame;
                        }
                        else
                        {
                            frameFormat = FrameFormat.StandardFrame;
                        }
                        row = dgv.Rows[e.RowIndex];
                        cell = row.Cells[e.ColumnIndex];
                        cell.Value = frameFormat;
                        // Проверяем размер id 
                        cell = row.Cells[1];
                        if ((UInt32)cell.Value <= Message.Frame.GetIdMaxValue(frameFormat))
                        {
                            cell.ErrorText = String.Empty;
                        }
                        else
                        {
                            cell.ErrorText = String.Format("Введённое значение больше, чем максимально допустимое {0}",
                                Message.Frame.GetIdMaxValue(frameFormat).ToString("X"));
                        }
                        e.Value = frameFormat;
                        e.ParsingApplied = true;
                        break;
                    }
                case 3: //"DataGridViewColumnFrameType"
                    {
                        FrameType frameType;

                        if ((Boolean)e.Value)
                        {
                            frameType = FrameType.REMOTEFRAME;
                            // У сообщения данного типа не может быть поля данных.
                            // Поэтому очищаем данные и запрещаем ввод
                            //String[] strArr = e.Value.ToString().Trim().Split(' ');
                            //dgv.Rows[e.RowIndex].Cells[4].Value = "DLC=";
                            //dgv.Rows[e.RowIndex].Cells[4].Value = new Byte[0];
                            dgv.Rows[e.RowIndex].Cells[4].ReadOnly = true;
                        }
                        else
                        { 
                            frameType = FrameType.DATAFRAME;
                            dgv.Rows[e.RowIndex].Cells[4].ReadOnly = false;
                        }
                        e.Value = frameType;
                        row = dgv.Rows[e.RowIndex];
                        cell = row.Cells[e.ColumnIndex];
                        cell.Value = frameType;
                        e.ParsingApplied = true;
                        break;
                    }
                case 4:
                    {
                        // Если используется удалённый запрос, поле должно быть недоступно
                        // для редактирования и поэтому разбор поля не делаем
                        cell = dgv.Rows[e.RowIndex].Cells[3];
                        switch ((Message.FrameType)cell.Value)
                        {
                            case FrameType.DATAFRAME:
                                {

                                    String[] strArr = e.Value.ToString().Trim().Split(' ');
                                    Byte[] data = new Byte[strArr.Length];

                                    // Длина данных CAN-сообщения не может быть более 8 байт
                                    if (strArr.Length > 8)
                                    {
                                        MessageBox.Show(this, String.Format("", strArr.Length.ToString()), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < strArr.Length; i++)
                                        {
                                            // Каждый байт должен состоять из 2-х симолов. Проверяем это
                                            if (strArr[i].Length == 2)
                                            {
                                                try
                                                {
                                                    data[i] = Byte.Parse(strArr[i], System.Globalization.NumberStyles.HexNumber);
                                                }
                                                catch
                                                {
                                                    MessageBox.Show("Ошибка ввода");
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Ошибка ввода");
                                            }
                                        }
                                        row = dgv.Rows[e.RowIndex];
                                        cell = row.Cells[e.ColumnIndex];
                                        cell.Value = data;
                                        e.Value = data;
                                        e.ParsingApplied = true;
                                    }
                                    break;
                                }
                            case FrameType.REMOTEFRAME:
                                {
                                    // Ничего не делаем
                                    e.ParsingApplied = true;
                                    break; 
                                }
                        }
                        break;
                    }
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события окончания редактирования ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_CellEndEdit(
            object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row;
            DataGridViewCell cell;
            //DataGridViewCheckBoxCell chkBoxCell;
            //DataGridViewTextBoxCell txtBoxCell;

            String msg;
            DataGridView dgv = (DataGridView)sender;


            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        // Получаем текущий (установленный пользователем формат кадра сообщения)
                        cell = dgv.Rows[e.RowIndex].Cells[2];
                        FrameFormat format =
                            (FrameFormat)cell.Value;


                        row = dgv.Rows[e.RowIndex];
                        cell = row.Cells[e.ColumnIndex];
                        // В зависимости от формата кадра проверяем допустимость значения
                        // идентификатора сообщения
                        if ((UInt32)cell.Value <= Frame.GetIdMaxValue(format))
                        {
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];

                            cell.ErrorText = String.Empty;
                        }
                        else
                        {
                            // Недопустимое значени идентификатора
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];

                            msg = String.Format("Введённое значение больше, чем максимально допустимое {0}",
                                Frame.GetIdMaxValue(format).ToString("X"));
                            cell.ErrorText = msg;

                            //MessageBox.Show(this, msg, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                    }
                case 3:
                    {
                        break;
                    }
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события ошибки формата данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_DataError(object sender, 
            DataGridViewDataErrorEventArgs e)
        {
            switch (e.ColumnIndex)
            {
                case 1:
                    {
                        e.Cancel = true;
                        break;
                    }
                default:
                    {
                        throw e.Exception; 
                    }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события показа контрола для редактирования ячейки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_EditingControlShowing(object sender, 
            DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            DataGridViewTextBoxEditingControl control;
            //DataGridViewCell cell;
            //Byte[] data;

            if (e.Control is DataGridViewTextBoxEditingControl)
            {
                control = (DataGridViewTextBoxEditingControl)e.Control;

                switch (dgv.CurrentCellAddress.X)
                {
                    case 1:
                        {
                            break; 
                        }
                    case 4: // Ввод данных CAN-сообщения
                        {
                            // Отслеживаем ввод символов пользователя
                            control.KeyPress += 
                                new KeyPressEventHandler(EventHandler_ControlEnterCanData_KeyPress);
                            break; 
                        }
                }
            }
           
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события ввода пользователем байт CAN-сообщения. Не более 8 байт
        /// в шестнадцатеричном формате.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ControlEnterCanData_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridViewTextBoxEditingControl control = 
                (DataGridViewTextBoxEditingControl)sender;

            // Проверяем являетсяли введённый символ цифрой
            if (Char.IsDigit(e.KeyChar))
            {
                // Проверяем можноли вводить следующй байт сообщения или уже заплненны все 8
                if (control.Text.Length < 24)
                {
                    if ((control.Text.Length != 0) && (0 == ((control.Text.Length + 1) % 3)))
                    {
                        //В данную позиюцию можно установить только символ пробела
                        control.Text = control.Text + " ";
                        control.SelectionStart = control.Text.Length;
                    }

                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                }
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);

                // Проверяем можноли вводить следующй байт сообщения или уже заплненны все 8
                if (control.Text.Length < 24)
                {
                    switch (e.KeyChar)
                    {
                        case 'A':
                            { e.Handled = false; break; }
                        case 'B':
                            { e.Handled = false; break; }
                        case 'C':
                            { e.Handled = false; break; }
                        case 'D':
                            { e.Handled = false; break; }
                        case 'E':
                            { e.Handled = false; break; }
                        case 'F':
                            { e.Handled = false; break; }
                        //case ' ':
                        //    { e.Handled = false; break; }
                        default:
                            { e.Handled = true; break; }
                    }

                    if ((control.Text.Length != 0) && (0 == ((control.Text.Length + 1) % 3)))
                    {
                        //В данную позиюцию можно установить только символ пробела
                        control.Text = control.Text + " ";
                        control.SelectionStart = control.Text.Length;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик клика по ячейкам грида исходящих сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_DataGridViewOutcomingMessages_CellClick(object sender, 
            DataGridViewCellEventArgs e)
        {
            DataGridView dgv;
            DataGridViewRow row;
            Message.Frame message;
            dgv = (DataGridView)sender;
            // Определяем от какой ячейки, какого столбца пришло событие
            // Если от столбца "Отправить", то отправляем сообщение
            if ((dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn) &&
                (e.RowIndex >= 0))
            {
                row = dgv.Rows[e.RowIndex];
                message = new Frame();
                message.Identifier = (UInt32)row.Cells[1].Value;
                message.FrameFormat = (Message.FrameFormat)row.Cells[2].Value;
                message.FrameType = (Message.FrameType)row.Cells[3].Value;
                message.Data = (Byte[])row.Cells[4].Value;
                
                //MessageBox.Show(String.Format("Сообщение отправлено: {0}", message.ToString()));
                if (this._CanPort.IsOpen)
                {
                    this._CanPort.Write(message);
                }
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события срабатывания таймера общего назначения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Timer_Tick(object sender, EventArgs e)
        {
            // Обновляем статистику
            this.UpdateStatistics();
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события изменения статуса CAN-порта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void EventHandler_CanPort_PortChangedStatus(object sender, 
            EventArgsPortChangesStatus args)
        {
            String msg;

            this._PropertyGridPortSettings.Refresh();

            switch (args.Status)
            {
                case CanPortStatus.IsActive:
                    {
                        this._MenuPortOpen.Enabled = false;
                        break; 
                    }
                case CanPortStatus.IsClosed:
                    {
                        this._MenuPortOpen.Enabled = true;
                        break; 
                    }
                case CanPortStatus.IsPassive:
                    {
                        this._MenuPortOpen.Enabled = false;
                        break; 
                    }
                case CanPortStatus.IsPassiveAfterReset:
                    {
                        this._MenuPortOpen.Enabled = false;
                        break; 
                    }
                case CanPortStatus.Unknown:
                    {
                        this._MenuPortOpen.Enabled = false;
                        this._MenuPortClose.Enabled = false;
                        msg = "Can-порт изменил состояние на Unknown";
                        throw new Exception(msg);
                        //break; 
                    }
                default:
                    {
                        msg = String.Format(
                            "Can-порт передал новое состояние {0}, которое не поддерживается в данном ПО",
                            args.Status.ToString());
                        throw new NotImplementedException(msg);
                    }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события приёма сообщения в CAN-порт
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_CanPort_MessageReceived(object sender, EventArgs e)
        {
            DataGridViewRow row;
            //DataGridViewCell cell;
            CanPort port;
            Message.Frame[] incomingMessages;
            int index;

            // Принятое сообщени от порта записываем в датагрид
            port = (CanPort)sender;

            incomingMessages = port.ReadMessages();

            for (int i = 0; i < incomingMessages.Length; i++)
            {
                index = this._DataGridViewIncomingMessages.Rows.Add(1);
                row = this._DataGridViewIncomingMessages.Rows[index];

                row.Cells[0].Value = DateTime.Now;
                row.Cells[1].Value = incomingMessages[i].TimeStamp;
                row.Cells[2].Value = incomingMessages[i].Identifier;
                row.Cells[3].Value = incomingMessages[i].FrameFormat;
                row.Cells[4].Value = incomingMessages[i].FrameType;
                row.Cells[5].Value = incomingMessages[i].Data;
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик события выбора узла дерева системных объектов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_TreeViewSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((e.Action == TreeViewAction.ByKeyboard) || 
                (e.Action == TreeViewAction.ByMouse))
            {
                if (e.Node.Equals(this._TreeNodeRoot))
                {
                    // Не реагируем
                    this._LabelInformation.Text = String.Empty;
                    this._LabelInformation.Visible = true;
                    this._DataGridViewStatistics.Visible = false;
                    this._PropertyGridPortSettings.Visible = false;
                }
                else if (e.Node.Equals(this._TreeNodePortSettings))
                {                    
                    if (this._CanPort.IsOpen)
                    {
                        this._PropertyGridPortSettings.Enabled = false;
                    }
                    else
                    {
                        this._PropertyGridPortSettings.Enabled = true;
                    }
                    this._DataGridViewStatistics.Visible = false;
                    this._PropertyGridPortSettings.Visible = true;
                    this._LabelInformation.Visible = false;
                }
                else if (e.Node.Equals(this._TreeNodeStatistics))
                {
                    this._PropertyGridPortSettings.Visible = false;

                    //if (this._CanPort.IsOpen)
                    //{
                    this._DataGridViewStatistics.Visible = true;
                    this._LabelInformation.Visible = false;
                    //}
                    //else
                    //{
                    //  this._DataGridViewStatistics.Visible = false;
                    //  this._LabelInformation.Text = "CAN-порт закрыт. Статистика не доступна";
                    //  this._LabelInformation.Visible = true;
                    //}
                }
                else
                {
                    throw new NotImplementedException(
                        String.Format("Обработка узла системного дерева {0} не реализована", e.Node.Name));
                }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обработчик событий выбора различных меню.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Menu_Click(object sender, EventArgs e)
        {
            ToolStripItem menu = (ToolStripItem)sender;

            if (menu.Equals(this._MenuFileExit))
            {
                Application.Exit();
            }
            else if (menu.Equals(this._MenuPortOpen))
            {
                    // Открываем порт
                    this._CanPort.Open();
                    this._MenuPortOpen.Enabled = false;
                    this._MenuPortClose.Enabled = true;
                    this._MenuPortActiveLine.Enabled = true;
                    this._MenuPortPassiveLine.Enabled = true;
                    this._MenuPortResetLine.Enabled = true;
            }
            else if (menu.Equals(this._MenuPortClose))
            {
                this._CanPort.Close();
                this._MenuPortOpen.Enabled = true;
                this._MenuPortClose.Enabled = false;
                this._MenuPortActiveLine.Enabled = false;
                this._MenuPortPassiveLine.Enabled = false;
                this._MenuPortResetLine.Enabled = false;
            }
            else if (menu.Equals(this._MenuPortActiveLine))
            {
                this._CanPort.Start();
            }
            else if (menu.Equals(this._MenuPortPassiveLine))
            {
                this._CanPort.Stop();
            }
            else if (menu.Equals(this._MenuPortResetLine))
            {
                this._CanPort.Reset();
            }
            else if (menu.Equals(this._MenuPortSendMessage))
            {
                //
            }
            else if (menu.Equals(this._MenuHelpAbout))
            {
                MessageBox.Show(this, "Меню не реализовано", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                throw new NotImplementedException(
                    String.Format("Обработка события click menu {0} не реализовано", menu.Name));
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// Обнавляет статистику CAN-порта
        /// </summary>
        private void UpdateStatistics()
        {
            F_CAN_STATS statistics;

            if (this._CanPort.IsOpen)
            {
                // Получаем и выводим статистику
                statistics = this._CanPort.GetStatistics();
            }
            else
            {
                statistics = new F_CAN_STATS();
                statistics.bus_error = 2;
            }

            System.Reflection.FieldInfo[] fields =
                    (typeof(F_CAN_STATS)).GetFields();

            for (int i = 0; i < this._DataGridViewStatistics.Rows.Count; i++)
            {
                for (int x = 0; x < fields.Length; x++)
                {
                    Object[] objs = fields[x].GetCustomAttributes(
                        typeof(System.ComponentModel.DescriptionAttribute), false);
                    if ((String)this._DataGridViewStatistics.Rows[i].Cells[0].Value ==
                        ((System.ComponentModel.DescriptionAttribute)objs[0]).Description)
                    {
                        this._DataGridViewStatistics.Rows[i].Cells[1].Value =
                            (UInt32)fields[x].GetValue(statistics);
                    }
                }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        #endregion
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    }
    //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
}
//~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// End Of File