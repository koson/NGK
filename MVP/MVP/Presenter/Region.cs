using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;

namespace Mvp.Presenter
{
    /// <summary>
    /// �������� ��� ���������� ������� ����� ��� ����������� ��������� ����� 
    /// </summary>
    public class Region: IDisposable
    {
        #region Constructors

        public Region(IRegionContainer regionContainer)
        {
            _RegionContainer = regionContainer;
        }

        #endregion

        #region Fields And Properties

        private IRegionContainer _RegionContainer;
        private IRegionPresenter _RegionPresenter;

        /// <summary>
        /// �������� �������
        /// </summary>
        public string Name { get { return _RegionContainer.Name; } }
        /// <summary>
        /// ��������� ���� �� ������ ��� ���
        /// </summary>
        public bool IsEmpty { get { return _RegionPresenter == null; } }
        /// <summary>
        /// ������������� ��� ���������� ��������� ���������� ���� � ������ �����
        /// </summary>
        public IRegionPresenter RegionPresenter 
        {
            get { return _RegionPresenter; }
            set
            {
                if (!IsEmpty)
                    _RegionPresenter.Close();

                _RegionPresenter = value;
                _RegionContainer.RegionView = _RegionPresenter.View;
            } 
        }
        
        #endregion

        #region Methods

        public void Dispose()
        {
            _RegionPresenter.Dispose();
        }

        #endregion
    }
}
