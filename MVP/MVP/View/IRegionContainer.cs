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
        /// Название региона представления
        /// </summary>
        string Name { get; }

        //IRegionView RegionView { get; set; }

        ReadOnlyCollection<IPartialView> Views { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Доабавляет в контейнер новое частичное представление
        /// </summary>
        /// <param name="partialView">Частичное предстваление</param>
        void Add(IPartialView partialView);

        #endregion
    }
}
