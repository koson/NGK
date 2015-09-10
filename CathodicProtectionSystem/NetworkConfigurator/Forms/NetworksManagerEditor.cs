using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Master;

namespace NGK.NetworkConfigurator
{
    public partial class NetworksManagerEditor : Form
    {
        #region Fields And Properties
        private NetworksManager _NetworksManager;
        public NetworksManager NetworksManager
        {
            get { return _NetworksManager; }
            set { _NetworksManager = value; }
        }
        /// <summary>
        /// Путь к файлу конфигурации
        /// </summary>
        private String _PathToFile;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public NetworksManagerEditor()
        {
            InitializeComponent();

            Init();
        }
        #endregion

        #region Methods

        private void Init()
        {
            this.Icon = Properties.Resources.faviconMy;
            
            //this.UseWaitCursor = true;

            this.NetworksManager = NetworksManager.Instance;

            // Инициализация меню
            ToolStripMenuItem menuItem;
            ToolStripSeparator menuSeparator;

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuFile";
            menuItem.Text = "&Файл"; // & для подчёркивания буквы Ф и возможность выбра меню сочетанием клавишь ALT + Ф
            this._MenuStripMain.Items.Add(menuItem);

            // Члены меню "Файл"
            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuNew";
            menuItem.Text = "&Создать";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            menuSeparator = new ToolStripSeparator();
            menuSeparator.Name = "_MenuFileSeparator1";
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuSeparator);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuOpenFile";
            menuItem.Text = "&Открыть файл";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuCloseFile";
            menuItem.Text = "&Закрыть файл";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            menuSeparator = new ToolStripSeparator();
            menuSeparator.Name = "_MenuFileSeparator2";
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuSeparator);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuSave";
            menuItem.Text = "&Сохранить";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuSaveAs";
            menuItem.Text = "Сохранить &как";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            menuSeparator = new ToolStripSeparator();
            menuSeparator.Name = "_MenuFileSeparator3";
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuSeparator);

            menuItem = new ToolStripMenuItem();
            menuItem.Name = "_MenuExit";
            menuItem.Text = "&Выход";
            menuItem.Click += new EventHandler(EventHandler_MenuItem_Click);
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems.Add(menuItem);

            // Настраиваем PropertyGrid для редактирования конфигурации сетей.
            PropertyGrid grid = new PropertyGrid();
            grid.Name = "_PropertyGridMain";
            grid.Dock = DockStyle.Fill;
            this._SplitContainerMain.Panel2.Controls.Add(grid);
            grid.SelectedObject = null;

            // Инициализация строки статуса
            ToolStripLabel lableItem;
            lableItem = new ToolStripLabel();
            lableItem.Name = "_ToolStripLabelPathToFile";
            lableItem.Alignment = ToolStripItemAlignment.Left;
            lableItem.AutoToolTip = true;
            lableItem.TextAlign = ContentAlignment.MiddleLeft;
            lableItem.ToolTipText = "Путь к файлу конфигурации";
            this._StatusStripMain.Items.Add(lableItem);


            // Устанавливаем путь к файлу конфигурации.
            this.SetPathToFile(null);


            return;
        }
        /// <summary>
        /// Обработчик выбора элементов главного меню 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_MenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            switch (item.Name)
            {
                case "_MenuNew":
                    {
                        this.CreateNewFile();
                        break;
                    }
                case "_MenuOpenFile":
                    {
                        this.OpenFile();
                        break; 
                    }
                case "_MenuCloseFile":
                    {
                        this.CloseFile();
                        break;
                    }
                case "_MenuSave":
                    {
                        this.SaveFile();
                        break; 
                    }
                case "_MenuSaveAs":
                    {
                        this.SaveAsFile();
                        break;
                    }
                case "_MenuExit":
                    {
                        this.Close();
                        break;
                    }
                default:
                    {
                        // Данный элемент меню не определён для обработчика
                        throw new NotImplementedException();
                        //break; 
                    }
            }
            return;
        }
        /// <summary>
        /// Предалает выбрать файл для загрузки конфигурации в менеджере сетей 
        /// </summary>
        private void OpenFile()
        {
            String msg;
            String path = String.Empty;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Открыть файл конфигурации";
            dlg.SupportMultiDottedExtensions = true;
            dlg.Filter = "Binary Config File (*.bin.nwc)|*.bin.nwc"; // nwc - NetWorks Configuration 
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;

            if (DialogResult.OK == dlg.ShowDialog(this))
            {
                try
                {
                    path = dlg.FileName;
                    Cursor.Current = Cursors.WaitCursor;
                    this._NetworksManager.LoadConfig(path);
                    this.SetPathToFile(path);
                    Cursor.Current = Cursors.Default;
                }
                catch (Exception ex)
                {
                    msg = String.Format("Не удалось загрузить конфигурацию из файла по причине: {0}", ex.Message);
                    MessageBox.Show(this, msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return;
        }
        /// <summary>
        /// Метод сохраняет конфигурацию менеджера сетей в файл
        /// </summary>
        private void SaveAsFile()
        {
            String msg;
            String path = String.Empty;

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Сохранить конфигурацию в файл";
            dlg.AddExtension = true;
            dlg.CheckFileExists = false;
            dlg.CheckPathExists = false;
            dlg.Filter = "Binary Congig File (*.bin.nwc)|*.bin.nwc"; // nwc - NetWorks Configuration
            dlg.CreatePrompt = false;
            dlg.OverwritePrompt = true;
            dlg.SupportMultiDottedExtensions = true;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                try
                {
                    this.NetworksManager.SaveConfig(dlg.FileName);
                    this.SetPathToFile(dlg.FileName);
                }
                catch (Exception ex)
                {
                    msg = String.Format("Не удалось сохранить файл. Ошибка: {0}", ex.Message);
                    MessageBox.Show(this, msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return;
        }
        /// <summary>
        /// Метод сохраняет конфигурацию менеджера сетей в файл
        /// </summary>
        private void SaveFile()
        {
            String msg;
            try
            {
                this._NetworksManager.SaveConfig(this._PathToFile);
            }
            catch (Exception ex)
            {
                msg = String.Format("Не удалось сохранить файл. Ошибка: {0}", ex.Message);
                MessageBox.Show(this, msg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }
        /// <summary>
        /// Закрывает файл.
        /// </summary>
        private void CloseFile()
        {            
            if (DialogResult.Yes == MessageBox.Show(this,
                "Сохранить текущие измениния?", "Сохрание файла",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                // Сохраняем конфигурацию в указанный файл
                this._NetworksManager.SaveConfig(this._PathToFile);
            }

            this.SetPathToFile(null);

            return;
        }
        /// <summary>
        /// Вызывает менеджер конфигурации для работы с конфигурацией системы
        /// </summary>
        private void CreateNewFile()
        {
            PropertyGrid grid;
            grid = (PropertyGrid)this._SplitContainerMain.Panel2.Controls["_PropertyGridMain"];
            grid.SelectedObject = this.NetworksManager;

            // Закрываем текущий файл, блокируем\разблокируем меню для работы с файлом конфигурации 
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuNew"].Enabled = false;
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuOpenFile"].Enabled = true;
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuCloseFile"].Enabled = true;
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSave"].Enabled = true;
            ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSaveAs"].Enabled = true;

            return;
        }
        /// <summary>
        /// Устанавливает путь к фалу конфигурации и 
        /// управляет доступностью элементов меню
        /// </summary>
        /// <param name="path">Путь к файлу конфигурации, елси null - файл закрыт</param>
        private void SetPathToFile(String path)
        {
            PropertyGrid grid;
            ToolStripLabel lableItem;

            if (path == null)
            {
                this._PathToFile = null;
                // Закрываем текущий файл, блокируем\разблокируем меню для работы с файлом конфигурации 
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuNew"].Enabled = true;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuOpenFile"].Enabled = true;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuCloseFile"].Enabled = false;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSave"].Enabled = false;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSaveAs"].Enabled = false;
                
                grid = (PropertyGrid)this._SplitContainerMain.Panel2.Controls["_PropertyGridMain"];
                grid.SelectedObject = null;

                lableItem = (ToolStripLabel)this._StatusStripMain.Items["_ToolStripLabelPathToFile"];
                lableItem.Text = String.Empty;
            }
            else
            {
                this._PathToFile = path;
                // Открываем файл конфигурации для работы, блокируем\разблокируем меню для работы с файлом конфигурации 
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuNew"].Enabled = false;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuOpenFile"].Enabled = false;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuCloseFile"].Enabled = true;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSave"].Enabled = true;
                ((ToolStripMenuItem)this._MenuStripMain.Items["_MenuFile"]).DropDownItems["_MenuSaveAs"].Enabled = true;

                grid = (PropertyGrid)this._SplitContainerMain.Panel2.Controls["_PropertyGridMain"];
                grid.SelectedObject = this._NetworksManager;

                lableItem = (ToolStripLabel)this._StatusStripMain.Items["_ToolStripLabelPathToFile"];
                lableItem.Text = this._PathToFile;
            }
            return;
        }
        #endregion
    }
}
