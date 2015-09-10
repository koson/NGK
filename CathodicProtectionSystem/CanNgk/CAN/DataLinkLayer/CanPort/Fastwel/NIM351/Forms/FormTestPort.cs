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
        /// ������� ������ ���� �����
        /// </summary>
        private MenuStrip _MenuStripMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"
        /// </summary>
        private ToolStripMenuItem _MenuFile;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"�����"
        /// </summary>
        private ToolStripLabel _MenuFileExit;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"
        /// </summary>
        private ToolStripMenuItem _MenuPort;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"�������"
        /// </summary>
        private ToolStripLabel _MenuPortOpen;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"�������"
        /// </summary>
        private ToolStripLabel _MenuPortClose;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"������������ � ����"
        /// </summary>
        private ToolStripLabel _MenuPortActiveLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"����������� �� ����"
        /// </summary>
        private ToolStripLabel _MenuPortPassiveLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"Reset ��������� Init"
        /// </summary>
        private ToolStripLabel _MenuPortResetLine;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "����"->"��������� ���������"
        /// </summary>
        private ToolStripLabel _MenuPortSendMessage; 
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "������"
        /// </summary>
        private ToolStripMenuItem _MenuHelp;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� "������"->"� ���������"
        /// </summary>
        private ToolStripLabel _MenuHelpAbout;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ��������� ����� ���� �� ������� � ������ ������
        /// </summary>
        private SplitContainer _SplitContainerMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ��������� ��� �������� ����
        /// </summary>
        private SplitContainer _SplitContainerLeftPanel;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ��������� ��� ������� ����
        /// </summary>
        private SplitContainer _SplitContainerRightPanel;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������ ��������� �������� ����
        /// </summary>
        private StatusStrip _StatusStripMain;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������ ��������� ��������
        /// </summary>
        private TreeView _TreeViewSystem;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// �������� ���� _TreeViewSystem  
        /// </summary>
        private TreeNode _TreeNodeRoot;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� _TreeViewSystem->"��������� �����"
        /// </summary>
        private TreeNode _TreeNodePortSettings;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� _TreeViewSystem->"���������� �����"
        /// </summary>
        private TreeNode _TreeNodeStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� ��� ����������� �������� �������� � CAN-���� �� ����
        /// </summary>
        private DataGridView _DataGridViewIncomingMessages;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���� ��� ����������� ��������� ��������� � ���� ����� CAN-����
        /// </summary>
        private DataGridView _DataGridViewOutcomingMessages;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������� ��� ����������� �������� �����
        /// </summary>
        private PropertyGrid _PropertyGridPortSettings;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������� ��� ����������� �������������� ������ �����
        /// </summary>
        private DataGridView _DataGridViewStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������� ��� ������ ���������� � ������ ������ �������� ����
        /// </summary>
        private Label _LabelInformation;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������ ��� ������ ����������.
        /// </summary>
        private System.Windows.Forms.Timer _Timer;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ����������� ���� ��� ����� �������� ��������� _DataGridViewIncomingMessages
        /// </summary>
        private ContextMenuStrip _ContextMenuStripIncomingBox;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ����� ������������ ���� _ContextMenuStripIncomingBox
        /// </summary>
        private ToolStripMenuItem _ContextMenuIncomingBoxClear;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ������������ ���� ����� _DataGridViewStatistics
        /// </summary>
        private ContextMenuStrip _ContextMenuStripStatistics;
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ����� ������������ ���� _ContextMenuStripStatistics
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
            throw new NotImplementedException("����� ������������ ������������ �� ���������");
        }

        public FormTestPort(CanPort port)
        {
            InitializeComponent();

            if (port == null)
            {
                throw new NullReferenceException("���������� �������� CanPort ����� ������� ������");
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
        /// ���������� ������� �������� �����
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

            this.Text = "CAN-���� Fastwel NIM-351";
            this.Icon = NGK.Properties.Resources.faviconMy;
            
            // ����������� �������� �����
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

            // ������������� ���� ��� "����"
            this._MenuFile.Name = "MenuFile";
            this._MenuFile.Text = "&����";
            this._MenuStripMain.Items.Add(this._MenuFile);

            // ������������� ������� ��� "����"->"..."
            this._MenuFileExit.Name = "MenuFileExit";
            this._MenuFileExit.Text = "�����";
            this._MenuFileExit.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuFile.DropDownItems.Add(this._MenuFileExit);

            // ������������� ���� ��� "����"
            this._MenuPort.Name = "MenuPort";
            this._MenuPort.Text = "&����";
            this._MenuStripMain.Items.Add(this._MenuPort);

            // ������������� ������� ��� "����"->"..."
            this._MenuPortOpen.Name = "MenuPortOpen";
            this._MenuPortOpen.Text = "�������";
            this._MenuPortOpen.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortOpen);

            this._MenuPortClose.Name = "MenuPortClose";
            this._MenuPortClose.Text = "�������";
            this._MenuPortClose.Enabled = false;
            this._MenuPortClose.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortClose);

            separator = new ToolStripSeparator();
            separator.Name = "MenuPortSeparator1";
            this._MenuPort.DropDownItems.Add(separator);
           
            this._MenuPortActiveLine.Name = "MenuPortActiveLine";
            this._MenuPortActiveLine.Text = "������������ � ����";
            this._MenuPortActiveLine.Enabled = false;
            this._MenuPortActiveLine.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortActiveLine);

            this._MenuPortPassiveLine.Name = "MenuPortPassiveLine";
            this._MenuPortPassiveLine.Text = "����������� �� ����";
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
            this._MenuPortSendMessage.Text = "��������� ���������";
            this._MenuPortSendMessage.Enabled = true;
            this._MenuPortSendMessage.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuPort.DropDownItems.Add(this._MenuPortSendMessage); 

            // ������������� ���� ��� "������"
            this._MenuHelp.Name = "MenuHelp";
            this._MenuHelp.AutoSize = true;
            this._MenuHelp.Text = "&������";
            this._MenuStripMain.Items.Add(this._MenuHelp);

            // ������������� ������� ��� "������"->"..."
            this._MenuHelpAbout.Name = "MenuHelpAbout";
            this._MenuHelpAbout.AutoSize = true;
            this._MenuHelpAbout.Text = "� ���������";
            this._MenuHelpAbout.Click += new EventHandler(EventHandler_Menu_Click);
            this._MenuHelp.DropDownItems.Add(this._MenuHelpAbout);

            // �������������� ������ ���������
            this._StatusStripMain.Name = "StatusStripMain";
           
            // ������� ��������� ��� ������������ �������� � ������� ����
            this._SplitContainerMain.SuspendLayout();
            this._SplitContainerMain.Name = "SplitContainerMain";
            this._SplitContainerMain.Dock = DockStyle.Fill;
            this._SplitContainerMain.Orientation = Orientation.Vertical;
            this.Controls.Add(this._SplitContainerMain);

            // ��������� ��� ������������� ������� �������� ����
            this._SplitContainerLeftPanel.Name = "SplitContainerLeftPanel";
            this._SplitContainerLeftPanel.Dock = DockStyle.Fill;
            this._SplitContainerLeftPanel.Orientation = Orientation.Horizontal;
            this._SplitContainerMain.Panel1.Controls.Add(this._SplitContainerLeftPanel);

            // ��������� ��� ������������� ������� ������� ����
            this._SplitContainerRightPanel.Name = "SplitContainerRightPanel.Name";
            this._SplitContainerRightPanel.Dock = DockStyle.Fill;
            this._SplitContainerRightPanel.Orientation = Orientation.Horizontal;
            this._SplitContainerMain.Panel2.Controls.Add(this._SplitContainerRightPanel);

            // �������������� ������ ��������
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
            this._TreeNodePortSettings.Text = "CAN-����";
            this._TreeViewSystem.TopNode.Nodes.Add(this._TreeNodePortSettings);
            //
            this._TreeNodeStatistics.Name = "TreeNodeStatistics";
            this._TreeNodeStatistics.Text = "����������";
            this._TreeViewSystem.TopNode.Nodes.Add(this._TreeNodeStatistics);

            this._TreeViewSystem.ExpandAll();

            // ����������� ����� ��� ���������� �������� ������
            DataGridViewCellStyle headerCellStyle = new DataGridViewCellStyle();
            headerCellStyle.Font = new Font(this._DataGridViewOutcomingMessages.Font, FontStyle.Bold);
            headerCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            headerCellStyle.WrapMode = DataGridViewTriState.True;

            // ���� ��� ����������� �������� ���������
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
            column.HeaderText = "����/�����";
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
            column.HeaderText = "������";
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

            // ����������� ���� ��� ����� �������� ���������
            this._ContextMenuStripIncomingBox = new ContextMenuStrip();
            this._ContextMenuStripIncomingBox.Name = "ContextMenuStripIncomingBox";
            //
            this._ContextMenuIncomingBoxClear = new ToolStripMenuItem();
            this._ContextMenuIncomingBoxClear.Name = "ContextMenuIncomingBoxClear";
            this._ContextMenuIncomingBoxClear.Text = "��������";
            this._ContextMenuIncomingBoxClear.Click += new EventHandler(_ContextMenuIncomingBoxClear_Click);
            this._ContextMenuStripIncomingBox.Items.Add(this._ContextMenuIncomingBoxClear);
            this._DataGridViewIncomingMessages.ContextMenuStrip = this._ContextMenuStripIncomingBox;


            // ���� ��� ����������� ��������� ��������
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
            column.HeaderText = "������";
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
                this._DataGridViewOutcomingMessages.Rows[i].Cells[0].Value = "���������";
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
            column.HeaderText = "��������";
            column.ValueType = typeof(String);
            column.CellTemplate = new DataGridViewTextBoxCell();
            this._DataGridViewStatistics.Columns.Add(column);

            column = new DataGridViewColumn();
            column.Name = "Value";
            column.ReadOnly = true;
            column.HeaderText = "��������";
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

            // ����������� ���� 
            this._ContextMenuStripStatistics = new ContextMenuStrip();
            this._ContextMenuStripStatistics.Name = "ContextMenuStripStatistics";
            this._ContextMenuStripStatistics.Opening += new CancelEventHandler(_ContextMenuStripStatistics_Opening);
            this._DataGridViewStatistics.ContextMenuStrip = this._ContextMenuStripStatistics;
            //
            this._ContexMenuStatisticsClear = new ToolStripMenuItem();
            this._ContexMenuStatisticsClear.Name = "ContexMenuStatisticsClear";
            this._ContexMenuStatisticsClear.Text = "�������� ����������";
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

            // �������������� CAN-����
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

            // �������������� ������
            this._Timer = new Timer();
            this._Timer.Interval = 500;
            this._Timer.Tick += new EventHandler(EventHandler_Timer_Tick);
            this._Timer.Start();

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ������ ������������ ���� ����� "����������"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ContextMenuStripStatistics_Opening(object sender, CancelEventArgs e)
        {
            // ��� �������� ������������ ���� ��������� � ������������ ������ ����
            // � ����������� �� �������� ��������� �����
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
        /// ���������� ����� �� ������ "�������� ����������" ������������ ���� ����� "����������"
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
        /// ���������� ������� ����� �� ������������ ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ContextMenuIncomingBoxClear_Click(object sender, EventArgs e)
        {
            // ������� ���� �������� ���������
            this._DataGridViewIncomingMessages.Rows.Clear();
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ����������� ������ � ������ ����� �������� ���������
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
                        // ���������� ������
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
        /// ���������� ������� ����������� �������� ������ ����� �� �����.
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
                        // � ����������� �� ���� ��������� ���������� ��� ��������
                        // ���� ������ (��� ��������� ������� RTR=true)
                        cell = dgv.Rows[e.RowIndex].Cells[3];
                        
                        switch ((Message.FrameType)cell.Value)
                        {
                            case FrameType.DATAFRAME:
                                {
                                    // ���������� ������
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
                                    // �������� ���� ������
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
        /// ���������� ������� 
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
                        // ��������� �������� ��������������
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
                            // �� ������� ������������� ������ �����.
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];
                            cell.ErrorText = "�������� �������� �� �������� ����������������� ������";
                            MessageBox.Show(this, "�������� �������� �� �������� ����������������� ������",
                                "������ �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        // ��������� ������ id 
                        cell = row.Cells[1];
                        if ((UInt32)cell.Value <= Message.Frame.GetIdMaxValue(frameFormat))
                        {
                            cell.ErrorText = String.Empty;
                        }
                        else
                        {
                            cell.ErrorText = String.Format("�������� �������� ������, ��� ����������� ���������� {0}",
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
                            // � ��������� ������� ���� �� ����� ���� ���� ������.
                            // ������� ������� ������ � ��������� ����
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
                        // ���� ������������ �������� ������, ���� ������ ���� ����������
                        // ��� �������������� � ������� ������ ���� �� ������
                        cell = dgv.Rows[e.RowIndex].Cells[3];
                        switch ((Message.FrameType)cell.Value)
                        {
                            case FrameType.DATAFRAME:
                                {

                                    String[] strArr = e.Value.ToString().Trim().Split(' ');
                                    Byte[] data = new Byte[strArr.Length];

                                    // ����� ������ CAN-��������� �� ����� ���� ����� 8 ����
                                    if (strArr.Length > 8)
                                    {
                                        MessageBox.Show(this, String.Format("", strArr.Length.ToString()), "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    else
                                    {
                                        for (int i = 0; i < strArr.Length; i++)
                                        {
                                            // ������ ���� ������ �������� �� 2-� �������. ��������� ���
                                            if (strArr[i].Length == 2)
                                            {
                                                try
                                                {
                                                    data[i] = Byte.Parse(strArr[i], System.Globalization.NumberStyles.HexNumber);
                                                }
                                                catch
                                                {
                                                    MessageBox.Show("������ �����");
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("������ �����");
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
                                    // ������ �� ������
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
        /// ���������� ������� ��������� �������������� ������
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
                        // �������� ������� (������������� ������������� ������ ����� ���������)
                        cell = dgv.Rows[e.RowIndex].Cells[2];
                        FrameFormat format =
                            (FrameFormat)cell.Value;


                        row = dgv.Rows[e.RowIndex];
                        cell = row.Cells[e.ColumnIndex];
                        // � ����������� �� ������� ����� ��������� ������������ ��������
                        // �������������� ���������
                        if ((UInt32)cell.Value <= Frame.GetIdMaxValue(format))
                        {
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];

                            cell.ErrorText = String.Empty;
                        }
                        else
                        {
                            // ������������ ������� ��������������
                            row = dgv.Rows[e.RowIndex];
                            cell = row.Cells[e.ColumnIndex];

                            msg = String.Format("�������� �������� ������, ��� ����������� ���������� {0}",
                                Frame.GetIdMaxValue(format).ToString("X"));
                            cell.ErrorText = msg;

                            //MessageBox.Show(this, msg, "������ �����", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        /// ���������� ������� ������ ������� ������
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
        /// ���������� ������� ������ �������� ��� �������������� ������
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
                    case 4: // ���� ������ CAN-���������
                        {
                            // ����������� ���� �������� ������������
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
        /// ���������� ������� ����� ������������� ���� CAN-���������. �� ����� 8 ����
        /// � ����������������� �������.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_ControlEnterCanData_KeyPress(object sender, KeyPressEventArgs e)
        {
            DataGridViewTextBoxEditingControl control = 
                (DataGridViewTextBoxEditingControl)sender;

            // ��������� ���������� �������� ������ ������
            if (Char.IsDigit(e.KeyChar))
            {
                // ��������� ������� ������� �������� ���� ��������� ��� ��� ��������� ��� 8
                if (control.Text.Length < 24)
                {
                    if ((control.Text.Length != 0) && (0 == ((control.Text.Length + 1) % 3)))
                    {
                        //� ������ �������� ����� ���������� ������ ������ �������
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

                // ��������� ������� ������� �������� ���� ��������� ��� ��� ��������� ��� 8
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
                        //� ������ �������� ����� ���������� ������ ������ �������
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
        /// ���������� ����� �� ������� ����� ��������� ���������
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
            // ���������� �� ����� ������, ������ ������� ������ �������
            // ���� �� ������� "���������", �� ���������� ���������
            if ((dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn) &&
                (e.RowIndex >= 0))
            {
                row = dgv.Rows[e.RowIndex];
                message = new Frame();
                message.Identifier = (UInt32)row.Cells[1].Value;
                message.FrameFormat = (Message.FrameFormat)row.Cells[2].Value;
                message.FrameType = (Message.FrameType)row.Cells[3].Value;
                message.Data = (Byte[])row.Cells[4].Value;
                
                //MessageBox.Show(String.Format("��������� ����������: {0}", message.ToString()));
                if (this._CanPort.IsOpen)
                {
                    this._CanPort.Write(message);
                }
            }

            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ������������ ������� ������ ����������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_Timer_Tick(object sender, EventArgs e)
        {
            // ��������� ����������
            this.UpdateStatistics();
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ��������� ������� CAN-�����
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
                        msg = "Can-���� ������� ��������� �� Unknown";
                        throw new Exception(msg);
                        //break; 
                    }
                default:
                    {
                        msg = String.Format(
                            "Can-���� ������� ����� ��������� {0}, ������� �� �������������� � ������ ��",
                            args.Status.ToString());
                        throw new NotImplementedException(msg);
                    }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ����� ��������� � CAN-����
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

            // �������� �������� �� ����� ���������� � ��������
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
        /// ���������� ������� ������ ���� ������ ��������� ��������
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
                    // �� ���������
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
                    //  this._LabelInformation.Text = "CAN-���� ������. ���������� �� ��������";
                    //  this._LabelInformation.Visible = true;
                    //}
                }
                else
                {
                    throw new NotImplementedException(
                        String.Format("��������� ���� ���������� ������ {0} �� �����������", e.Node.Name));
                }
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ���������� ������� ������ ��������� ����.
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
                    // ��������� ����
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
                MessageBox.Show(this, "���� �� �����������", "��������",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                throw new NotImplementedException(
                    String.Format("��������� ������� click menu {0} �� �����������", menu.Name));
            }
            return;
        }
        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        /// <summary>
        /// ��������� ���������� CAN-�����
        /// </summary>
        private void UpdateStatistics()
        {
            F_CAN_STATS statistics;

            if (this._CanPort.IsOpen)
            {
                // �������� � ������� ����������
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