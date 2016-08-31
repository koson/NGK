using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Mvp.View.Collections.ObjectModel;

namespace Mvp.View
{
    public abstract class WindowView<T>: IWindowView where T: Form
    {
        #region Costructors

        public WindowView()
        {
            _Regions = new RegionContainersCollection();
            _Form = Activator.CreateInstance<T>();
        }

        #endregion

        #region Fields And Properties

        private readonly T _Form;
        private readonly RegionContainersCollection _Regions;
        private WindowViewContext<WindowView<T>> _Context;

        public T Form { get { return _Form; } }

        Form IWindowView.Form { get { return _Form; } }

        public string Name
        {
            get { return Form.Name; }
            set { Form.Name = value; }
        }

        public virtual WindowViewContext<WindowView<T>> Context
        {
            get { return _Context; }
            set { _Context = value; }
        }

        public RegionContainersCollection Regions
        {
            get { return _Regions; }
        }

        /// <summary>
        /// TODO: это рудимент - проверить и удалить
        /// </summary>
        public ViewType ViewType
        {
            get { return ViewType.Window; }
        }

        #endregion

        #region Methods

        public void Show()
        {
            _Form.Show();
        }

        public void Hide()
        {
            _Form.Hide();
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            _Form.Dispose();
        }


        #endregion

        #region Events


        #endregion
    }
}
