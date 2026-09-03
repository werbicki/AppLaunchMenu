using AppLaunchMenu.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppLaunchMenu
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapNetworkDrives : PageNotifyPropertyChanged
    {
        public event EventHandler? CloseRequested;
        private NetworkDriveListViewModel m_objNetworkDriveListViewModel;

        public MapNetworkDrives(NetworkDriveListViewModel p_objNetworkDriveListViewModel)
        {
            m_objNetworkDriveListViewModel = p_objNetworkDriveListViewModel;

            this.InitializeComponent();
            DataContext = p_objNetworkDriveListViewModel;
        }

        public ObservableCollection<NetworkDriveViewModel> NetworkDrives
        {
            get { return m_objNetworkDriveListViewModel.AllNetworkDrives; }
        }

        private async void EnvironmentTable_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                bool blnSuccess = true;

                System.Threading.Thread.Sleep(1000);

                foreach (NetworkDriveViewModel objNetworkDriveViewModel in m_objNetworkDriveListViewModel.AllNetworkDrives)
                {
                    blnSuccess &= objNetworkDriveViewModel.MapNetworkDrive();
                }

                if (blnSuccess)
                {
                    System.Threading.Thread.Sleep(3000);

                    CloseRequested?.Invoke(this, EventArgs.Empty);
                }
            });
        }
    }
}
