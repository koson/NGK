using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;
using System.Windows.Forms;
using Infrastructure.Api.Controls;

namespace Infrastructure.Api.Plugins
{
    /// <summary>
    /// �� ������� ������ ������ ������������� ��� 
    /// ���������� ��������� ������������� � ������ �������, �������������� � ��������.
    /// ������ ��������� ��������������� � WorkingRegion ������� �����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PartialViewPresenter<T> : RegionPresenter<T>, IPartialViewPresenter
        where T : IPartialView
    {
        #region Constructors

        public PartialViewPresenter()
        {
            _FunctionalButtons = new List<FunctionalButton>();
        }
        
        #endregion

        #region Fields And Properties

        private readonly List<FunctionalButton> _FunctionalButtons;

        /// <summary>
        /// ��������� (��������������� � TitleRegion ������� �����)
        /// </summary>
        public abstract string Title { get; }
        /// <summary>
        /// �������������� ������. ����������� �� �����������
        /// ������ ������. � ������ ������ �� ������ ���� �� ����� 3 (F3, F4, F5)
        /// </summary>
        IEnumerable<FunctionalButton> IPartialViewPresenter.FunctionalButtons
        {
            get { return _FunctionalButtons; }
        }

        protected IList<FunctionalButton> FunctionalButtons
        {
            get { return _FunctionalButtons; }
        }

        #endregion

        #region Methods

        public override void Close()
        {
            foreach (FunctionalButton button in FunctionalButtons)
            {
                button.Hide();
                button.Dispose();
            }
        }

        #endregion
    }
}
