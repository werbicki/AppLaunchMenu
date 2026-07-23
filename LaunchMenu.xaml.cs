using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using AppLaunchMenu.Dialogs;
using AppLaunchMenu.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchMenu : PageNotifyPropertyChanged
    {
        private MenuFile m_objMenuFile = new();
        private MenuFileViewModel m_objMenuFileViewModel;
        private ApplicationViewModel? m_objSelectedApplication = null;
        private bool m_blnShowEnvironment = false;
        private string m_strCommandLine = "Select an application from the list and press the 'Launch' button.";
        private string m_strStatusText = "";
        private bool m_blnEditMode = false;

        public LaunchMenu()
        {
            InitializeComponent();
            EmptyViewModel.EmptyChild = new EmptyViewModel(this);

            m_objMenuFile.FileChanged += MenuFile_FileChanged;

            m_objMenuFileViewModel = new MenuFileViewModel(m_objMenuFile, this);
            m_objMenuFileViewModel.MenuListViewModel.PropertyChanged += MenuListViewModel_OnPropertyChanged;

            StatusText = "Hostname: " + System.Environment.MachineName + ", Username: " + System.Environment.UserDomainName + "\\" + System.Environment.UserName;
        }

        public LaunchMenu(bool p_blnEmptyConstructor)
        {
            m_objMenuFileViewModel = new MenuFileViewModel(m_objMenuFile, this);
            m_objMenuFileViewModel.MenuListViewModel.PropertyChanged += MenuListViewModel_OnPropertyChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainWindowPageArgs args = (MainWindowPageArgs)e.Parameter;
            MenuFile? objMenuFile = null;

            if (args.Parameter != null)
                objMenuFile = (MenuFile)args.Parameter;

            if (objMenuFile != null)
            {
                if (m_objMenuFile != null)
                    m_objMenuFile.FileChanged -= MenuFile_FileChanged;

                if (m_objMenuFileViewModel != null)
                    m_objMenuFileViewModel.PropertyChanged -= MenuListViewModel_OnPropertyChanged;

                m_objMenuFile = objMenuFile;
                m_objMenuFile.FileChanged += MenuFile_FileChanged;

                m_objMenuFileViewModel = new MenuFileViewModel(m_objMenuFile, this);
                m_objMenuFileViewModel.PropertyChanged += MenuListViewModel_OnPropertyChanged;

                OnPropertyChanged(nameof(Menus));
            }

            base.OnNavigatedTo(e);
        }

        private void MenuFile_FileChanged(object? sender, DataAccessBase.DataChangedEventArgs e)
        {
        }

        private void MenuListViewModel_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CanEdit")
            {
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(EditMode));
            }
            else if (e.PropertyName == "Menus")
                OnPropertyChanged(nameof(Menus));
            else if (e.PropertyName == "SelectedMenu")
                OnPropertyChanged(nameof(SelectedMenuIndex));
        }

        private void Reload_Click(object sender, RoutedEventArgs e)
        {
            m_objMenuFileViewModel.Reload(this);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            m_objMenuFile.Save();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.MainWindow?.Exit();
        }

        private async void Help_Click(object sender, RoutedEventArgs e)
        {
            ModalDialog objAboutDialog = new ModalDialog
            {
                //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                Title = "AppLaunchMenu",
                Message = "App Launch Menu",
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
            MenuViewModel objMenuViewModel = m_objMenuFileViewModel.MenuListViewModel.AddChild<MenuViewModel, Menu>("New Menu");
        }

        private void Menus_RemoveMenu(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            m_objMenuFileViewModel.MenuListViewModel.RemoveChild<MenuViewModel, Menu>((MenuViewModel)args.Item);
        }

        private bool CanEdit
        {
            get { return m_objMenuFile.CanEdit; }
        }

        public bool EditMode
        {
            get
            {
                if (m_objMenuFile.CanEdit)
                    return m_blnEditMode;
                return
                    false;
            }
            set
            {
                if (m_objMenuFile.CanEdit)
                {
                    m_blnEditMode = value;
                    OnPropertyChanged(nameof(EditMode));
                }
            }
        }

        internal MenuFileViewModel MenuFileViewModel
        {
            get { return m_objMenuFileViewModel; }
        }

        public ObservableCollection<MenuViewModel> Menus
        {
            get { return m_objMenuFileViewModel.MenuListViewModel.Menus; }
        }

        public int SelectedMenuIndex
        {
            get { return MenuFileViewModel.MenuListViewModel.SelectedMenuIndex; }
            set { MenuFileViewModel.MenuListViewModel.SelectedMenuIndex = value; }
        }

        public ApplicationViewModel? SelectedApplication
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
                    DataModels.Environment? objEnvironment = m_objSelectedApplication.Application.Environment;
                    if (objEnvironment != null)
                        CommandLine = m_objSelectedApplication.Application.GetExecutablePath(objEnvironment) + " " + m_objSelectedApplication.Application.GetParameters(objEnvironment);
                }
                else
                    CommandLine = "Select an application from the list and press the 'Launch' button.";

                OnPropertyChanged(nameof(SelectedApplication));
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

                OnPropertyChanged(nameof(ShowEnvironment));
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

                OnPropertyChanged(nameof(CommandLine));
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

                OnPropertyChanged(nameof(StatusText));
            }
        }

        public void Execute()
        {
            if (m_objSelectedApplication != null)
                Execute(m_objSelectedApplication);
        }

        private async void Execute(ApplicationViewModel p_objApplicationViewModel)
        {
            bool blnExecute = true;

            if (ShowEnvironment)
            {
                ModalDialog objEnvironmentReviewDialog = new ModalDialog()
                {
                    //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme,
                    Title = p_objApplicationViewModel.Name,
                    IsResizable = true,
                    Page = new EnvironmentReview(p_objApplicationViewModel),
                    CloseButtonText = "Cancel",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary
                };
                ContentDialogResult objDialogResult = await objEnvironmentReviewDialog.ShowAsync();

                if (objDialogResult != ContentDialogResult.Primary)
                    blnExecute = false;
            }

            if (blnExecute)
            {
                try
                {
                    p_objApplicationViewModel.Application.Execute(p_objApplicationViewModel.Application.Environment);
                }
                catch (Exception e)
                {
                    ModalDialog objErrorDialog = new ModalDialog
                    {
                        //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                        //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme
                        Title = "AppLaunchMenu",
                        Message = e.Message,
                        CloseButtonText = "OK",
                        DefaultButton = ContentDialogButton.None
                    };
                    ContentDialogResult objResult = await objErrorDialog.ShowAsync();
                }
            }
        }
    }
}
