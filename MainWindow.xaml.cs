using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using AppLaunchMenu.DataAccess;
using AppLaunchMenu.Dialogs;
using AppLaunchMenu.Helper;
using AppLaunchMenu.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Hosting;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.Core;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.WindowManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : WindowNotifyPropertyChanged
    {
        private MenuFile m_objMenuFile;
        private bool m_blnCloseRequested = false;

        public MainWindow(MenuFile p_objMenuFile)
        {
            m_objMenuFile = p_objMenuFile;
            m_objMenuFile.DataChanged += MenuFile_DataChanged;

            InitializeComponent();
            RootElement.DataContext = this;

            // Hides the default system title bar and replaces the  system title bar with the WinUI TitleBar control. 
            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            WindowHelper.CenterOnScreen(this, new Size(600, 700));

            this.Closed += MainWindow_Closed;

            Navigate(typeof(LaunchMenu), p_objMenuFile);
        }

        private void MenuFile_DataChanged(object? sender, DataModels.DataAccessBase.DataChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AppTitle));
        }

        // Wraps a call to rootFrame.Navigate to give the Page a way to know which NavigationRootPage is navigating.
        // Please call this function rather than rootFrame.Navigate to navigate the rootFrame.
        public void Navigate(Type pageType, object? targetPageArguments = null, NavigationTransitionInfo? navigationTransitionInfo = null)
        {
            MainWindowPageArgs args = new()
            {
                MainWindow = this,
                Parameter = targetPageArguments
            };

            InnerFrame.Navigate(pageType, args, navigationTransitionInfo);
        }

        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (!m_blnCloseRequested)
            {
                args.Handled = true;

                ModalDialog objExitDialog = new ModalDialog()
                {
                    //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme,
                    Title = "Exit AppLaunchMenu?",
                    Message = "Would you like to exit AppLaunchMenu?",
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary
                };

                ContentDialogResult objDialogResult = await objExitDialog.ShowAsync();

                if (objDialogResult == ContentDialogResult.Primary)
                {
                    m_blnCloseRequested = true;
                    Application.Current.Exit();
                }

            }
        }

        public void Exit()
        {
            m_blnCloseRequested = true;
            Application.Current.Exit();
        }

        private String AppTitle
        {
            get { return "AppLaunchMenu - " + m_objMenuFile.Filename + (m_objMenuFile.IsDirty ? "*" : ""); }
        }
    }

    public class MainWindowPageArgs
    {
        public MainWindow? MainWindow;
        public object? Parameter;
    }
}
