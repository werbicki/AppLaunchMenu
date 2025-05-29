using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using AppLaunchMenu.DataAccess;
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
    public sealed partial class MainWindow : Window
    {
        private bool m_blnCloseRequested = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MenuFile p_objMenuFile)
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;

            WindowHelper.CenterOnScreen(this, new Size(600, 700));

            Navigate(typeof(LaunchMenu), p_objMenuFile);
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
            ContentFrame.Navigate(pageType, args, navigationTransitionInfo);
        }

        private async void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (!m_blnCloseRequested)
            {
                args.Handled = true;

                ContentDialog objExitDialog = new ContentDialog
                {
                    XamlRoot = this.Content.XamlRoot,
                    Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                    Title = "Exit AppLaunchMenu?",
                    Content = "Would you like to exit AppLaunchMenu?",
                    CloseButtonText = "OK",
                    PrimaryButtonText = "Cancel",
                    DefaultButton = ContentDialogButton.Primary
                };

                ContentDialogResult objResult = await objExitDialog.ShowAsync();

                if (objResult != ContentDialogResult.Primary)
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

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
        }
    }

    public class MainWindowPageArgs
    {
        public MainWindow? MainWindow;
        public object? Parameter;
    }
}
