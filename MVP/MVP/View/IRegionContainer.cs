using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Mvp.View
{
    public interface IRegionContainer
    {
        #region Fields And Properties
        
        /// <summary>
        /// �������� ������� �������������
        /// </summary>
        string Name { get; }

        //IRegionView RegionView { get; set; }

        ReadOnlyCollection<IPartialView> Views { get; }

        #endregion

        #region Methods

        /// <summary>
        /// ���������� � ��������� ����� ��������� �������������
        /// </summary>
        /// <param name="partialView">��������� �������������</param>
        void Add(IPartialView partialView);

        #endregion
    }
}
