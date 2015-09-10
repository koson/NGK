using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

//========================================================================================
namespace NGK.CorrosionMonitoringSystem.Forms.Controls
{
    //====================================================================================
    /// <summary>
    /// ����� ��� �������� ������ ��������� ������� ������������ ������
    /// �� ������� ����������
    /// </summary>
    public class SystemStatusBar: StatusStrip
    {
        #region Fields And Properties
        /// <summary>
        /// ����� ���������� ��������� � �������
        /// </summary>
        private Int32 _TotalDevices;
        /// <summary>
        /// ����� ���������� ��������� � �������
        /// </summary>
        public Int32 TotalDevices
        {
            get { return _TotalDevices; }
            set 
            {
                _TotalDevices = value;
                this._ToolStripLabelTotalDevices.Text =
                String.Format("����� ���������: {0}", value);
            }
        }
        /// <summary>
        /// ����� ���������� ����������� ���������
        /// </summary>
        private Int32 _FaultyDevices;
        /// <summary>
        /// ����� ���������� ����������� ���������
        /// </summary>
        public Int32 FaultyDevices
        {
            get { return _FaultyDevices; }
            set 
            {
                ToolStripButton btn = (ToolStripButton)this._ToolStripButtonFaultyDevices;
                
                _FaultyDevices = value;
                
                btn.Text = String.Format("����������� ���������: {0}", value);
                
                if (value == 0)
                {
                    btn.BackColor = this.BackColor;                   
                }
                else if (value > 0)
                {
                    btn.BackColor = System.Drawing.Color.Red;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("FaultyDevices", 
                        "�������� ������ ���� �����������");
                }
            }
        }
        public DateTime DateTime
        {
            set 
            {
                this._ToolStripLabelDateTime.Text = 
                    value.ToString(new System.Globalization.CultureInfo("ru-Ru"));
            }
        }
        /// <summary>
        /// ������� ������ ��������� ��� ���������� ������ ����� ��������� � �������
        /// </summary>
        private ToolStripStatusLabel _ToolStripLabelTotalDevices;
        /// <summary>
        /// ������ ������ ��������� ��� ����������� ������������ ��������� � �������
        /// </summary>
        private ToolStripButton _ToolStripButtonFaultyDevices;
        /// <summary>
        /// ������� ������ ��������� ��� ����������� ���������� ������� � ����
        /// </summary>
        private ToolStripStatusLabel _ToolStripLabelDateTime;
        
        #endregion

        #region Constructors
        //--------------------------------------------------------------------------------
        /// <summary>
        /// ����������� �� ���������
        /// </summary>
        public SystemStatusBar()
        {
            this._ToolStripLabelTotalDevices = new ToolStripStatusLabel();
            this._ToolStripLabelTotalDevices.Name = "ToolStripLabelTotalDevices";
            this._ToolStripLabelTotalDevices.AutoSize = true;
            this._ToolStripLabelTotalDevices.ToolTipText = "���������� ����� ���������� ��������� � �������";
            this._ToolStripLabelTotalDevices.AutoToolTip = true;
            this._ToolStripLabelTotalDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this._ToolStripLabelTotalDevices.BorderSides = ToolStripStatusLabelBorderSides.All;

            this._ToolStripButtonFaultyDevices = new ToolStripButton();
            this._ToolStripButtonFaultyDevices.Name = "ToolStripButtonFaultyDevices"; 
            this._ToolStripButtonFaultyDevices.AutoSize = true;
            this._ToolStripButtonFaultyDevices.ToolTipText = "����� ���������� ����������� ��������� � �������";
            this._ToolStripButtonFaultyDevices.AutoToolTip = true;
            this._ToolStripButtonFaultyDevices.DisplayStyle = ToolStripItemDisplayStyle.Text;

            this._ToolStripLabelDateTime = new ToolStripStatusLabel();
            this._ToolStripLabelDateTime.Name = "ToolStripLabelDateTime";
            this._ToolStripLabelDateTime.AutoSize = true;
            this._ToolStripLabelDateTime.ToolTipText = "��������� ���� � �����";
            this._ToolStripLabelDateTime.AutoToolTip = true;
            this._ToolStripLabelDateTime.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this._ToolStripLabelDateTime.BorderSides = ToolStripStatusLabelBorderSides.All;

            this.Items.AddRange(new ToolStripItem[] { this._ToolStripLabelTotalDevices,
                this._ToolStripButtonFaultyDevices, this._ToolStripLabelDateTime });

            this.TotalDevices = 0;
            this.FaultyDevices = 0;
        }
        //--------------------------------------------------------------------------------
        #endregion
        //--------------------------------------------------------------------------------
    }
    //====================================================================================
}
//========================================================================================
// End Of File