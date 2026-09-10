using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System.Net;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuFileViewModel : ViewModelTreeBase<LaunchMenuFile>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(Folder), ViewModelType = typeof(FolderViewModel) },
                    new() { DataModelType = typeof(Environment), ViewModelType = typeof(EnvironmentViewModel) },
                    new() { DataModelType = typeof(Application), ViewModelType = typeof(ApplicationViewModel) },
                ];
            }
        }

        public MenuFileViewModel(LaunchMenuFile p_objMenuFile, ILaunchMenu p_objLaunchMenu)
            : base(p_objMenuFile, p_objLaunchMenu)
        {
            p_objMenuFile.FileChanged += MenuFile_FileChanged; 
        }

        private void MenuFile_FileChanged(object? sender, DataAccessBase.DataChangedEventArgs e)
        {
            ReloadChildren();
            OnPropertyChanged(nameof(LocalDomainName));
            OnPropertyChanged(nameof(LocalUsername));
            OnPropertyChanged(nameof(LocalHostname));
            OnPropertyChanged(nameof(LocalIpAddress));
            OnPropertyChanged(nameof(LocalDataCenter));
            OnPropertyChanged(nameof(LogoImage));
        }

        public string LocalDomainName
        {
            get { return DataModel.LocalDomainName; }
        }

        public string LocalUsername
        {
            get { return DataModel.LocalUsername; }
        }

        public string LocalHostname
        {
            get { return DataModel.LocalHostname; }
        }

        public IPAddress LocalIpAddress
        {
            get { return DataModel.LocalIpAddress; }
        }

        public string LocalDataCenter
        {
            get { return DataModel.LocalDataCenter; }
        }

        [DialogContent("Map Network Drives")]
        public bool MapNetworkDrives
        {
            get { return DataModel.MapNetworkDrives; }
            set
            {
                DataModel.MapNetworkDrives = value;
                OnPropertyChanged(nameof(MapNetworkDrives));
            }
        }

        [DialogContent("Logo Image")]
        public string LogoImage
        {
            get { return DataModel.LogoImage; }
            set
            {
                DataModel.LogoImage = value;
                OnPropertyChanged(nameof(LogoImage));
            }
        }

        public DataCenterListViewModel DataCenterListViewModel
        {
            get { return ViewModel<DataCenterListViewModel, DataCenterList>(DataModel.DataCenterList); }
        }

        public NetworkDriveListViewModel NetworkDriveListViewModel
        {
            get { return ViewModel<NetworkDriveListViewModel, NetworkDriveList>(DataModel.NetworkDriveList); }
        }

        public ScriptListViewModel ScriptListViewModel
        {
            get { return ViewModel<ScriptListViewModel, ScriptList>(DataModel.ScriptList); }
        }

        public MenuListViewModel MenuListViewModel
        {
            get { return ViewModel<MenuListViewModel, MenuList>(DataModel.MenuList); }
        }

        public EnvironmentViewModel EnvironmentViewModel
        {
            get { return ViewModel<EnvironmentViewModel, Environment>(DataModel.Environment); }
        }

        public void Reload(ILaunchMenu p_objLaunchMenu)
        {
            DataModel.Reload();

            OnPropertyChanged(nameof(MenuListViewModel));
        }

        protected override void OnLoadChildren()
        {
            Children.Add(DataCenterListViewModel);
            Children.Add(NetworkDriveListViewModel);
            //Children.Add(ServerListViewModel);
            Children.Add(ScriptListViewModel);
            Children.Add(EnvironmentViewModel);
        }
    }
}
