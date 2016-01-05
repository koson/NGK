using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Mvp.WinApplication;
using MvpApplication2.Presenter;
using MvpApplication2.View;

namespace MvpApplication2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            WinFormsApplication app = new WinFormsApplication();
            app.ApplicationStarting += 
                new EventHandler(EventHandler_app_ApplicationRunning);
            app.Run();
        }

        private static void EventHandler_app_ApplicationRunning(
            object sender, EventArgs e)
        {
            //Код инициализации приложения (загрузка ресурсов и т.п.
            WinFormsApplication app = (WinFormsApplication)sender;

            //Создаём presenter splash screen 
            BootstrapperView bootstrapperView = new BootstrapperView();
            BootstrapperPresenter bootstrapperPresenter = 
                new BootstrapperPresenter(app, bootstrapperView, null);
            //Подключем метод для выполнения инициализации приложения
            bootstrapperPresenter.SystemInitializationRunning += 
                new EventHandler(BootstrapperBlock);
            
            //MainScreenView mainScreenView = new MainScreenView();
            //MainScreenPresenter mainScreenPresenter =
            //    new MainScreenPresenter(app, mainScreenPresenter, null);

            app.ShowWindow(bootstrapperPresenter);
        }

        // Здесь код инициализации системы. Выполняется в отдельном потоке
        static void BootstrapperBlock(object sender, EventArgs e)
        {
            BootstrapperPresenter presenter = (BootstrapperPresenter)sender;

            presenter.WtriteText("Загрузка конфигурации...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("Применение конфигурации...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("Загрузка БД...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("Загрузка журнала событий...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("Запуск системы мониторинга...");
            System.Threading.Thread.Sleep(2000);
        }
    }
}