using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System.Collections.ObjectModel;

namespace AppLaunchMenu.ViewModels
{
    public partial class NetworkDriveListViewModel : ViewModelTreeBase<NetworkDriveList>
    {
        protected override ViewModelMapping[] ViewModelMappings
        {
            get
            {
                return
                [
                    new() { DataModelType = typeof(NetworkDrive), ViewModelType = typeof(NetworkDriveViewModel) },
                ];
            }
        }

        protected ObservableCollection<NetworkDriveViewModel> m_objAllNetworkDrives = new ObservableCollection<NetworkDriveViewModel>();

        public NetworkDriveListViewModel(NetworkDriveList p_objNetworkDriveList, ILaunchMenu p_objLaunchMenu)
            : base(p_objNetworkDriveList, p_objLaunchMenu)
        {
            foreach (NetworkDrive objNetworkDrive in DataModel.NetworkDrives)
                m_objAllNetworkDrives.Add(new NetworkDriveViewModel(objNetworkDrive, p_objLaunchMenu, this));
        }

        protected override void OnLoadChildren()
        {
            foreach (NetworkDriveViewModel objNetworkDriveViewModel in Collection<NetworkDriveViewModel, NetworkDrive>(this))
                Children.Add(objNetworkDriveViewModel);
        }

        [DialogContent("Network Drive List Name")]
        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<NetworkDriveViewModel> NetworkDrives
        {
            get { return Collection<NetworkDriveViewModel, NetworkDrive>(); }
        }

        public ObservableCollection<NetworkDriveViewModel> AllNetworkDrives
        {
            get { return m_objAllNetworkDrives; }
        }
    }
}
