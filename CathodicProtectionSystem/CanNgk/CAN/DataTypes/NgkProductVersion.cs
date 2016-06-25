using System;
using System.Collections.Generic;
using System.Text;

namespace NGK.CAN.DataTypes
{
    /// <summary>
    /// ��������� ��� �������� ������� ��������� ���.
    /// </summary>
    /// <remarks>
    /// ������ ����������� �� ���� ������: �������� � ��������. ����� ��������� ������:
    /// DDD.DD , ��� ����� ����� - �������� ������, � ������� - �������� ������;
    /// �������� ����� �����������: 100 * ������� �����. �������� ���������� �������� 100�65500;
    /// ��������  ����� �����������: 1 * ������� �����. �������� ���������� �������� 1�99;
    /// </remarks>
    public struct NgkProductVersion: IEquatable<NgkProductVersion>
    {
        #region Fields And Properties
        /// <summary>
        /// ������ ������ �������� � ������� DDD.DD [Major.Minor]
        /// </summary>
        private UInt16 _Version;
        /// <summary>
        /// ���������� ��� ������������� ������ �������� ��� �������������� ��� �����
        /// ����� ���������� �������� � �������� ����� 
        /// </summary>
        public UInt16 TotalVersion
        {
            get { return _Version; }
            set
            {
                _Version = value;
            }
        }
        /// <summary>
        /// ���������� ��� ������������� �������� ����� �������
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// ������������ �������� �������� ����� ������ �������� ���</exception>
        public UInt16 Major
        {
            get { return System.Convert.ToUInt16((this._Version / 100)); }
            set
            {
                String msg;
                if (value > 655)
                {
                    msg = String.Format(
                        "������������ �������� �������� ����� ������ {0}, �� ����� ���� ������ 655",
                        value.ToString());
                    throw new ArgumentOutOfRangeException("Major", msg);
                }
                else
                {
                    // ������� �������� �����
                    this._Version &= 0x7F; // �������� ����� ���������� 7 �������� ������ �����
                    this._Version += (System.Convert.ToUInt16(value * 100));
                }
            }
        }
        /// <summary>
        /// ���������� ��� ������������� �������� ����� �������
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// ������������ �������� �������� ����� ������ �������� ���</exception>
        public Byte Minor
        {
            get { return System.Convert.ToByte((this._Version % 100)); }
            set
            {
                String msg;
                if (value > 99)
                {
                    msg = String.Format(
                        "������������ �������� �������� ����� ������ {0}, �� ����� ���� ������ 99",
                        value.ToString());
                    throw new ArgumentOutOfRangeException("Minor", msg);
                }
                else
                {
                    // ������� �������� �����
                    this._Version &= 0xF80; // �������� ����� ���������� 7 �������� ������ �����
                    this._Version += value;
                }
            }
        }
        /// <summary>
        /// ���������� ��� ������������� �������� � ���� System.Version
        /// </summary>
        public Version Version
        {
            get { return new Version(Convert.ToInt32(Major), Convert.ToInt32(Minor)); }
            set 
            {
                Minor = Convert.ToByte(value.Minor);
                Major = Convert.ToUInt16(value.Major);
            }
        }

        #endregion

        #region Constructors
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="versionBasis">������� �������� � ������� DDD.DD [Major.Minor]</param>
        public NgkProductVersion(UInt16 versionBasis)
        {
            this._Version = versionBasis;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="version"></param>
        public NgkProductVersion(Version version)
        {
            _Version = 0;
            Major = Convert.ToUInt16(version.Major);
            Minor = Convert.ToByte(version.Minor);
        }
        #endregion

        #region Methods
        /// <summary>
        /// ���������� ������ ������� ��� � ������� ������������� ����� DDD,DD
        /// </summary>
        /// <returns>������ �������</returns>
        public float ToFloat()
        {
            return this._Version / 100;
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}",
                this.Major.ToString("D3"),
                this.Minor.ToString("D2")); ;
        }

        public static bool operator ==(NgkProductVersion v1, NgkProductVersion v2)
        {
            return v1.TotalVersion == v2.TotalVersion;
        }

        public static bool operator !=(NgkProductVersion v1, NgkProductVersion v2)
        {
            return v1.TotalVersion != v2.TotalVersion;
        }

        public override bool Equals(object obj)
        {
            return obj is NgkProductVersion ?
                this.TotalVersion == ((NgkProductVersion)obj).TotalVersion : false;
            //return base.Equals(obj);
        }

        #endregion

        #region IEquatable<NgkProductVersion> Members

        public bool Equals(NgkProductVersion other)
        {
            return this.TotalVersion == other.TotalVersion;
        }

        #endregion
    }
}
