using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;

namespace PluginsInfrastructure
{
    /// <summary>
    /// �� ������� ������ ������ ������������� ��� 
    /// ���������� ��������� ������������� � ������ �������, �������������� � ��������.
    /// ������ ��������� ��������������� � WorkingRegion ������� �����
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PartialViewPresenter<T>: RegionPresenter<T>, IPartialViewPresenter
        where T : IRegionView
    {
        #region Fields And Properties

        /// <summary>
        /// ��������� (��������������� � TitleRegion ������� �����)
        /// </summary>
        public abstract string Title { get; }


        #endregion
    }
}
