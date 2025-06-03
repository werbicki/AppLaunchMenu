using Windows.UI.ViewManagement;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using AppLaunchMenu.Helper;
using AppLaunchMenu.DataModels;
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

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
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

            string[] arrArguments = Environment.GetCommandLineArgs();
            if (arrArguments.Length > 1)
            {
                for (int intArgument = 1; intArgument < arrArguments.Length; intArgument++)
                {
                    FileInfo objFileInfo = new FileInfo(arrArguments[1]);
                    if (objFileInfo.Exists)
                    {
                        MenuFile objMenuFile = new MenuFile(objFileInfo.FullName);
                        objMenuFiles.Add(objMenuFile);
                    }
                }
            }
            else
            {
                MenuFile objMenuFile = new MenuFile(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName + ".menu");
                objMenuFiles.Add(objMenuFile);
            }

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
