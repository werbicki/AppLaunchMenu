using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public partial class MenuFileViewModel : ViewModelBase<MenuFile>
    {
        public MenuFileViewModel(MenuFile p_objMenuFile, LaunchMenu p_objLaunchMenu)
            : base(p_objMenuFile, p_objLaunchMenu)
        {
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
    }
}
