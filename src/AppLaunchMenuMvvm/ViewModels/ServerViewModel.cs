using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;

namespace AppLaunchMenu.ViewModels
{
    public class ServerViewModel : ViewModelTreeBase<Server>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get { return []; }
        }

        public ServerViewModel(Server p_objServer, ILaunchMenu p_objLaunchMenu, ITreeViewItem p_objParent)
            : base(p_objServer, p_objLaunchMenu, p_objParent)
        {
        }

        internal Server Server
        {
            get { return DataModel; }
        }

        [DialogContent("Service Username")]
        public string ServiceUsername
        {
            get { return DataModel.ServiceUsername; }
            set
            {
                DataModel.ServiceUsername = value;
                OnPropertyChanged(nameof(ServiceUsername));
            }
        }
    }
}
