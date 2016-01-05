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
            //��� ������������� ���������� (�������� �������� � �.�.
            WinFormsApplication app = (WinFormsApplication)sender;

            //������ presenter splash screen 
            BootstrapperView bootstrapperView = new BootstrapperView();
            BootstrapperPresenter bootstrapperPresenter = 
                new BootstrapperPresenter(app, bootstrapperView, null);
            //��������� ����� ��� ���������� ������������� ����������
            bootstrapperPresenter.SystemInitializationRunning += 
                new EventHandler(BootstrapperBlock);
            
            //MainScreenView mainScreenView = new MainScreenView();
            //MainScreenPresenter mainScreenPresenter =
            //    new MainScreenPresenter(app, mainScreenPresenter, null);

            app.ShowWindow(bootstrapperPresenter);
        }

        // ����� ��� ������������� �������. ����������� � ��������� ������
        static void BootstrapperBlock(object sender, EventArgs e)
        {
            BootstrapperPresenter presenter = (BootstrapperPresenter)sender;

            presenter.WtriteText("�������� ������������...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("���������� ������������...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("�������� ��...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("�������� ������� �������...");
            System.Threading.Thread.Sleep(2000);
            presenter.WtriteText("������ ������� �����������...");
            System.Threading.Thread.Sleep(2000);
        }
    }
}