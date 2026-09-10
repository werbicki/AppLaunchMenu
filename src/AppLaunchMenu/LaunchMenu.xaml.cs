using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using AppLaunchMenu.Dialogs;
using AppLaunchMenu.ViewModels;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    public class TabViewMenuPage : PageNotifyPropertyChanged
    {
        private readonly MenuPage m_objMenuPage;
        private readonly MenuViewModel m_objMenuViewModel;

        public TabViewMenuPage(MenuPage p_objMenuPage, MenuViewModel p_objMenuViewModel)
        {
            m_objMenuPage = p_objMenuPage;
            m_objMenuViewModel = p_objMenuViewModel;
        }

        public new string Name
        {
            get { return m_objMenuViewModel.Name; }
        }

        public MenuPage MenuPage
        {
            get { return m_objMenuPage; }
        }

        public bool EditMode
        {
            get { return m_objMenuViewModel.EditMode; }
        }

        public IconSource Icon
        {
            get { return m_objMenuViewModel.Icon; }
        }

    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LaunchMenu : PageNotifyPropertyChanged, ILaunchMenu
    {
        private LaunchMenuFile m_objMenuFile = new();
        private MenuFileViewModel m_objMenuFileViewModel;
        private ObservableCollection<TabViewMenuPage> m_objTabViewMenuPages = new ObservableCollection<TabViewMenuPage>();
        private ApplicationViewModel? m_objSelectedApplication = null;
        private bool m_blnShowEnvironment = false;
        private string m_strCommandLine = "Select an application from the list and press the 'Launch' button.";

        public LaunchMenu()
        {
            InitializeComponent();
            EmptyViewModel.EmptyChild = new EmptyViewModel(this);

            m_objMenuFile.FileChanged += MenuFile_FileChanged;

            m_objMenuFileViewModel = new MenuFileViewModel(m_objMenuFile, this);
            m_objMenuFileViewModel.MenuListViewModel.PropertyChanged += MenuListViewModel_OnPropertyChanged;

            m_objLogoImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/CompanyName.png"));

            InializeMenus();
        }

        public LaunchMenu(bool p_blnEmptyConstructor)
        {
            m_objMenuFileViewModel = new MenuFileViewModel(m_objMenuFile, this);
            m_objMenuFileViewModel.MenuListViewModel.PropertyChanged += MenuListViewModel_OnPropertyChanged;

            InializeMenus();
        }

        private void InializeMenus()
        {
            m_objTabViewMenuPages.Clear();

            foreach (MenuViewModel objMenuViewModel in m_objMenuFileViewModel.MenuListViewModel.Menus)
                m_objTabViewMenuPages.Add(new TabViewMenuPage(new MenuPage(this, objMenuViewModel), objMenuViewModel));
        }

        private void LaunchMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //if (m_objMenuFileViewModel.MapNetworkDrives)
            //    MapNetworkDrives_ClickAsync(this, new RoutedEventArgs());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            MainWindowPageArgs args = (MainWindowPageArgs)e.Parameter;
            LaunchMenuFile? objMenuFile = null;

            if (args.Parameter != null)
                objMenuFile = (LaunchMenuFile)args.Parameter;

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

                InializeMenus();

                m_objLogoImage.Source = new BitmapImage(new Uri(m_objMenuFile.LogoImagePath, UriKind.Absolute));

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
                OnPropertyChanged(nameof(HasEditAccess));
                OnPropertyChanged(nameof(EditMode));
            }
            else if ((e.PropertyName == "LocalDomainName")
                || (e.PropertyName == "LocalUsername")
                || (e.PropertyName == "LocalHostname")
                || (e.PropertyName == "LocalIpAddress")
                || (e.PropertyName == "LocalDataCenter")
                )
                OnPropertyChanged(nameof(StatusText));
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

        private async void SaveAs_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker objFileOpenPicker = new FileOpenPicker(App.MainWindow.AppWindow.Id)
            {
                // (Optional) Specify the initial location for the picker. 
                //     If the specified location doesn't exist on the user's machine, it falls back to the DocumentsLibrary.
                //     If not set, it defaults to PickerLocationId.Unspecified, and the system will use its default location.
                SuggestedStartFolder = m_objMenuFile.Directory,

                // (Optional) specify the text displayed on the commit button. 
                //     If not specified, the system uses a default label of "Open" (suitably translated).
                CommitButtonText = "Choose selected files",

                // (Optional) specify file extension filters. If not specified, defaults to all files (*.*).
                FileTypeFilter = { ".xml" },

                // (Optional) specify the view mode of the picker dialog. If not specified, defaults to List.
                ViewMode = PickerViewMode.List,
            };

            PickFileResult objPickFileResult = await objFileOpenPicker.PickSingleFileAsync();

            if (objPickFileResult != null)
                m_objMenuFile.SaveAs(objPickFileResult.Path);
        }

        private async void MapNetworkDrives_ClickAsync(object sender, RoutedEventArgs e)
        {
            MapNetworkDrives objMapNetworkDrives = new MapNetworkDrives(m_objMenuFileViewModel.NetworkDriveListViewModel);

            ModalDialog objMapNetworkDrivesDialog = new ModalDialog(new Size(500, 300))
            {
                //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme,
                Title = "Map Network Drives",
                IsResizable = true,
                Page = objMapNetworkDrives,
                CloseButtonText = "OK",
                DefaultButton = ContentDialogButton.Close
            };

            // Allow the Page to request the ModalDialog to Close() itself.
            objMapNetworkDrives.CloseRequested += (s, e) =>
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    objMapNetworkDrivesDialog.Close();
                });
            };

            ContentDialogResult objResult = await objMapNetworkDrivesDialog.ShowAsync();
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

        private bool HasEditAccess
        {
            get { return m_objMenuFile.HasEditAccess; }
        }

        public bool EditMode
        {
            get { return m_objMenuFile.EditMode; }
            set
            { 
                if (m_objMenuFile.HasEditAccess)
                {
                    m_objMenuFile.EditMode = value;
                    OnPropertyChanged(nameof(EditMode));
                }
            }
        }

        public MenuFileViewModel MenuFileViewModel
        {
            get { return m_objMenuFileViewModel; }
        }

        public ObservableCollection<TabViewMenuPage> Menus
        {
            get { return m_objTabViewMenuPages; }
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
            get { return "Username: " + m_objMenuFileViewModel.LocalDomainName + "\\" + m_objMenuFileViewModel.LocalUsername + ", Hostname: " + m_objMenuFileViewModel.LocalHostname + " (" + m_objMenuFileViewModel.LocalIpAddress.ToString() + "), Data Center: " + m_objMenuFileViewModel.LocalDataCenter; }
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
                ModalDialog objEnvironmentReviewDialog = new ModalDialog(new Size(1100, 700))
                {
                    //Style = Microsoft.UI.Xaml.Application.Current.Resources["DefaultContentDialogStyle"] as Style,
                    //RequestedTheme = (VisualTreeHelper.GetParent(sender as Button) as StackPanel).ActualTheme,
                    Title = p_objApplicationViewModel.Name,
                    IsResizable = true,
                    Page = new EnvironmentReview(p_objApplicationViewModel.Environment),
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
