using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuFileViewModel : ViewModelTreeBase<LaunchMenuFile>
    {
        public MenuFileViewModel(LaunchMenuFile p_objMenuFile, LaunchMenu p_objLaunchMenu)
            : base(p_objMenuFile, p_objLaunchMenu)
        {
            p_objMenuFile.FileChanged += MenuFile_FileChanged; 
        }

        private void MenuFile_FileChanged(object? sender, DataAccessBase.DataChangedEventArgs e)
        {
            ReloadChildren();
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

        public void Reload(LaunchMenu p_objLaunchMenu)
        {
            DataModel.Reload();

            OnPropertyChanged(nameof(MenuListViewModel));
        }

        protected override void OnLoadChildren()
        {
            Children.Add(NetworkDriveListViewModel);
            Children.Add(ScriptListViewModel);
            Children.Add(EnvironmentViewModel);
        }
    }
}
