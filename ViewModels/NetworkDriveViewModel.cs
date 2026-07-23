using AppLaunchMenu.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace AppLaunchMenu.ViewModels
{
    public class NetworkDriveViewModel : ViewModelTreeBase<NetworkDrive>
    {
        public NetworkDriveViewModel(NetworkDrive p_objNetworkDrive, LaunchMenu p_objLaunchMenu)
            : base(p_objNetworkDrive, p_objLaunchMenu)
        {
        }

        internal NetworkDrive NetworkDrive
        {
            get { return DataModel; }
        }

        public override string Name
        {
            get { return DataModel.Name; }
            set
            {
                DataModel.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string RemoteUncPath
        {
            get { return DataModel.RemoteUncPath; }
            set
            {
                DataModel.RemoteUncPath = value;
                OnPropertyChanged(nameof(RemoteUncPath));
            }
        }

        public string LocalDriveLetter
        {
            get { return DataModel.LocalDriveLetter; }
            set
            {
                DataModel.LocalDriveLetter = value;
                OnPropertyChanged(nameof(LocalDriveLetter));
            }
        }

        public bool Persistent
        {
            get { return DataModel.Persistent; }
            set
            {
                DataModel.Persistent = value;
                OnPropertyChanged(nameof(Persistent));
            }
        }

        public bool UnmapFirst
        {
            get { return DataModel.UnmapFirst; }
            set
            {
                DataModel.UnmapFirst = value;
                OnPropertyChanged(nameof(UnmapFirst));
            }
        }

        public bool ForceUnmap
        {
            get { return DataModel.ForceUnmap; }
            set
            {
                DataModel.ForceUnmap = value;
                OnPropertyChanged(nameof(ForceUnmap));
            }
        }
    }
}
