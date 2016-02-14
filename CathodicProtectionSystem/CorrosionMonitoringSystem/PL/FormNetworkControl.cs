using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NGK.CAN.ApplicationLayer.Network.Master;
using NGK.CAN.ApplicationLayer.Network.Master.Services;

namespace NGK.CorrosionMonitoringSystem.Forms
{
    public partial class FormNetworkControl : Form
    {
        #region Fields And Properties
        /// <summary>
        /// 
        /// </summary>
        NetworksManager _NetworksManager;
        /// <summary>
        /// Singleton
        /// </summary>
        private static FormNetworkControl _Instance;
        /// <summary>
        /// 
        /// </summary>
        public static FormNetworkControl Instance
        {
            get 
            {
                lock (SyncRoot)
                {
                    if (_Instance == null)
                    {
                        _Instance = new FormNetworkControl();
                    }
                }
                return _Instance;
            }
        }
        private static Object SyncRoot = new object();
        #endregion

        #region Constructor
        /// <summary>
        /// 
        /// </summary>
        private FormNetworkControl()
        {
            InitializeComponent();
            Load += new EventHandler(EventHandler_FormNetworkControl_Load);
            FormClosed += new FormClosedEventHandler(FormNetworkControl_FormClosed);
        }

        #endregion

        #region Event Handlers
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventHandler_FormNetworkControl_Load(object sender, EventArgs e)
        {
            _NetworksManager = NetworksManager.Instance;
            Text = "Управление сетями НГК CAN";
            Icon = Properties.Resources.faviconMy;
            // Инициализируем системное дерево
            InitTreeViewSystem();
        }
        private void FormNetworkControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            lock (SyncRoot)
            {
                _Instance = null;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// </summary>
        private void InitTreeViewSystem()
        {
            TreeNode node;
            TreeNode childNode;

            _TreeViewSystem.Nodes.Clear();
            _TreeViewSystem.AfterSelect += 
                new TreeViewEventHandler(_TreeViewSystem_AfterSelect);
            node = new TreeNode();
            node.Name = "RootNode";
            node.Text = "Сети НГК CAN";
            _TreeViewSystem.Nodes.Add(node);
            _TreeViewSystem.TopNode = node;

            foreach (NetworkController controller in _NetworksManager.Networks)
            {
                node = new TreeNode();
                node.Name = "NodeNetworkId" + controller.NetworkId.ToString();
                node.Text = String.Format("NetwrokId {0} ({1})", 
                    controller.NetworkId, controller.NetworkName);
                node.Tag = controller.NetworkId;
 
                foreach (Service service in controller.Services)
                {
                    childNode = new TreeNode();
                    childNode.Name = node.Name + service.ServiceType.ToString();
                    childNode.Text = service.ServiceType.ToString();
                    childNode.Tag = service.ServiceType;
                    node.Nodes.Add(childNode);
                }
                _TreeViewSystem.TopNode.Nodes.Add(node);
            }
        }

        private void _TreeViewSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UInt32 networkId;
            TreeNode node = e.Node;
            
            switch (e.Action)
            {
                case TreeViewAction.ByKeyboard:
                case TreeViewAction.ByMouse:
                    {
                        if (node.Tag == null)
                        {
                            return;
                        }

                        _PropertyGridViewer.SelectedObject = null;
                        if (node.Tag is ServiceType)
                        {
                            networkId = (UInt32)node.Parent.Tag;
                            NetworkController controller = _NetworksManager.Networks[networkId];
                            ServiceType type = (ServiceType)node.Tag;
                            switch (type)
                            {
                                case ServiceType.BootUp:
                                    {
                                        _PropertyGridViewer.SelectedObject = 
                                            (ServiceBootUp)controller.Services[type];
                                        break;
                                    }
                                //case ServiceType.Emcy:
                                //    {
                                //        _PropertyGridViewer.SelectedObject =
                                //            (Service)controller.Services[type];
                                //        break;
                                //    }
                                case ServiceType.Nmt:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServiceNmt)controller.Services[type];
                                        break;
                                    }
                                case ServiceType.NodeGuard:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServiceNodeGuard)controller.Services[type];
                                        break;
                                    }
                                case ServiceType.PdoReceive:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServicePdoReceive)controller.Services[type];
                                        break;
                                    }
                                case ServiceType.PdoTransmit:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServicePdoTransmit)controller.Services[type];
                                        break;
                                    }
                                case ServiceType.SdoUpload:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServiceSdoUpload)controller.Services[type];
                                        break;
                                    }
                                case ServiceType.Sync:
                                    {
                                        _PropertyGridViewer.SelectedObject =
                                            (ServiceSync)controller.Services[type];
                                        break;
                                    }
                                default:
                                    { throw new NotSupportedException(); }
                            }
                        }
                        if (node.Tag is UInt32)
                        {
                            networkId = (UInt32)node.Tag;
                            _PropertyGridViewer.SelectedObject = _NetworksManager.Networks[networkId];
                        }
                        break;
                    }
            }
        }

        #endregion
    }
}