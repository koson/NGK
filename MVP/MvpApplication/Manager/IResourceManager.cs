using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MvpApplication.Manager
{
    public interface IResourceManager
    {
        /// <summary>
        /// ¬озвращает изображение, если изображение с указанным
        /// именем не найдено, возвращает значение по умолчанию
        /// </summary>
        /// <param name="name">Ќаименование ресурса</param>
        /// <param name="defaultImage">«начение по умолчанию</param>
        /// <returns>–есурс</returns>
        Bitmap GetImage(string name, Image defaultImage);
        Bitmap GetLogo { get; }
    }
}
