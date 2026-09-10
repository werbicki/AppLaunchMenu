using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public class ServiceViewModel : ViewModelTreeBase<Service>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get { return []; }
        }

        public ServiceViewModel(Service p_objService, ILaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objService, p_objLaunchMenu, p_objParent)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (ServerViewModel objServerViewModel in Collection<ServerViewModel, Server>(this))
                Children.Add(objServerViewModel);
        }

        [DialogContent("Executable Path")]
        public string ExecutablePath
        {
            get { return DataModel.ExecutablePath; }
            set
            {
                DataModel.ExecutablePath = value;
                OnPropertyChanged(nameof(ExecutablePath));
            }
        }
    }
}
