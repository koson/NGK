using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    public interface IRegionPresenter: IDisposable
    {
        #region Properties

        IPartialView View { get; }

        #endregion
        
        #region Methods

        void Show();
        void Hide();
        void Close();
        
        #endregion
        
        #region Events

        /// <summary>
        /// ������� ���������� ��� ������ ������ Show � ����������� ���������� �������������
        /// </summary>
        event EventHandler Shown;

        #endregion
    }
}
