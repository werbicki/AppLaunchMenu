using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using AppLaunchMenu.DataAccess;
using AppLaunchMenu.Helper;
using AppLaunchMenu.ViewModels;
using System.Collections.ObjectModel;
using Windows.UI;
using Windows.Devices.Enumeration;
using AppLaunchMenu.DataModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchMenu : Page, INotifyPropertyChanged
    {
        private MenuFile m_objMenuFile = new();
        private MenuListViewModel m_objMenusViewModel;
        private DataModels.Application? m_objSelectedApplication = null;
        private bool m_blnShowEnvironment = true;
        private string m_strCommandLine = "Select an application from the list and press the 'Launch' button.";
        private string m_strStatusText = "";
        private int m_intSelectedMenu = 0;
        private bool m_blnEditMode = false;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var eventHandler = PropertyChanged;
            if (eventHandler != null)
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
        }

        public LaunchMenu()
        {
            m_objMenusViewModel = new(this);

            InitializeComponent();

            StatusText = "Hostname: " + System.Environment.MachineName + ", Username: " + System.Environment.UserDomainName + "\\" + System.Environment.UserName;
        }

        public LaunchMenu(MenuFile p_objMenuFile)
        {
            InitializeComponent();

            m_objMenuFile = p_objMenuFile;
            m_objMenusViewModel = new MenuListViewModel(this, m_objMenuFile);

            StatusText = "Hostname: " + System.Environment.MachineName + ", Username: " + System.Environment.UserDomainName + "\\" + System.Environment.UserName;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainWindowPageArgs args = (MainWindowPageArgs)e.Parameter;
            if (args.Parameter != null)
                m_objMenuFile = (MenuFile)args.Parameter;
            if (m_objMenuFile != null)
                m_objMenusViewModel = new MenuListViewModel(this, m_objMenuFile);

            base.OnNavigatedTo(e);
        }

        private bool CanEdit
        {
            get { return m_objMenusViewModel.CanEdit; }
        }

        public bool EditMode
        {
            get
            {
                if (m_objMenusViewModel.CanEdit)
                    return m_blnEditMode;
                return
                    false;
            }
            set
            {
                if (m_objMenusViewModel.CanEdit)
                {
                    m_blnEditMode = value;
                    OnPropertyChanged(nameof(EditMode));
                }
            }
        }

        public ObservableCollection<MenuViewModel> Menus
        {
            get
            {
                if (m_objMenusViewModel != null)
                    return m_objMenusViewModel.Menus;
                return [];
            }
        }

        public MenuViewModel? SelectedMenu
        {
            get
            {
                if ((Menus.Count > 0) && (m_intSelectedMenu < Menus.Count))
                    return Menus[m_intSelectedMenu];
                return null;
            }
            set
            {
                if (value != null)
                {
                    m_intSelectedMenu = Menus.IndexOf(value);

                    OnPropertyChanged(nameof(SelectedMenu));
                }
            }
        }

        public DataModels.Application? SelectedApplication
        {
            get
            {
                return m_objSelectedApplication;
            }
            set
            {
                m_objSelectedApplication = value;

                if (m_objSelectedApplication != null)
                {
                    DataModels.Environment? objEnvironment = m_objSelectedApplication.Environment;
                    if (objEnvironment != null)
                        CommandLine = m_objSelectedApplication.GetExecutablePath(objEnvironment) + " " + m_objSelectedApplication.GetParameters(objEnvironment);
                }
                else
                    CommandLine = "Select an application from the list and press the 'Launch' button.";
            }
        }

        public bool ShowEnvironment
        {
            get
            {
                return m_blnShowEnvironment;
            }
            set
            {
                m_blnShowEnvironment = value;
            }
        }

        public string CommandLine
        {
            get
            {
                return m_strCommandLine;
            }
            set
            {
                m_strCommandLine = value;
                m_objCommandLine.Text = m_strCommandLine;
            }
        }

        public string StatusText
        {
            get
            {
                return m_strStatusText;
            }
            set
            {
                m_strStatusText = value;
                m_objStatusText.Text = m_strStatusText;
            }
        }

        public void Execute()
        {
            if (m_objSelectedApplication != null)
                Execute(m_objSelectedApplication);
        }

        private async void Execute(DataModels.Application p_objApplication)
        {
            if (ShowEnvironment)
            {
                MainWindow objNewWindow = new MainWindow();
                objNewWindow.Title = p_objApplication.Name;

                // Navigate to the first page, configuring the new page
                // by passing required information as a navigation parameter
                objNewWindow.Navigate(typeof(EnvironmentReview), p_objApplication);

                // Ensure the new MainWindow is active
                WindowHelper.CenterOnScreen(objNewWindow, new Size(1200, 800));
                objNewWindow.Activate();
            }
            else
            {
                try
                {
                    p_objApplication.Execute(p_objApplication.Environment);
                }
                catch (Exception e)
                {
                    ContentDialog objErrorDialog = new ContentDialog
                    {
                        XamlRoot = this.Content.XamlRoot,
                        Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                        Title = "AppLaunchMenu",
                        Content = e.Message,
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.None
                    };

                    ContentDialogResult objResult = await objErrorDialog.ShowAsync();
                }
            }
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            m_objMenusViewModel?.Reload(this);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            m_objMenuFile?.Save();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow?.Exit();
        }

        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog objAboutDialog = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                Title = "AppLaunchMenu",
                Content = "App Launch Menu",
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.None
            };

            ContentDialogResult objResult = await objAboutDialog.ShowAsync();
        }

        private void LaunchButton_Click(object sender, RoutedEventArgs e)
        {
            Execute();
        }

        private void Menus_AddMenu(TabView sender, object args)
        {
            MenuViewModel? objMenuViewModel = m_objMenusViewModel?.AddMenu(this, "New Menu");
            SelectedMenu = objMenuViewModel;
        }

        private void Menus_RemoveMenu(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            m_objMenusViewModel?.RemoveMenu((MenuViewModel)args.Item);
        }
    }
}
