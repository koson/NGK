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
    /// ќт данного класса должны наследоватьс€ все 
    /// презентеры частичных представлений в данном проекте, представленных в плагинах.
    /// ƒанный презентер устанавливаетс€ в WorkingRegion главной формы
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
        /// «аголовок (устанавливаетс€ в TitleRegion главной формы)
        /// </summary>
        public abstract string Title { get; }
        /// <summary>
        /// ‘ункциональные кнопки. –асположены на всплывающей
        /// панели справа. ¬ данном случае их должно быть не более 3 (F3, F4, F5)
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
