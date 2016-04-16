using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CorrosionMonitoringSystem.Models.Modbus
{
    public class KIP9811Address : ModbusVisitingCard
    {
        /// <summary>
        /// ������� ������
        /// </summary>
        public const UInt16 Errors = 0x000C;
        /// <summary>
        /// ������� ������ �����������
        /// </summary>
        public const UInt16 ErrorsRegistration = 0x000D;
        /// <summary>
        /// ��������� ����������
        /// </summary>
        public const UInt16 DeviceStatus = 0x000E;
        /// <summary>
        /// �������� ���������
        /// </summary>
        public const UInt16 protection_pot = 0x000F;
        /// <summary>
        /// ��������������� ��������� ���������� ������������
        /// </summary>	
        public const UInt16 polarisation_pot = 0x0010;
        /// <summary>
        /// ��� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����
        /// </summary>
        public const UInt16 protection_cur = 0x0011;
        /// <summary>
        /// ��������� ���������� ���������� �� �����������
        /// </summary>
        public const UInt16 induced_ac = 0x0012;
        /// <summary>
        /// ��� ����������� ���������������� ���������
        /// </summary>
        public const UInt16 polarisation_cur = 0x0013;
        /// <summary>
        /// ��������� ���� ����������� ���������������� ���������.
        /// </summary>
        public const UInt16 density_cur = 0x0014;
        /// <summary>
        /// ��� �������������� ������ 1 
        /// </summary>
        public const UInt16 aux_cur1 = 0x0015;
        /// <summary>
        /// ��� �������������� ������ 2
        /// </summary>
        public const UInt16 aux_cur2 = 0x0016;
        /// <summary>
        /// ������� �������� ������� ��� � ���������� ������� 
        /// </summary>
        public const UInt16 corrosion_depth = 0x0017;
        /// <summary>
        /// �������� �������� ������� ��� � ���������� �������
        /// </summary>
        public const UInt16 corrosion_speed = 0x0018;
        /// <summary>
        /// ��������� �������
        /// </summary>
        public const UInt16 usikp_state = 0x0019;
        /// <summary>
        /// ��������� �������� ������� �1� �������� �������� 30,0-100,0 ��
        /// </summary>
        public const UInt16 corrosion_sense1 = 0x001A;
        /// <summary>
        /// ��������� �������� ������� �2� �������� �������� 30,0-100,0 ��	
        /// </summary>
        public const UInt16 corrosion_sense2 = 0x001B;
        /// <summary>
        /// ��������� �������� ������� �3� �������� �������� 30,0-100,0 ��	
        /// </summary>
        public const UInt16 corrosion_sense3 = 0x001C;
        /// <summary>
        /// ��� ��������� �� ����������	
        /// </summary>
        public const UInt16 polarisation_cur_dc = 0x001D;
        /// <summary>
        /// ��� ��������� �� ����������	
        /// </summary>
        public const UInt16 polarisation_cur_ac = 0x001E;
        /// <summary>
        /// ��������� ���� ��������� �� ����������� 
        /// </summary>
        public const UInt16 density_pol_cur_dc = 0x001F;
        /// <summary>
        /// ��������� ���� ��������� �� �����������
        /// </summary>
        public const UInt16 density_pol_cur_ac = 0x0020;
        //��������������� 
        //reserved3 =	0x0021
        /// <summary>
        /// ���������� ����������� �������� �������	
        /// </summary>
        public const UInt16 battery_voltage = 0x0022;
        /// <summary>
        /// ����������� ����������� ������� ��(�) 	
        /// </summary>
        public const UInt16 int_temp = 0x0023;
        /// <summary>
        /// ������� ���������������� ��������� (��)
        /// </summary>
        public const UInt16 electrod_area = 0x0024;
        /// <summary>
        /// ������ ��������� � �������� ���������� (������� ����)
        /// </summary>
        public const UInt16 meas_period_high = 0x0025;
        /// <summary>
        /// ������ ��������� � �������� ���������� (������� ����)
        /// </summary>
        public const UInt16 meas_period_low = 0x0026;
        //���������������	0x0027
        /// <summary>
        /// ������ ������ �������
        /// </summary>
        public const UInt16 usikp_period = 0x0028;
        /// <summary>
        /// ������ ������ �������� ��������
        /// </summary>
        public const UInt16 corr_sense_period = 0x0029;
        /// <summary>
        /// ������ ������ �������������� ������ 1 4-20 ��	
        /// </summary>
        public const UInt16 aux1_period = 0x002A;
        /// <summary>
        /// ������ ������ �������������� ������ 2 4-20 ��
        /// </summary>
        public const UInt16 aux2_period = 0x002B;
        /// <summary>
        /// ����������� ��� �������� ����� (�)
        /// </summary>
        public const UInt16 shunt_nom = 0x002C;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���������������� ���������� ���������� ������������.
        /// </summary>
        public const UInt16 polarisation_pot_en = 0x002D;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ��������� ����������.
        /// </summary>
        public const UInt16 protection_pot_en = 0x002E;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���� �������� ������ � ����� ������� ������� ��������� ���������� �� ������� �����.	
        /// </summary>
        public const UInt16 protection_cur_en = 0x002F;
        /// <summary>
        /// ���� ���������� ������ ������ ���� ����������� ���������������� ���������	
        /// </summary>
        public const UInt16 polarisation_cur_en = 0x0030;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���������� ����������� ���������� �� �����������
        /// </summary>
        public const UInt16 induced_ac_en = 0x0031;
        /// <summary>
        /// ���� ���������� �������� ����� ���������
        /// </summary>
        public const UInt16 status_flags_en = 0x0032;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���� ��������� �� �����������	
        /// </summary>
        public const UInt16 polarisation_cur_dc_en = 0x0033;
        /// <summary>
        /// ���� ���������� ������ ������ ��������� ���� ��������� �� �����������
        /// </summary>
        public const UInt16 polarisation_cur_ac_en = 0x0034;
        /// <summary>
        /// ���������� ��� ���������� �������� PDO
        /// </summary>
        public const UInt16 pdo_flags = 0x0035;
        /// <summary>
        /// ������� ����� ���������� 
        /// </summary>
        public const UInt16 datetime_high = 0x0036;
        /// <summary>
        /// ������� ����� ���������� 
        /// </summary>
        public const UInt16 datetime_low = 0x0037;
    }
}
