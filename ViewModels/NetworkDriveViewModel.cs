using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class NetworkDriveViewModel : TreeViewItemViewModel
    {
        private readonly NetworkDriveListViewModel m_objNetworkDriveListViewModel;
        protected NetworkDrive m_objNetworkDrive;

        public NetworkDriveViewModel(LaunchMenu? p_objLaunchMenu, NetworkDriveListViewModel p_objNetworkDriveListViewModel, NetworkDrive p_objVariable)
            : base(p_objLaunchMenu)
        {
            m_objNetworkDriveListViewModel = p_objNetworkDriveListViewModel;
            m_objNetworkDrive = p_objVariable;
        }

        internal NetworkDrive NetworkDrive
        {
            get { return m_objNetworkDrive; }
        }

        public override string Name
        {
            get { return m_objNetworkDrive.Name; }
            set
            {
                m_objNetworkDrive.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string RemoteUncPath
        {
            get { return m_objNetworkDrive.RemoteUncPath; }
            set
            {
                m_objNetworkDrive.RemoteUncPath = value;
                OnPropertyChanged(nameof(RemoteUncPath));
            }
        }

        public string LocalDriveLetter
        {
            get { return m_objNetworkDrive.LocalDriveLetter; }
            set
            {
                m_objNetworkDrive.LocalDriveLetter = value;
                OnPropertyChanged(nameof(LocalDriveLetter));
            }
        }

        public bool Persistent
        {
            get { return m_objNetworkDrive.Persistent; }
            set
            {
                m_objNetworkDrive.Persistent = value;
                OnPropertyChanged(nameof(Persistent));
            }
        }

        public bool UnmapFirst
        {
            get { return m_objNetworkDrive.UnmapFirst; }
            set
            {
                m_objNetworkDrive.UnmapFirst = value;
                OnPropertyChanged(nameof(UnmapFirst));
            }
        }

        public bool ForceUnmap
        {
            get { return m_objNetworkDrive.ForceUnmap; }
            set
            {
                m_objNetworkDrive.ForceUnmap = value;
                OnPropertyChanged(nameof(ForceUnmap));
            }
        }
    }
}
