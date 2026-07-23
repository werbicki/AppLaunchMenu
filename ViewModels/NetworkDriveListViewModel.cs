using AppLaunchMenu.DataAccess;
using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public partial class NetworkDriveListViewModel : ViewModelTreeBase<NetworkDriveList>
    {
        public NetworkDriveListViewModel(NetworkDriveList p_objNetworkDriveList, LaunchMenu p_objLaunchMenu)
            : base(p_objNetworkDriveList, p_objLaunchMenu)
        {
        }

        protected override void OnLoadChildren()
        {
            foreach (NetworkDriveViewModel objNetworkDriveViewModel in Collection<NetworkDriveViewModel, NetworkDrive>())
                Children.Add(objNetworkDriveViewModel);
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<NetworkDriveViewModel> NetworkDrives
        {
            get { return Collection<NetworkDriveViewModel, NetworkDrive>(); }
        }
    }
}
