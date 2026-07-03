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
    public partial class NetworkDriveListViewModel : TreeViewItemViewModel
    {
        LaunchMenu m_objLaunchMenu;
        NetworkDriveList m_objNetworkDriveList;
        ObservableCollection<NetworkDriveViewModel> m_objNetworkDrives = new();

        public NetworkDriveListViewModel(LaunchMenu p_objLaunchMenu, MenuFile p_objMenuFile)
            : base(p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objNetworkDriveList = new NetworkDriveList(p_objMenuFile, "NetworkDrives");
        }

        public NetworkDriveListViewModel(LaunchMenu p_objLaunchMenu, NetworkDriveList p_objNetworkDriveList)
            : base(p_objLaunchMenu)
        {
            m_objLaunchMenu = p_objLaunchMenu;
            m_objNetworkDriveList = p_objNetworkDriveList;
        }

        public override string Name
        {
            get
            {
                if (string.IsNullOrEmpty(m_objNetworkDriveList.Name))
                    return "NetworkDriveList";
                return m_objNetworkDriveList.Name;
            }
            set
            {
                m_objNetworkDriveList.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public override bool Expanded
        {
            get { return false; }
        }

        public ObservableCollection<NetworkDriveViewModel> NetworkDrives
        {
            get { return m_objNetworkDrives; }
        }

        public NetworkDriveViewModel? AddNetworkDrive(LaunchMenu p_objLaunchMenu, String p_strNetworkDrive)
        {
            NetworkDriveViewModel? objNetworkDriveViewModel = null;
            NetworkDrive? objNetworkDrive = m_objNetworkDriveList.MenuFile.AddNetworkDrive(p_strNetworkDrive);
            if (objNetworkDrive != null)
            {
                objNetworkDriveViewModel = new NetworkDriveViewModel(p_objLaunchMenu, this, objNetworkDrive);
                m_objNetworkDrives.Add(objNetworkDriveViewModel);

                this.OnPropertyChanged(nameof(NetworkDrives));
            }

            return objNetworkDriveViewModel;
        }

        public void RemoveNetworkDrive(NetworkDriveViewModel p_objNetworkDriveViewModel)
        {
            if (m_objNetworkDrives.Remove(p_objNetworkDriveViewModel))
            {
                m_objNetworkDriveList.MenuFile.RemoveNetworkDrive(p_objNetworkDriveViewModel.NetworkDrive);

                this.OnPropertyChanged(nameof(NetworkDrives));
            }
        }

        protected override void OnLoadChildren()
        {
            foreach (DataModelBase objItem in m_objNetworkDriveList.Items)
            {
                if (objItem is NetworkDrive objNetworkDrive)
                    Children.Add(new NetworkDriveViewModel(LaunchMenu, this, objNetworkDrive));
            }
        }
    }
}
