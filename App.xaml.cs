﻿using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using AppLaunchMenu.Helper;
using AppLaunchMenu.DataAccess;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        static private MainWindow? m_objWindow;
        private string m_strMenuFilePath = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".menu";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        public App(string p_strMenuFilePath)
        {
            m_strMenuFilePath = p_strMenuFilePath;
            this.InitializeComponent();
        }

        static public MainWindow? MainWindow
        {
            get { return m_objWindow; }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            List<MenuFile> objMenuFiles = new List<MenuFile>();

            MenuFile objMenuFile = new MenuFile(m_strMenuFilePath);
            objMenuFiles.Add(objMenuFile);

            if (objMenuFiles.Count > 0)
            {
                m_objWindow = new MainWindow(objMenuFiles[0]);

                m_objWindow.Activate();
            }
            else
            {
                //MessageBox.Show("Command-line Options: AppLauncher.exe MenuFile [MenuFile...]", "AppLauncher");
                Application.Current.Exit();
            }
        }
    }
}
