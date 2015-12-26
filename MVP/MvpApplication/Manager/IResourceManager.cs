using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MvpApplication.Manager
{
    public interface IResourceManager
    {
        /// <summary>
        /// ���������� �����������, ���� ����������� � ���������
        /// ������ �� �������, ���������� �������� �� ���������
        /// </summary>
        /// <param name="name">������������ �������</param>
        /// <param name="defaultImage">�������� �� ���������</param>
        /// <returns>������</returns>
        Bitmap GetImage(string name, Image defaultImage);
        Bitmap GetLogo { get; }
    }
}
