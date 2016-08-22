using System;
using System.Collections.Generic;
using System.Text;
using Mvp.Presenter;
using Mvp.View;

namespace PluginsInfrastructure
{
    /// <summary>
    /// От данного класса должны наследоваться все 
    /// презентеры частичных представлений в данном проекте, представленных в плагинах.
    /// Данный презентер устанавливается в WorkingRegion главной формы
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PartialViewPresenter<T>: RegionPresenter<T>, IPartialViewPresenter
        where T : IRegionView
    {
        #region Fields And Properties

        /// <summary>
        /// Заголовок (устанавливается в TitleRegion главной формы)
        /// </summary>
        public abstract string Title { get; }


        #endregion
    }
}
