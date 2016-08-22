using System;
using System.Collections.Generic;
using System.Text;
using Mvp.View;
using System.Collections.ObjectModel;

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
            _RegionPresenters = new List<IRegionPresenter>();
        }

        #endregion

        #region Fields And Properties

        private IRegionContainer _RegionContainer;
        private List<IRegionPresenter> _RegionPresenters;

        /// <summary>
        /// �������� �������
        /// </summary>
        public string Name { get { return _RegionContainer.Name; } }
        /// <summary>
        /// ��������� ���� �� ������ ��� ���
        /// </summary>
        public bool IsEmpty 
        { 
            get { return _RegionPresenters.Count == 0; } 
        }

        public ReadOnlyCollection<IRegionPresenter> RegionPresenters
        {
            get { return _RegionPresenters.AsReadOnly(); }
        }

        /// <summary>
        /// ������������� ��� ���������� ��������� ���������� ���� � ������ �����
        /// </summary>
        //public IRegionPresenter RegionPresenter 
        //{
        //    get { return _RegionPresenter; }
        //    set
        //    {
        //        if (!IsEmpty)
        //            _RegionPresenter.Close();

        //        _RegionPresenter = value;
        //        _RegionContainer.RegionView = _RegionPresenter.View;
        //    } 
        //}
        
        #endregion

        #region Methods
        /// <summary>
        /// ������������� ��������� ���������� ���� � ������ �����
        /// </summary>
        /// <param name="presenter"></param>
        public void Add(IRegionPresenter presenter)
        {
            _RegionPresenters.Add(presenter);
            _RegionContainer.Add(presenter.View);
            presenter.Shown += new EventHandler(EventHandler_PartialViewPresenter_Shown);
        }
        /// <summary>
        /// ������� ��������� ���������� ���� �� ������� �����
        /// </summary>
        /// <param name="presenter"></param>
        public void Remove(IRegionPresenter presenter)
        {
            throw new NotImplementedException();
        }

        private void EventHandler_PartialViewPresenter_Shown(object sender, EventArgs e)
        {
            IRegionPresenter presenter = (IRegionPresenter)sender;
            // �����, ������������ ���� �� ����������� ����������, ��� ������ ����������
            foreach (IPartialView view in _RegionContainer.Views)
            {
                if (!view.Equals(presenter.View))
                    view.Hide();
            }
        }

        public void Dispose()
        {
            foreach(IRegionPresenter presenter in _RegionPresenters)
                presenter.Dispose();
        }

        #endregion
    }
}
